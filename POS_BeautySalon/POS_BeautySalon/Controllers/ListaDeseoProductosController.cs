using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace POS_BeautySalon.Controllers
{
    public class ListaDeseoProductosController : Controller
    {
        private readonly SalonContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ListaDeseoProductosController(SalonContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ListaDeseoProductos
        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoles = _userManager.GetRolesAsync(_userManager.FindByIdAsync(userId).Result).Result;

            if (userRoles.Contains("Cliente"))
            {
                var salonContext = _context.ListaDeseoProductos.Where(l => l.ListaDeseo.ClienteId == userId)
                .Include(l => l.ListaDeseo)
                .Include(l => l.Producto);
                return View(await salonContext.ToListAsync());
            }
            else
            {
                var salonContext = _context.ListaDeseoProductos
                .Include(l => l.ListaDeseo)
                .Include(l => l.Producto);
                return View(await salonContext.ToListAsync());
            }
                
        }

        // GET: ListaDeseoProductos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaDeseoProducto = await _context.ListaDeseoProductos
                .Include(l => l.ListaDeseo)
                .Include(l => l.Producto)
                .FirstOrDefaultAsync(m => m.ListaDeseoProductoId == id);
            if (listaDeseoProducto == null)
            {
                return NotFound();
            }

            return View(listaDeseoProducto);
        }

        // GET: ListaDeseoProductos/Create
        public IActionResult Create(int productoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var listaDeseo = _context.ListaDeseos.FirstOrDefault(ld => ld.ClienteId == userId);

            if (listaDeseo == null)
            {
                // Si el usuario no tiene una lista de deseos, redirigir o manejar de alguna manera
                return RedirectToAction(nameof(Index));
            }

            var listaDeseoProducto = new ListaDeseoProducto
            {
                ProductoId = productoId,
                ListaID = listaDeseo.ListaID
            };

            //ViewData["ListaID"] = new SelectList(_context.ListaDeseos, "ListaID", "ListaID");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre");
            return View(listaDeseoProducto);
        }

        // POST: ListaDeseoProductos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ListaDeseoProductoId,ListaID,ProductoId")] ListaDeseoProducto listaDeseoProducto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(listaDeseoProducto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ListaID"] = new SelectList(_context.ListaDeseos, "ListaID", "ListaID", listaDeseoProducto.ListaID);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", listaDeseoProducto.ProductoId);
            //return View(listaDeseoProducto);
            return View(listaDeseoProducto);
        }

        // GET: ListaDeseoProductos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaDeseoProducto = await _context.ListaDeseoProductos.FindAsync(id);
            if (listaDeseoProducto == null)
            {
                return NotFound();
            }
            ViewData["ListaID"] = new SelectList(_context.ListaDeseos, "ListaID", "ListaID", listaDeseoProducto.ListaID);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", listaDeseoProducto.ProductoId);
            return View(listaDeseoProducto);
        }

        // POST: ListaDeseoProductos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ListaDeseoProductoId,ListaID,ProductoId")] ListaDeseoProducto listaDeseoProducto)
        {
            if (id != listaDeseoProducto.ListaDeseoProductoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listaDeseoProducto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListaDeseoProductoExists(listaDeseoProducto.ListaDeseoProductoId))
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
            ViewData["ListaID"] = new SelectList(_context.ListaDeseos, "ListaID", "ListaID", listaDeseoProducto.ListaID);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", listaDeseoProducto.ProductoId);
            return View(listaDeseoProducto);
        }

        // GET: ListaDeseoProductos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaDeseoProducto = await _context.ListaDeseoProductos
                .Include(l => l.ListaDeseo)
                .Include(l => l.Producto)
                .FirstOrDefaultAsync(m => m.ListaDeseoProductoId == id);
            if (listaDeseoProducto == null)
            {
                return NotFound();
            }

            return View(listaDeseoProducto);
        }

        // POST: ListaDeseoProductos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listaDeseoProducto = await _context.ListaDeseoProductos.FindAsync(id);
            if (listaDeseoProducto != null)
            {
                _context.ListaDeseoProductos.Remove(listaDeseoProducto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListaDeseoProductoExists(int id)
        {
            return _context.ListaDeseoProductos.Any(e => e.ListaDeseoProductoId == id);
        }
    }
}
