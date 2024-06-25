using DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace POS_BeautySalon.Controllers
{
    public class CarritoController : Controller
    {
        private readonly SalonContext _salonContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarritoController(SalonContext salonContext)
        {
            _salonContext = salonContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Método para agregar productos al carrito
        public async Task<IActionResult> AgregarAlCarrito(int productoId, int cantidad)
        {
            var carrito = ObtenerCarritoDelUsuarioActual();

            var carritoProducto = new CarritoProducto
            {
                CarritoId = carrito.CarritoId,
                ProductoId = productoId,
                Cantidad = cantidad
            };

            _salonContext.Add(carritoProducto);
            await _salonContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Método para editar cantidad de un producto del carrito
        public async Task<IActionResult> EditarCantidad(int carritoProductoId, int nuevaCantidad)
        {
            var carritoProducto = await _salonContext.CarritoProductos.FindAsync(carritoProductoId);
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
            return _salonContext.Carritos.FirstOrDefault(c => c.ClienteId == clienteId);
        }
    }
}
