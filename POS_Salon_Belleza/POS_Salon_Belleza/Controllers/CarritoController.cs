using Microsoft.AspNetCore.Mvc;

namespace POS_Salon_Belleza.Controllers
{
    public class CarritoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
