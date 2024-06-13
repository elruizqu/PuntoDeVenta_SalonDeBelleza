using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;

using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
//using YourNamespace.Models;

namespace POS_BeautySalon.Controllers
{
    public class AdminController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthContext _AuthContext;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AdminController(UserManager<ApplicationUser> userManager, AuthContext AuthContext, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _AuthContext = AuthContext;
        }


        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);

        }


        // CREATE
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
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

        // edit (POST)
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
                existingUser.UserName = user.UserName;
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
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        //DELETE POST
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            // Handle errors here
            return View(user);
        }

        // BLOCK
        public async Task<IActionResult> Block(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Implementa el bloqueo del usuario según tu lógica
            // Por ejemplo, puedes establecer una bandera en una propiedad personalizada
            // o deshabilitar la cuenta del usuario.

            // user.LockoutEnd = DateTime.Now.AddYears(100); // Ejemplo para bloquear al usuario hasta una fecha específica.
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            // Handle errors here
            return View(user);
        }
    }


   
}
