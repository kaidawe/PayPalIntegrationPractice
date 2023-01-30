using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaypalLab.Repositories;
using PaypalLab.ViewModels;

namespace PaypalLab.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class RoleController : Controller
    {
        ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Role
        public ActionResult Index()
        {
            RoleRepo roleRepo = new RoleRepo(_context);
            return View(roleRepo.GetAllRoles());
        }

        public ActionResult Create()
        {

            return View();

        }


        [HttpPost]
        public IActionResult Create(RoleVM createRole)
        {

            if (ModelState.IsValid)
            {
                string message = "";

                RoleRepo cr = new RoleRepo(_context);

                bool newRole = cr.CreateRole(createRole.RoleName);

                if (newRole)
                {
                    message = $"Success creating Role {createRole.RoleName}";
                    return RedirectToAction("Index");
                }
                else
                {
                    message = $"Creating role {createRole.RoleName} not sucessful";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }
    }

}