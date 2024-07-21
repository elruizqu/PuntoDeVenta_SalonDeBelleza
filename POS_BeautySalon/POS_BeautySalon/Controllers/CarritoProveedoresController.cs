using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace POS_BeautySalon.Controllers
{
    public class CarritoProveedoresController : Controller
    {
        private readonly SalonContext _salonContext;

        public CarritoProveedoresController(SalonContext salonContext)
        {
            _salonContext = salonContext;
        }

        public IActionResult Index()
        {
            var carritoProveedor = ObtenerCarritoEstilista();
            var productosEnCarrito = _salonContext.CarritoProvProductos
                .Include(cp => cp.Producto)
                .Where(cp => cp.CarritoProveedorId == carritoProveedor.CarritoProveedorId)
                .ToList();

            // Calcular el total del carrito
            var total = carritoProveedor.CalcularTotal();

            // Pasar el valor Total a la vista
            ViewBag.Total = total;

            return View(productosEnCarrito);
        }

        // Método para agregar productos al carrito
        public async Task<IActionResult> AgregarProdProveedor(int productoId)
        {
            var carritoProveedor = ObtenerCarritoEstilista();

            var carritoProvProducto = new CarritoProvProducto
            {
                CarritoProveedorId = carritoProveedor.CarritoProveedorId,
                ProductoId = productoId,
                Cantidad = 1
            };

            _salonContext.Add(carritoProvProducto);
            await _salonContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Método para editar cantidad de un producto del carrito
        public async Task<IActionResult> EditarCantidad(int carritoProvProductoId, int nuevaCantidad)
        {
            var carritoProvProducto = await _salonContext.CarritoProvProductos.FindAsync(carritoProvProductoId);
            var producto = await _salonContext.Productos.SingleAsync(p => p.ProductoId == carritoProvProducto.ProductoId);

            if (producto.Cantidad < nuevaCantidad)
            {
                TempData["Error"] = "La cantidad digitada excede la cantidad disponible de este producto.";
                return RedirectToAction(nameof(Index));
            }

            carritoProvProducto.Cantidad = nuevaCantidad;

            _salonContext.Update(carritoProvProducto);
            await _salonContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Método para eliminar un producto del carrito
        public async Task<IActionResult> EliminarDelCarrito(int carritoProvProductoId)
        {
            var carritoProvProducto = await _salonContext.CarritoProvProductos.FindAsync(carritoProvProductoId);

            _salonContext.CarritoProvProductos.Remove(carritoProvProducto);
            await _salonContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Método complementario para obtener el carrito del estilista
        private CarritoProveedor ObtenerCarritoEstilista()
        {
            var carritoProveedor = _salonContext.CarritoProveedores.FirstOrDefault();

            if (carritoProveedor == null)
            {
                carritoProveedor = new CarritoProveedor();
                _salonContext.CarritoProveedores.Add(carritoProveedor);
                _salonContext.SaveChanges();
            }
            return carritoProveedor;
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarCompra()
        {
            
            var carritoProveedor = ObtenerCarritoEstilista();

            var productosEnCarrito = _salonContext.CarritoProvProductos
                .Include(cp => cp.Producto)
                .Where(cp => cp.CarritoProveedorId == carritoProveedor.CarritoProveedorId)
                .ToList();

            var total = carritoProveedor.CalcularTotal();

            // Crear una única factura
            var facturaProveedor = new FacturaProveedor
            {
                ProveedorId = carritoProveedor.ProveedorId,
                PrecioTotal = total,
                Fecha = DateTime.Now,
            };

            _salonContext.FacturaProveedores.Add(facturaProveedor);
            await _salonContext.SaveChangesAsync();

            // Crear y guardar los detalles de la factura
            foreach (var item in productosEnCarrito)
            {
                var detalleProveedorFactura = new DetalleProveedorFactura
                {
                    FacturaProveedorId = facturaProveedor.FacturaProveedorId,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Producto.PrecioProveedor
                };

                _salonContext.DetalleProveedorFacturas.Add(detalleProveedorFactura);
            }

            await _salonContext.SaveChangesAsync();

            // Limpiar el carrito
            _salonContext.CarritoProvProductos.RemoveRange(productosEnCarrito);
            await _salonContext.SaveChangesAsync();

            return Json(new { success = true, message = "Compra confirmada con éxito." });
        }
    }
}
