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
    public class CitasController : Controller
    {
        private readonly SalonContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CitasController(SalonContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Citas
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoles = _userManager.GetRolesAsync(_userManager.FindByIdAsync(userId).Result).Result;

            if (userRoles.Contains("Cliente"))
            {
                var salonContext = _context.Citas.Where(c => c.ClienteId == userId)
                    .Include(c => c.Cliente).Include(c => c.Servicio);
                return View(await salonContext.ToListAsync());
            }
            else
            {
                var salonContext = _context.Citas.Include(c => c.Cliente).Include(c => c.Servicio);
                return View(await salonContext.ToListAsync());
            }

            
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .FirstOrDefaultAsync(m => m.CitaId == id);
            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // GET: Citas/Create
        public async Task<IActionResult> CreateAsync()
        {
            /*
            var usuariosCliente = await _userManager.GetUsersInRoleAsync("Cliente");

            var clientesSelectList = usuariosCliente.Select(u => new SelectListItem
            {
                Text = u.Nombre,
                Value = u.Id
            }).ToList();
            
            //ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre");
            ViewData["ClienteId"] = clientesSelectList;
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre");
            return View();*/
            if (User.IsInRole("Administrador"))
            {
                var usuariosCliente = await _userManager.GetUsersInRoleAsync("Cliente");

                var clientesSelectList = usuariosCliente.Select(u => new SelectListItem
                {
                    Text = u.Nombre,
                    Value = u.Id
                }).ToList();

                ViewData["ClienteId"] = clientesSelectList;
            }
            else
            {
                var identidad = User.Identity as ClaimsIdentity;
                string idUsuarioLogeado = identidad.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                ViewData["ClienteId"] = new List<SelectListItem>
        {
            new SelectListItem
            {
                Text = User.Identity.Name,
                Value = idUsuarioLogeado
            }
        };
            }

            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre");
            return View();
        }

        // POST: Citas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CitaId,Estado,ClienteId,ServicioId,Fecha,Hora,Notas")] Cita cita)
        {
            
            var identidad = User.Identity as ClaimsIdentity;
            string idUsuarioLogeado = identidad.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            
            if (!User.IsInRole("Administrador"))
            {
                cita.ClienteId = idUsuarioLogeado;
            }

            cita.Estado = 1;

            if (ModelState.IsValid)
            {
                

                //validacion de cita antes del dia actual
                if (cita.Fecha < DateTime.Today)
                {
                    ModelState.AddModelError("", "No se pueden programar citas en fechas anteriores a la fecha actual.");
                    ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre");
                    ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre");
                    return View(cita);
                }
                // Validar que la cita esté en el rango permitido (lunes a sábado, 7 am a 7 pm)
                if (cita.Fecha.DayOfWeek == DayOfWeek.Sunday ||
                    cita.Hora.TimeOfDay < new TimeSpan(7, 0, 0) ||
                    cita.Hora.TimeOfDay >= new TimeSpan(19, 0, 0))
                {
                    ModelState.AddModelError("", "Las citas solo se pueden programar de lunes a sábado, entre las 7 am y las 7 pm.");
                    ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre");
                    ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre");
                    return View(cita);
                }

                // Duración de la cita
                TimeSpan citaDuracion = new TimeSpan(2, 0, 0);
                DateTime citaFin = cita.Hora.Add(citaDuracion);

                // Obtener todas las citas del día
                var citasDelDia = _context.Citas
                    .Where(c => c.Fecha == cita.Fecha)
                    .ToList();

                // Validar que no haya otra cita en el intervalo de 2 horas antes y después de la nueva cita
                var conflictingCitas = citasDelDia
                    .Where(c => 
                        (c.Hora < citaFin && c.Hora.Add(citaDuracion) > cita.Hora) ||  // Validar que las citas existentes no se solapen con la nueva cita
                        (c.Hora < cita.Hora && c.Hora.Add(citaDuracion) > cita.Hora)) // Validar que las citas existentes no terminen durante la nueva cita)
                    .ToList();

                if (conflictingCitas.Any())
                {
                    ModelState.AddModelError("", "Ya existe una cita programada en el mismo horario o dentro del intervalo de 2 horas.");
                    ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre");
                    ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre");
                    return View(cita);
                }

                _context.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));


            }
            ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre", cita.ClienteId);
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre", cita.ServicioId);
            return View(cita);
        }

        // GET: Citas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre", cita.ClienteId);
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre", cita.ServicioId);
            return View(cita);
        }

        // POST: Citas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CitaId,Estado,ClienteId,ServicioId,Fecha,Hora,Notas")] Cita cita)
        {
            if (id != cita.CitaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitaExists(cita.CitaId))
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
            ViewData["ClienteId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Nombre", cita.ClienteId);
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre", cita.ServicioId);
            return View(cita);
        }

        // GET: Citas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .FirstOrDefaultAsync(m => m.CitaId == id);
            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // POST: Citas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null)
            {
                _context.Citas.Remove(cita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.CitaId == id);
        }
    }
}
