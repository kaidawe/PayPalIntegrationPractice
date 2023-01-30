using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaypalLab.Models;
using System.Diagnostics;

namespace PaypalLab.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }


        // Home page shows list of items.
        // Item price is set through the ViewBag.
        public IActionResult Index()
        {
            return View("Index", "3.55|CAD");
        }

        // Home page shows list of items.
        // Item price is set through the ViewBag.
        public IActionResult Transactions()
        {
            DbSet<IPN> items = _context.IPNs;

            return View(items);
        }

        // This method receives and stores
        // the Paypal transaction details.
        [HttpPost]
        public JsonResult PaySuccess([FromBody] IPN ipn)
        {
            try
            {
                _context.IPNs.Add(ipn);
                _context.SaveChanges();

                var aa = _context.IPNs;
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json(ipn);
        }

        // Home page shows list of items.
        // Item price is set through the ViewBag.
        public IActionResult Confirmation(string confirmationId)
        {
            IPN transaction = _context.IPNs.FirstOrDefault(t => t.paymentID == confirmationId);

            return View("Confirmation", transaction);
        }

        [Authorize]
        public IActionResult SecureArea()
        {
            // Get user name of user who is logged in.
            // This line must be in the controller.
            string userName = User.Identity.Name;
            // Usually this section would be in a repository.
            var registeredUser = _context.MyRegisteredUsers
            .Where(ru => ru.Email == userName)
            .FirstOrDefault(); // return one item
            return View(registeredUser);
        }

    }
}