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



        // Acción para mostrar las facturas de proveedores
        public async Task<IActionResult> VerFacturasProveedores()
        {
            var facturasProveedores = await _salonContext.FacturaProveedores
                .Include(f => f.Proveedor)
                .Include(f => f.DetalleProveedorFacturas)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            return PartialView("FacturasProveedores", facturasProveedores);
        }


        //Accion para ver Detalles de la factura 
        public async Task<IActionResult> VerDetallesProveedorFactura(int facturaProveedorId)
        {
            var detallesFactura = await _salonContext.DetalleProveedorFacturas
                .Include(d => d.Producto)
                .ThenInclude(p => p.Marca)
                .Include(d => d.Producto)
                .ThenInclude(p => p.Categoria)
                .Where(d => d.FacturaProveedorId == facturaProveedorId)
                .ToListAsync();

            ViewBag.FacturaProveedorId = facturaProveedorId; // Para pasar el ID a la vista, si es necesario
            return View("DetallesProveedorFactura", detallesFactura);
        }

        //Eliminar Factura de proveedor, Vista FacturasProveedores
        [HttpPost]
        public async Task<IActionResult> EliminarFacturaProveedor(int facturaProveedorId)
        {
            var factura = await _salonContext.FacturaProveedores
                .Include(f => f.DetalleProveedorFacturas)
                .FirstOrDefaultAsync(f => f.FacturaProveedorId == facturaProveedorId);

            if (factura == null)
            {
                return NotFound();
            }

            // Elimina los detalles relacionados
            _salonContext.DetalleProveedorFacturas.RemoveRange(factura.DetalleProveedorFacturas);

            // Elimina la factura
            _salonContext.FacturaProveedores.Remove(factura);

            await _salonContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }




        // Método para editar cantidad de un producto del carrito
        public async Task<IActionResult> EditarCantidad(int carritoProvProductoId, int nuevaCantidad)
        {
            var carritoProvProducto = await _salonContext.CarritoProvProductos.FindAsync(carritoProvProductoId);

           
            carritoProvProducto.Cantidad = nuevaCantidad; // Actualizar la cantidad en el carrito

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

        /*private CarritoProveedor ObtenerCarritoEstilista()
        {
            var carritoProveedor = _salonContext.CarritoProveedores.FirstOrDefault();

            if (carritoProveedor == null)
            {
                carritoProveedor = new CarritoProveedor();
                _salonContext.CarritoProveedores.Add(carritoProveedor);
                _salonContext.SaveChanges();
            }
            return carritoProveedor;
        }*/

        private CarritoProveedor ObtenerCarritoEstilista()
        {
            var carritoProveedor = _salonContext.CarritoProveedores.FirstOrDefault();

            if (carritoProveedor == null)
            {
                // Asigna un ProveedorId válido
                var proveedor = _salonContext.Proveedores.FirstOrDefault();
                if (proveedor == null)
                {
                    throw new InvalidOperationException("No hay proveedores disponibles.");
                }

                carritoProveedor = new CarritoProveedor
                {
                    ProveedorId = proveedor.ProveedorId
                };

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


            //CAMBIO DE ZONA HORARIA DE UTC A MST AGREGADO
            var mountainTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            var fechaMST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, mountainTimeZone);

            // Crear una única factura
            var facturaProveedor = new FacturaProveedor
            {
                ProveedorId = carritoProveedor.ProveedorId,
                PrecioTotal = total,
                //Fecha = DateTime.Now, Asi estaba antes
                Fecha = fechaMST,
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

                // Actualizar el inventario de productos
                var producto = await _salonContext.Productos.FindAsync(item.ProductoId);
                producto.Cantidad += item.Cantidad; // Incrementar la cantidad de productos en el inventario

                // Marcar producto como agotado si la cantidad es cero
                producto.Estado = producto.Cantidad <= 0 ? 0 : 1;

                _salonContext.Update(producto);
            }

            await _salonContext.SaveChangesAsync();

            // Limpiar el carrito
            _salonContext.CarritoProvProductos.RemoveRange(productosEnCarrito);
            await _salonContext.SaveChangesAsync();

            return Json(new { success = true, message = "Compra confirmada con éxito." });
        }
    }
}
