using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace POS_BeautySalon.Controllers
{
    public class ListaDeseosController : Controller
    {
        private readonly SalonContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ListaDeseosController(SalonContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ListaDeseos
        public async Task<IActionResult> Index()
        {
           var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoles = _userManager.GetRolesAsync(_userManager.FindByIdAsync(userId).Result).Result;

            if (userRoles.Contains("Cliente"))
            {
                var salonContext = _context.ListaDeseos.Where(l => l.ClienteId == userId)
                    .Include(l => l.Cliente);
                return View(await salonContext.ToListAsync());
            }
            else
            {
                var salonContext = _context.ListaDeseos.Include(l => l.Cliente);
                return View(await salonContext.ToListAsync());
            }
            /*
           var salonContext = _context.ListaDeseos.Include(l => l.Cliente);
           return View(await salonContext.ToListAsync());*/
        }


        // GET: ListaDeseos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaDeseo = await _context.ListaDeseos
                .Include(l => l.Cliente)
                .Include(lp => lp.ListaDeseoProductos)
                .ThenInclude(p => p.Producto)
                .FirstOrDefaultAsync(m => m.ListaID == id);
            if (listaDeseo == null)
            {
                return NotFound();
            }

            return View(listaDeseo);
        }






        // GET: ListaDeseos/Create
        [Authorize]
        public IActionResult Create()
        {
           // ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre");
            return View();
        }


        // POST: ListaDeseos/Create
       
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ListaID")] ListaDeseo listaDeseo)
        {
            
            if (ModelState.IsValid)
            {
                //Obtener el usuario loggeado
                var identidad = User.Identity as ClaimsIdentity;
                string idUsuarioLogeado = identidad.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                listaDeseo.ClienteId = idUsuarioLogeado;

                _context.Add(listaDeseo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           // ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre", listaDeseo.ClienteId);
            return View(listaDeseo);
        }




        // GET: ListaDeseos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaDeseo = await _context.ListaDeseos.FindAsync(id);
            if (listaDeseo == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre", listaDeseo.ClienteId);
            return View(listaDeseo);
        }

        // POST: ListaDeseos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ListaID,ClienteId")] ListaDeseo listaDeseo)
        {
            if (id != listaDeseo.ListaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listaDeseo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListaDeseoExists(listaDeseo.ListaID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre", listaDeseo.ClienteId);
            return View(listaDeseo);
        }

        // GET: ListaDeseos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaDeseo = await _context.ListaDeseos
                .Include(l => l.Cliente)
                .FirstOrDefaultAsync(m => m.ListaID == id);
            if (listaDeseo == null)
            {
                return NotFound();
            }

            return View(listaDeseo);
        }

        // POST: ListaDeseos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listaDeseo = await _context.ListaDeseos.FindAsync(id);
            if (listaDeseo != null)
            {
                _context.ListaDeseos.Remove(listaDeseo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListaDeseoExists(int id)
        {
            return _context.ListaDeseos.Any(e => e.ListaID == id);
        }
    }
}
