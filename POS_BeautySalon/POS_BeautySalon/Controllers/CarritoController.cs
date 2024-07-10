using DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace POS_BeautySalon.Controllers
{
    public class CarritoController : Controller
    {
        private readonly SalonContext _salonContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarritoController(SalonContext salonContext, UserManager<ApplicationUser> userManager)
        {
            _salonContext = salonContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var carrito = ObtenerCarritoDelUsuarioActual();
            var productosEnCarrito = _salonContext.CarritoProductos
                .Include(cp => cp.Producto)
                .Where(cp => cp.CarritoId == carrito.CarritoId)
                .ToList();

            //Calcular el total del carrito
            var total = carrito.CalcularTotal();

            //Pasar el valor Total a la vista
            ViewBag.Total = total;

            return View(productosEnCarrito);
        }

        //Método para agregar productos al carrito
        public async Task<IActionResult> AgregarAlCarrito(int productoId)
        {
            var carrito = ObtenerCarritoDelUsuarioActual();

            var carritoProducto = new CarritoProducto
            {
                CarritoId = carrito.CarritoId,
                ProductoId = productoId,
                Cantidad = 1
            };

            _salonContext.Add(carritoProducto);
            await _salonContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Método para editar cantidad de un producto del carrito
        public async Task<IActionResult> EditarCantidad(int carritoProductoId, int nuevaCantidad)
        {
            var carritoProducto = await _salonContext.CarritoProductos.FindAsync(carritoProductoId);
            var producto = await _salonContext.Productos.SingleAsync(p => p.ProductoId == 
            carritoProducto.ProductoId);

            if(producto.Cantidad < nuevaCantidad)
            {
                TempData["Error"] = "La cantidad digitada excede la cantidad disponible de este producto.";
                return RedirectToAction(nameof(Index));
            }

            carritoProducto.Cantidad = nuevaCantidad;

            _salonContext.Update(carritoProducto);
            await _salonContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Método para eliminar un producto del carrito
        public async Task<IActionResult> EliminarDelCarrito(int carritoProductoId)
        {
            var carritoProducto = await _salonContext.CarritoProductos.FindAsync(carritoProductoId);

            _salonContext.CarritoProductos.Remove(carritoProducto);
            await _salonContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Método complementario para obtener el carrito del usuario autenticado
        private Carrito ObtenerCarritoDelUsuarioActual()
        {
            var clienteId = _userManager.GetUserId(User);
            var carrito = _salonContext.Carritos.FirstOrDefault(c => c.ClienteId == clienteId);

            if (carrito == null)
            {
                carrito = new Carrito { ClienteId = clienteId };
                _salonContext.Carritos.Add(carrito);
                _salonContext.SaveChanges();
            }
            return carrito;
        }

        //Método para imprimir la factura
        public IActionResult ImprimirFactura()
        {
            var carrito = ObtenerCarritoDelUsuarioActual();
            var productosEnCarrito = _salonContext.CarritoProductos
                .Include(cp => cp.Producto)
                .Where(cp => cp.CarritoId == carrito.CarritoId)
                .ToList();

            var total = carrito.CalcularTotal();
            var consecutivo = GenerarConsecutivo();

            //Pasar los detalles de la factura a la vista
            ViewBag.ProductosEnCarrito = productosEnCarrito;
            ViewBag.Total = total;
            ViewBag.Consecutivo = consecutivo;

            return View("ImprimirFactura");
        }

        //Método para generar un número de consecutivo aleatorio
        private string GenerarConsecutivo()
        {
            var rng = new Random();
            var builder = new StringBuilder();

            for(int i = 0; i< 25; i++)
            {
                builder.Append(rng.Next(0,10));
            }

            return builder.ToString();
        }


        //Confirmacion de Compra
        [HttpPost]
        public async Task<IActionResult> ConfirmarCompra(string metodoPago, string consecutivo)
        {
            if ((metodoPago == "Transferencia" || metodoPago == "Sinpe") && string.IsNullOrEmpty(consecutivo))
            {
                return Json(new { success = false, message = "El número de consecutivo es requerido para el método de pago seleccionado." });
            }

            var carrito = ObtenerCarritoDelUsuarioActual();

            var productosEnCarrito = _salonContext.CarritoProductos
                .Include(cp => cp.Producto)
                .Where(cp => cp.CarritoId == carrito.CarritoId)
                .ToList();

            var total = carrito.CalcularTotal();

            // Crear y guardar la factura
            var factura = new Factura
            {
                ClienteId = carrito.ClienteId,
                PrecioTotal = total,
                Fecha = DateTime.Now
            };

            _salonContext.Facturas.Add(factura);
            await _salonContext.SaveChangesAsync();



            // Limpiar el carrito
            _salonContext.CarritoProductos.RemoveRange(productosEnCarrito);
            await _salonContext.SaveChangesAsync();

            return Json(new { success = true, message = "Compra confirmada con éxito." });
        }


    }
}
