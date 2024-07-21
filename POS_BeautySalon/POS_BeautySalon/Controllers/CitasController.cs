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
using DAL.Migrations.Salon;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace POS_BeautySalon.Controllers
{
    public class CitasController : Controller
    {
        private readonly SalonContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public CitasController(SalonContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
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
        public async Task<IActionResult> CreateAsync(int servicioId)
        {
            
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

            var cita = new Cita
            {
                ServicioId = servicioId
               
            };

           
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre");
            return View(cita);
        }

        // POST: Citas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CitaId,Estado,ClienteId,ServicioId,Fecha,Hora,Notas")] Cita cita)
        {
            /*
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
                    ViewData["ClienteId"] = new List<SelectListItem>
            {
            new SelectListItem
            {
                Text = User.Identity.Name,
                Value = idUsuarioLogeado
            }
            };
                    ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre");
                    return View(cita);
                }
                // Validar que la cita esté en el rango permitido (lunes a sábado, 7 am a 7 pm)
                if (cita.Fecha.DayOfWeek == DayOfWeek.Sunday ||
                    cita.Hora.TimeOfDay < new TimeSpan(7, 0, 0) ||
                    cita.Hora.TimeOfDay >= new TimeSpan(19, 0, 0))
                {
                    ModelState.AddModelError("", "Las citas solo se pueden programar de lunes a sábado, entre las 7 am y las 7 pm.");
                    ViewData["ClienteId"] = new List<SelectListItem>
            {
            new SelectListItem
            {
                Text = User.Identity.Name,
                Value = idUsuarioLogeado
            }
            };
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
                    ViewData["ClienteId"] = new List<SelectListItem>
            {
            new SelectListItem
            {
                Text = User.Identity.Name,
                Value = idUsuarioLogeado
            }
            };
                    ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre");
                    return View(cita);
                }

                _context.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));


            }
            ViewData["ClienteId"] = new List<SelectListItem>
            {
            new SelectListItem
            {
                Text = User.Identity.Name,
                Value = idUsuarioLogeado
            }
            };
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre", cita.ServicioId);
            return View(cita);*/
            var identidad = User.Identity as ClaimsIdentity;
            string idUsuarioLogeado = identidad.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (!User.IsInRole("Administrador"))
            {
                cita.ClienteId = idUsuarioLogeado;
            }

            cita.Estado = 1;

            if (ModelState.IsValid)
            {
                if (cita.Fecha < DateTime.Today)
                {
                    ModelState.AddModelError("", "No se pueden programar citas en fechas anteriores a la fecha actual.");
                    return View(cita);
                }

                if (cita.Fecha.DayOfWeek == DayOfWeek.Sunday ||
                    cita.Hora.TimeOfDay < new TimeSpan(7, 0, 0) ||
                    cita.Hora.TimeOfDay >= new TimeSpan(19, 0, 0))
                {
                    ModelState.AddModelError("", "Las citas solo se pueden programar de lunes a sábado, entre las 7 am y las 7 pm.");
                    return View(cita);
                }

                TimeSpan citaDuracion = new TimeSpan(2, 0, 0);
                DateTime citaFin = cita.Hora.Add(citaDuracion);
                var citasDelDia = _context.Citas.Where(c => c.Fecha == cita.Fecha).ToList();
                var conflictingCitas = citasDelDia
                    .Where(c => (c.Hora < citaFin && c.Hora.Add(citaDuracion) > cita.Hora) || (c.Hora < cita.Hora && c.Hora.Add(citaDuracion) > cita.Hora))
                    .ToList();

                if (conflictingCitas.Any())
                {
                    ModelState.AddModelError("", "Ya existe una cita programada en el mismo horario o dentro del intervalo de 2 horas.");
                    return View(cita);
                }

                _context.Add(cita);
                await _context.SaveChangesAsync();

                // Enviar correo de confirmación al cliente y al estilista
                var cliente = await _userManager.FindByIdAsync(cita.ClienteId);
                var servicio = await _context.Servicios.FindAsync(cita.ServicioId);
                var estilistaEmail = _configuration["Estilista:Email"];

                var emailContent = $@"
                <p>Hola {cliente.Nombre},</p>
                <p>Código de reserva: {cita.CitaId}</p>
                <p>Tu cita para el servicio {servicio.Nombre} ha sido agendada para el {cita.Fecha:dd/MM/yyyy} a las {cita.Hora:hh\\:mm}.</p>
                <p>Notas: {cita.Notas}</p>
                <p>Gracias por elegirnos!</p>";

                var estilistaEmailContent = $@"
                <p>Hola Viviana,</p>
                <p>Código de reserva: {cita.CitaId}</p>
                <p>Se ha agendado una nueva cita para el servicio {servicio.Nombre} con {cliente.Nombre} el {cita.Fecha:dd/MM/yyyy} a las {cita.Hora:hh\\:mm}.</p>
                <p>Notas: {cita.Notas}</p>";

                await _emailSender.SendEmailAsync(cliente.Email, "Confirmación de Cita", emailContent);
                await _emailSender.SendEmailAsync(estilistaEmail, "Nueva Cita Agendada", estilistaEmailContent);

                return RedirectToAction(nameof(Index));
            }

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
