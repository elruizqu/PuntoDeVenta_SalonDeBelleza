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
            var producto = await _salonContext.Productos.FindAsync(productoId);

            if(producto.Cantidad < 1)
            {
                TempData["Error"] = "Este producto está agotado";
                return RedirectToAction(nameof(Index));
            }

            if(producto.Estado == 0)
            {
                TempData["Error"] = "Este producto no está disponible hasta dentro de 2 semanas";
                return RedirectToAction(nameof(Index));
            }

            var carritoProducto = new CarritoProducto
            {
                CarritoId = carrito.CarritoId,
                ProductoId = productoId,
                Cantidad = 1
            };

            producto.Cantidad -= 1;

            // Verificar si la cantidad del producto es igual o menor a 5
            if (producto.Cantidad <= 5)
            {
                producto.Alerta = $"Alerta! Quedan pocas unidades de {producto.Nombre}";
            }
            else if(producto.Cantidad > 5)
            {
                producto.Alerta = null;
            }

            // Actualizar el estado del producto si se agota
            producto.Estado = producto.Cantidad == 0 ? 0 : 1;

            _salonContext.Add(carritoProducto);
            _salonContext.Update(producto);
            await _salonContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Método para editar cantidad de un producto del carrito
        public async Task<IActionResult> EditarCantidad(int carritoProductoId, int nuevaCantidad)
        {
            var carritoProducto = await _salonContext.CarritoProductos.FindAsync(carritoProductoId);
            var producto = await _salonContext.Productos.SingleAsync(p => p.ProductoId ==
            carritoProducto.ProductoId);

            if (producto.Cantidad < nuevaCantidad - carritoProducto.Cantidad)
            {
                TempData["Error"] = "La cantidad digitada excede la cantidad disponible de este producto.";
                return RedirectToAction(nameof(Index));
            }

            producto.Cantidad -= nuevaCantidad - carritoProducto.Cantidad;

            // Verificar si la cantidad del producto es igual o menor a 5
            if (producto.Cantidad <= 5)
            {
                producto.Alerta = $"Alerta! Quedan pocas unidades de {producto.Nombre}";
            }
            else if(producto.Cantidad > 5)
            {
                producto.Alerta = null;
            }

            // Actualizar el estado del producto si se agota
            producto.Estado = producto.Cantidad == 0 ? 0 : 1;

            carritoProducto.Cantidad = nuevaCantidad;

            _salonContext.Update(carritoProducto);
            _salonContext.Update(producto);
            await _salonContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Método para eliminar un producto del carrito
        public async Task<IActionResult> EliminarDelCarrito(int carritoProductoId)
        {
            var carritoProducto = await _salonContext.CarritoProductos.FindAsync(carritoProductoId);
            var producto = await _salonContext.Productos.FindAsync(carritoProducto.ProductoId);

            // Incrementar la cantidad del producto en el inventario tras eliminar
            producto.Cantidad += carritoProducto.Cantidad;

            // Verificar si la cantidad del producto es igual o menor a 5
            if (producto.Cantidad <= 5)
            {
                TempData["Alerta"] = $"Alerta! Quedan pocas unidades de {producto.Nombre}";
            }

            _salonContext.CarritoProductos.Remove(carritoProducto);
            _salonContext.Update(producto);
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

            // Pasar los detalles de la factura a la vista
            ViewBag.ProductosEnCarrito = productosEnCarrito;
            ViewBag.Total = total;
            ViewBag.Consecutivo = consecutivo;

            return View("ImprimirFactura");
        }

       


        private int GenerarConsecutivo()
        {
            // Obtener el último ID de factura en la base de datos
            var ultimoIdFactura = _salonContext.Facturas
                                  .OrderByDescending(f => f.FacturaId)
                                  .Select(f => f.FacturaId)
                                  .FirstOrDefault();

            // Sumarle 1 para generar el nuevo consecutivo
            return ultimoIdFactura + 1;
        }



        //Compra de productos en carrito
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


            //CAMBIO DE ZONA HORARIA DE UTC A MST AGREGADO
            var mountainTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            var fechaMST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, mountainTimeZone);

            // Crear una única factura
            var factura = new Factura
            {
                ClienteId = carrito.ClienteId,
                PrecioTotal = total,
                //Fecha = DateTime.Now, ASI ESTABA ANTES
                Fecha = fechaMST,
            };

            _salonContext.Facturas.Add(factura);
            await _salonContext.SaveChangesAsync();

            // Crear y guardar los detalles de la factura
            foreach (var item in productosEnCarrito)
            {
                var detalleFactura = new DetalleFactura
                {
                    FacturaId = factura.FacturaId,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Producto.Precio
                };

                _salonContext.DetalleFacturas.Add(detalleFactura);
            }

            await _salonContext.SaveChangesAsync();


            // Limpiar el carrito
            _salonContext.CarritoProductos.RemoveRange(productosEnCarrito);
            await _salonContext.SaveChangesAsync();

           

            // Actualizar los puntos del programa lealtad al cliente
            var cliente = await _userManager.FindByIdAsync(carrito.ClienteId);
            if (cliente != null)
            {
                // Contar el número de facturas del cliente para saber si ya puede hacer uso del programa lealtad
                var numFacturas = _salonContext.Facturas.Count(f => f.ClienteId == carrito.ClienteId);

                if (numFacturas % 2 == 0) //Cuando tenga dos compras realizadas
                {
                    cliente.PuntosProgramaLealtad += 10; // Sumar 10 puntos
                    var updateResult = await _userManager.UpdateAsync(cliente);
                    if (!updateResult.Succeeded)
                    {
                        return Json(new { success = false, message = "Error al actualizar los puntos del cliente." });
                    }
                    await Task.Delay(5000); // Esperar 5 segundos
                    return Json(new { success = true, message = "Compra confirmada con éxito. Has acumulado 10 puntos y ya puedes hacer uso del programa de lealtad." });
                }
            }

            return Json(new { success = true, message = "Compra confirmada con éxito." });
        }

    }

}

