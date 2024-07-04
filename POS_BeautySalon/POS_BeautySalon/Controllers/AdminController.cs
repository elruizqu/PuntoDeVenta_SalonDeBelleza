using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;

using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace POS_BeautySalon.Controllers
{
    public class AdminController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthContext _AuthContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly SalonContext _salonContext;

        public AdminController(UserManager<ApplicationUser> userManager, AuthContext AuthContext, SignInManager<ApplicationUser> signInManager, SalonContext salonContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _AuthContext = AuthContext;
            _salonContext = salonContext;
              
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);

        }


        // CREATE
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(user);
        }




        // EDIT (GET)
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Edit (POST)
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByIdAsync(user.Id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.Email = user.Email;
                //existingUser.UserName = user.UserName;
                existingUser.Nombre = user.Nombre;
                existingUser.Apellido = user.Apellido;
                existingUser.Estado = user.Estado;

                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                // Manejar errores aquí
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(user);
        }







        // DELETE 
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // DELETE (POST)
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Eliminar la lista de deseos del usuario
            var listaDeseo = await _salonContext.ListaDeseos
                .Include(ld => ld.ListaDeseoProductos)
                .FirstOrDefaultAsync(ld => ld.ClienteId == id);

            if (listaDeseo != null)
            {
                // Eliminar los productos asociados a la lista de deseos
                _salonContext.ListaDeseoProductos.RemoveRange(listaDeseo.ListaDeseoProductos);

                // Eliminar la lista de deseos del usuario
                _salonContext.ListaDeseos.Remove(listaDeseo);
            }
            


            // Eliminar las citas del usuario
            var citas = await _salonContext.Citas.Where(c => c.ClienteId == id).ToListAsync();
            if (citas != null && citas.Any())
            {
                _salonContext.Citas.RemoveRange(citas);
            }

            // Eliminar el carrito de compras del usuario
            var carrito = await _salonContext.Carritos
                .Include(c => c.CarritoProductos)
                .FirstOrDefaultAsync(c => c.ClienteId == id);

            if (carrito != null)
            {
                // Eliminar los productos asociados al carrito de compras
                _salonContext.CarritoProductos.RemoveRange(carrito.CarritoProductos);

                // Eliminar el carrito de compras del usuario
                _salonContext.Carritos.Remove(carrito);
            }



            // Guardar cambios

            await _salonContext.SaveChangesAsync();


            //Eliminar el usuario
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            // Manejar errores aquí
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }



        // Locked Account GET
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Locked(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // Locked Account POST
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockedConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Fecha específica para bloquear
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            user.Estado = 0;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

           
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }



        // Unlock GET
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Unlock(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // Unlock POST
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Para quitar la fecha de bloqueo y cambiar el estado a activo 1
            user.LockoutEnd = null;
            user.Estado = 1;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }

    }
}
