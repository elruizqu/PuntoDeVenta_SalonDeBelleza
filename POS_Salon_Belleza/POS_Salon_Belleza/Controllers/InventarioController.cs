using Microsoft.AspNetCore.Mvc;

namespace POS_Salon_Belleza.Controllers
{
    public class InventarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Agregar()
        {
            return View();
        }
    }
}
