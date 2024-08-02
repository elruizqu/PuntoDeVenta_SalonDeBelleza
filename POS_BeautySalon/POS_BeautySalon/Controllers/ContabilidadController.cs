using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.Extensions.Hosting;

namespace POS_BeautySalon.Controllers
{
    public class ContabilidadController : Controller
    {
        private readonly SalonContext _salonContext;

        public ContabilidadController(SalonContext salonContext)
        {
            _salonContext = salonContext;
        }

        public IActionResult Index()
        {
            var resumenVentas = ObtenerResumenVentas();
            var reporteBalances = ObtenerReporteBalances();
            var facturasDelDia = ObtenerFacturasDelDia(); // Obtener facturas del día

            // Pasar todos los datos a la vista usando ViewBag
            ViewBag.ResumenVentas = resumenVentas;
            ViewBag.ReporteBalances = reporteBalances;
            ViewBag.FacturasDelDia = facturasDelDia;

            return View();
        }

        private IEnumerable<Factura> ObtenerFacturasDelDia()
        {
            var hoy = DateTime.Today;
            return _salonContext.Facturas
                .Include(f => f.Cliente) // Incluye los clientes para mostrar sus nombres
                .Where(f => f.Fecha >= hoy && f.Fecha < hoy.AddDays(1))
                .ToList();
        }

        private dynamic ObtenerResumenVentas()
        {
            var ventas = _salonContext.Facturas
                .Include(f => f.DetalleFacturas)
                .ThenInclude(df => df.Producto)
                .Include(f => f.DetalleFacturas)
                .ThenInclude(df => df.Servicio)
                .ToList();

            var totalVentas = ventas.Count;  //cantidad de ventas
            var totalProductos = ventas.SelectMany(f => f.DetalleFacturas).Where(df => df.ProductoId != null).Sum(df => df.PrecioUnitario * df.Cantidad); //ganacia en venta de productos
            var totalServicios = ventas.SelectMany(f => f.DetalleFacturas).Where(df => df.ServicioId != null).Sum(df => df.PrecioUnitario); //ganacia en venta de productos

            return new
            {
                TotalVentas = totalVentas,
                TotalProductos = totalProductos,
                TotalServicios = totalServicios
            };
        }

        private dynamic ObtenerReporteBalances()
        {
            var productos = _salonContext.Productos.ToList();

            var totalStockValorado = productos.Sum(p => p.Cantidad * p.PrecioProveedor); //Costo de los productos en Stock actual, precio provedor * la cantidad que hay en stock
            var totalIngresos = _salonContext.Cierres.Sum(c => c.TotalCierre); //Suma el total de los cierres que tenemos para sacar la ganancia que llevamos
            var totalCostos = _salonContext.Facturas.SelectMany(f => f.DetalleFacturas).Where(df => df.ProductoId != null).Sum(df => df.Producto.PrecioProveedor * df.Cantidad);//Suma de los costos de los productos vendidos, calculados como PrecioProveedor * Cantidad para cada producto vendido.

            return new
            {
                TotalStockValorado = totalStockValorado,
                TotalIngresos = totalIngresos,
                TotalCostos = totalCostos
            };
        }
    }
}
