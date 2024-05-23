using Microsoft.AspNetCore.Mvc;

namespace POS_Salon_Belleza.Controllers
{
    public class CatalogoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
