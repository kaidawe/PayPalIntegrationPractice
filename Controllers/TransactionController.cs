using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PaypalLab.Controllers
{
    [Authorize(Roles = "Manager")]
    public class TransactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
