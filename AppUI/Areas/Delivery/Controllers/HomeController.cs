using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppUI.Areas.Delivery.Controllers
{
    [Area("Delivery")]
    [Authorize(Roles = "Delivery")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}