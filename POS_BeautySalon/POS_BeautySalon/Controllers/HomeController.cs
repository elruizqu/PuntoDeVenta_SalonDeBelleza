using DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_BeautySalon.Models;
using System.Diagnostics;

namespace POS_BeautySalon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SalonContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SalonContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Estilista"))
            {
                return RedirectToAction("Index", "Citas");
                
            }
            else
            {
                var comentarios = await _context.Comentarios.Include(c => c.Cliente).ToListAsync();
                return View(comentarios);

            }
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([Bind("Detalle")] Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                var clienteId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (clienteId == null)
                {
                    return BadRequest("Cliente no identificado.");
                }

                comentario.ClienteId = clienteId;

                _context.Add(comentario);
                await _context.SaveChangesAsync();

                var comentarios = await _context.Comentarios.Include(c => c.Cliente).ToListAsync();

                return PartialView("_CommentsPartial", comentarios);
            }

            return BadRequest("Hubo un error al procesar tu solicitud.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
