using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;

namespace POS_BeautySalon.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly SalonContext _context;

        public ProveedoresController(SalonContext context)
        {
            _context = context;
        }

        // GET: Proveedores
        public async Task<IActionResult> Index()
        {
            foreach (var proveedor in _context.Proveedores)
            {
                proveedor.Calificacion = (int?)CalcularCalificacionPromedio(proveedor.Puntualidad, proveedor.Calidad);
            }

            return View(_context.Proveedores);

            //return View(await _context.Proveedores.ToListAsync());
        }

        // Método para calcular la calificación promedio
        private double CalcularCalificacionPromedio(int? puntualidad, int? calidad)
        {
            if (puntualidad.HasValue && calidad.HasValue)
            {
                return (puntualidad.Value + calidad.Value) / 2.0;
            }
            return 0;
        }

        // Método para verificar si el proveedor necesita calificación
        private bool NecesitaCalificacion(Proveedor proveedor)
        {
            return !proveedor.Puntualidad.HasValue || !proveedor.Calidad.HasValue;
        }

        public IActionResult CalificarPendientes()
        {
            var proveedoresPendientes = _context.Proveedores.Where(NecesitaCalificacion).ToList();

            if (proveedoresPendientes.Count == 0)
            {
                ViewBag.Message = "No hay proveedores pendientes de calificación.";
            }
            else
            {
                ViewBag.Message = "Proveedores pendientes de calificación:";
            }

            return View(proveedoresPendientes);
        }

        // Acción para mostrar el formulario de calificación de proveedor
        public IActionResult Calificar(int proveedorId)
        {
            var proveedor = _context.Proveedores.FirstOrDefault(p => p.ProveedorId == proveedorId);

            if (proveedor == null)
            {
                return NotFound("Proveedor no encontrado.");
            }

            return View(proveedor);
        }

        // Acción para calificar un proveedor
        [HttpPost]
        public IActionResult Calificar(int proveedorId, int puntualidad, int calidad)
        {
            var proveedor = _context.Proveedores.FirstOrDefault(p => p.ProveedorId == proveedorId);

            if (proveedor == null)
            {
                return NotFound("Proveedor no encontrado.");
            }

            try
            {
                if (puntualidad < 1 || puntualidad > 5 || calidad < 1 || calidad > 5)
                {
                    throw new ArgumentException("Las calificaciones deben estar entre 1 y 5.");
                }

                proveedor.Puntualidad = puntualidad;
                proveedor.Calidad = calidad;
                proveedor.Calificacion = (int?)CalcularCalificacionPromedio(proveedor.Puntualidad, proveedor.Calidad);

                _context.Update(proveedor);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Calificar", proveedor);
            }
        }


        // GET: Proveedores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(m => m.ProveedorId == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // GET: Proveedores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proveedores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProveedorId,Nombre,Email,Telefono,Puntualidad,Calidad,Calificacion")] Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(proveedor);
        }

        // GET: Proveedores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            return View(proveedor);
        }

        // POST: Proveedores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProveedorId,Nombre,Email,Telefono,Puntualidad,Calidad,Calificacion")] Proveedor proveedor)
        {
            if (id != proveedor.ProveedorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedorExists(proveedor.ProveedorId))
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
            return View(proveedor);
        }

        // GET: Proveedores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(m => m.ProveedorId == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor != null)
            {
                _context.Proveedores.Remove(proveedor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedores.Any(e => e.ProveedorId == id);
        }
    }
}
