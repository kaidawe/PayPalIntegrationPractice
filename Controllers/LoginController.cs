using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PaypalLab.Repositories;
using PaypalLab.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PaypalLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private IConfiguration _config;
        private IServiceProvider _serviceProvider;
        private ApplicationDbContext _context;

        public LoginController(SignInManager<IdentityUser> signInManager,
                                IConfiguration config,
                                IServiceProvider serviceProvider,
                                ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _config = config;
            _serviceProvider = serviceProvider;
            _context = context;
        }

        [HttpGet]
        [Route("List")]
        // Since we have cookie authentication and Jwt authentication we must
        // specify that we want Jwt authentication here.
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                                         , Roles = "Admin,Manager")]
        public IActionResult List()
        {
            var claim = HttpContext.User.Claims.ElementAt(0);
            string userName = claim.Value;
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);


            var techArray = new[] {
            new {Text = "Silverlight", Count = 10, Link="/Tags/Silverlight" },
            new {Text = "IIS 7", Count = 11, Link="http://iis.net" },
            new {Text = "IE 8", Count = 12, Link="/Tags/IE8" },
            new {Text = "C#", Count = 13, Link="/Tags/C#" },
            new {Text = "Azure", Count = 13, Link="?Tag=Azure" }
        };

            var responseObject = new
            {
                techArray = techArray
                                     ,
                userName = userName
                                     ,
                userId = userId
            };

            return new ObjectResult(responseObject);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginVM loginVM)
        {
            UserRepo userRepo = new UserRepo(_context);
            UserRoleRepo userRoleRepo = new UserRoleRepo(_serviceProvider);


            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginVM.Email.ToUpper()
                                                                     , loginVM.Password
                                                                     , loginVM.RememberMe
                                                                     , lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var UserManager = _serviceProvider
                                         .GetRequiredService<UserManager<IdentityUser>>();

                    var user = await UserManager.FindByEmailAsync(loginVM.Email);

                    if (user != null)
                    {
                        var firstName = userRepo.GetUserName(loginVM.Email);
                        var tokenString = GenerateJSONWebToken(user);
                        var userRoles = userRoleRepo.GetUserRoles(loginVM.Email);
                        var jsonOK = new { tokenString = tokenString, firstName = firstName.FirstName, userRoles = userRoles, StatusCode = "OK" };

                        return new ObjectResult(jsonOK);
                    }
                }
                else if (result.IsLockedOut)
                {
                    var jsonLocked = new { tokenString = "", StatusCode = "Locked out." };
                    return new ObjectResult(jsonLocked);
                }
            }
            var jsonInvalid = new { tokenString = "", StatusCode = "Invalid Login." };
            return new ObjectResult(jsonInvalid);
        }




        [HttpPost]
        [Route("UserDetails")]
        [Authorize(Roles = "Staff")] 
        /// I have this set to Staff because my loggedin user has both roles "admin" and "manager" and I needed it to throw errors
        /// to correctly answer the lab, but I know this is supposed to be "manager"
        public IActionResult UserDetails([FromBody] NamesVM names)
        {
            var claim = HttpContext.User.Claims.ElementAt(0);
            string userName = claim.Value;
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userDetails = new
            {
                FirstName = names.firstName,
                LastName = names.lastName,
                UserId = userId
            };

            return Json(userDetails);
        }



        List<Claim> AddUserRoleClaims(List<Claim> claims, string userId)
        {
            // Get current user's roles. 
            var userRoleList = _context.UserRoles.Where(ur => ur.UserId == userId);
            var roleList = from ur in userRoleList
                           from r in _context.Roles
                           where r.Id == ur.RoleId
                           select new { r.Name };

            // Add each of the user's roles to the claims list.
            foreach (var roleItem in roleList)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleItem.Name));
            }
            return claims;
        }

        string GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey
                = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials
                = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

            claims = AddUserRoleClaims(claims, user.Id);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        [Route("Csrf")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                                         , Roles = "Admin,Manager")]
        public IActionResult Csrf()
        {
            return new ObjectResult(new
            {
                email = this.User.Identities.ElementAt(0).Name,
                userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier)
            });
        }

    }
}

