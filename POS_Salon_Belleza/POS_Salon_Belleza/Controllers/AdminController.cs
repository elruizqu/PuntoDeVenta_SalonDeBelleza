using Microsoft.AspNetCore.Mvc;

namespace POS_Salon_Belleza.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }
    }
}
