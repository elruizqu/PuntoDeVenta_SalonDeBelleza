using DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace POS_BeautySalon.Controllers
{
    public class CierreController : Controller
    {
        private readonly SalonContext _salonContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public CierreController(SalonContext salonContext, UserManager<ApplicationUser> userManager)
        {
            _salonContext = salonContext;
            _userManager = userManager;
        }

        // Mostrar todos los cierres registrados
        public IActionResult VerCierres()
        {
            var cierres = _salonContext.Cierres.ToList();
            return View(cierres);
        }

        public IActionResult Index()
        {
            // Mostrar las facturas del día actual
            var hoy = DateTime.Today;
            var facturasDeHoy = _salonContext.Facturas
                .Include(f => f.Cliente)
                .Where(f => f.Fecha >= hoy)
                .ToList();

            return View(facturasDeHoy);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCierre()
        {
            var hoy = DateTime.Today;

            // Obtener todas las facturas generadas hoy
            var facturasDeHoy = await _salonContext.Facturas
                .Include(f => f.Cliente)
                .Where(f => f.Fecha >= hoy)
                .ToListAsync();

            var totalProductos = facturasDeHoy
                .Where(f => f.ProductoId != null)
                .Sum(f => f.PrecioTotal);

            var totalServicios = facturasDeHoy
                .Where(f => f.ServicioId != null)
                .Sum(f => f.PrecioTotal);

            var totalCierre = totalProductos + totalServicios;

            var cierre = new Cierre
            {

                FechaCierre = DateTime.Now,
                TotalProductos = totalProductos,
                TotalServicios = totalServicios,
                TotalCierre = totalCierre
            };

            _salonContext.Cierres.Add(cierre);
            await _salonContext.SaveChangesAsync();

            return Json(new { success = true, message = "Cierre registrado con éxito." });
        }


        

        public IActionResult VerCierre()
        {
            var cierres = _salonContext.Cierres.ToList();
            return View(cierres);
        }

    }
}
    

