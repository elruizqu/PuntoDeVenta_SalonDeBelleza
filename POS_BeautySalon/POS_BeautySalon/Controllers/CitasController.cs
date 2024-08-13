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
                var salonContext = _context.Citas
                    .Where(c => c.ClienteId == userId)
                    .Include(c => c.Cliente)
                    .Include(c => c.Servicio).ToList(); ;
                //return View(await salonContext.ToListAsync());

                var citasPasadas = salonContext.Where(c => c.Fecha < DateTime.Today).ToList();
                var citasFuturas = salonContext.Where(c => c.Fecha > DateTime.Today).ToList();
                var citasDelDia = salonContext.Where(c => c.Fecha == DateTime.Today).ToList();

                ViewData["CitasPasadas"] = citasPasadas;
                ViewData["CitasFuturas"] = citasFuturas;
                ViewData["CitasDelDia"] = citasDelDia;
            }
            else
            {
                var salonContext = _context.Citas
                    .Include(c => c.Cliente)
                    .Include(c => c.Servicio).ToList(); ;
                //return View(await salonContext.ToListAsync());

                var citasPasadas = salonContext.Where(c => c.Fecha < DateTime.Today).ToList();
                var citasFuturas = salonContext.Where(c => c.Fecha > DateTime.Today).ToList();
                var citasDelDia = salonContext.Where(c => c.Fecha == DateTime.Today).ToList();

                ViewData["CitasPasadas"] = citasPasadas;
                ViewData["CitasFuturas"] = citasFuturas;
                ViewData["CitasDelDia"] = citasDelDia;
            }

            return View();

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (User.IsInRole("Administrador") || User.IsInRole("Estilista"))
            {
                // Mostrar todos los clientes
                var usuariosCliente = await _userManager.GetUsersInRoleAsync("Cliente");
                ViewBag.ClienteId = new SelectList(
            usuariosCliente.Select(u => new
            {
                Id = u.Id,
                NombreCompleto = $"{u.Nombre} {u.Apellido}"
            }).ToList(),
            "Id",
            "NombreCompleto"
        );
            }
            else if (User.IsInRole("Cliente"))
            {
                // Mostrar solo el cliente actualmente logueado
                var cliente = await _userManager.FindByIdAsync(userId);
                ViewBag.ClienteId = new SelectList(
            new List<ApplicationUser> { cliente }.Select(u => new
            {
                Id = u.Id,
                NombreCompleto = $"{u.Nombre} {u.Apellido}"
            }).ToList(),
            "Id",
            "NombreCompleto"
        );
            }

            // Mostrar todos los servicios
            ViewBag.ServicioId = new SelectList(await _context.Servicios.ToListAsync(), "ServicioId", "Nombre");

            var cita = new Cita
            {
                ServicioId = servicioId
            };

            return View(cita);
        }


        // POST: Citas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("CitaId,Estado,ClienteId,ServicioId,Fecha,Hora,Notas,FacturaId")] Cita cita)
        {
            var identidad = User.Identity as ClaimsIdentity;
            string idUsuarioLogeado = identidad.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!User.IsInRole("Administrador") && !User.IsInRole("Estilista"))
            {
                cita.ClienteId = idUsuarioLogeado;
            }

            cita.Estado = 1;

            if (ModelState.IsValid)
            {
                if (cita.Fecha < DateTime.Today)
                {
                    ModelState.AddModelError("", "No se pueden programar citas en fechas anteriores a la fecha actual.");
                }
                else if (cita.Fecha.DayOfWeek == DayOfWeek.Sunday ||
                         cita.Hora.TimeOfDay < new TimeSpan(7, 0, 0) ||
                         cita.Hora.TimeOfDay >= new TimeSpan(19, 0, 0))
                {
                    ModelState.AddModelError("", "Las citas solo se pueden programar de lunes a sábado, entre las 7 am y las 7 pm.");
                }
                else
                {
                    TimeSpan citaDuracion = new TimeSpan(2, 0, 0);
                    DateTime citaFin = cita.Hora.Add(citaDuracion);
                    var citasDelDia = _context.Citas.Where(c => c.Fecha == cita.Fecha).ToList();
                    var conflictingCitas = citasDelDia
                        .Where(c => (c.Hora < citaFin && c.Hora.Add(citaDuracion) > cita.Hora) || (c.Hora < cita.Hora && c.Hora.Add(citaDuracion) > cita.Hora))
                        .ToList();

                    if (conflictingCitas.Any())
                    {
                        ModelState.AddModelError("", "Ya existe una cita programada en el mismo horario o dentro del intervalo de 2 horas.");
                    }
                    else
                    {
                        _context.Add(cita);
                        await _context.SaveChangesAsync();

                        // Crear la factura
                        var servicio = await _context.Servicios.FindAsync(cita.ServicioId);
                        var factura = new Factura
                        {
                            ClienteId = cita.ClienteId,
                            PrecioTotal = servicio.Precio,
                            Fecha = DateTime.Now
                        };

                        _context.Add(factura);
                        await _context.SaveChangesAsync();

                        // Crear el detalle de la factura
                        var detalleFactura = new DetalleFactura
                        {
                            FacturaId = factura.FacturaId,
                            ServicioId = cita.ServicioId,
                            PrecioUnitario = servicio.Precio
                        };

                        _context.Add(detalleFactura);
                        await _context.SaveChangesAsync();

                        // Vincular la cita con la factura
                        cita.FacturaId = factura.FacturaId;
                        _context.Update(cita);
                        await _context.SaveChangesAsync();

                        // Enviar correo de confirmación
                        var cliente = await _userManager.FindByIdAsync(cita.ClienteId);
                        var estilistaEmail = _configuration["Estilista:Email"];

                        if (cliente != null && !string.IsNullOrWhiteSpace(cliente.Email))
                        {
                            var emailContent = $@"
            <p>Hola {cliente.Nombre},</p>
            <p>Código de reserva: {cita.CitaId}</p>
            <p>Tu cita para el servicio {servicio.Nombre} ha sido agendada para el {cita.Fecha:dd/MM/yyyy} a las {cita.Hora:hh\:mm}.</p>
            <p>Notas: {cita.Notas}</p>
            <p>Gracias por elegirnos!</p>";
                            try
                            {
                                await _emailSender.SendEmailAsync(cliente.Email, "Confirmación de Cita", emailContent);
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "Hubo un problema al enviar el correo de confirmación al cliente. Por favor, inténtalo de nuevo más tarde.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "No se pudo enviar el correo de confirmación porque la dirección de correo del cliente no es válida.");
                        }

                        if (!string.IsNullOrWhiteSpace(estilistaEmail))
                        {
                            var estilistaEmailContent = $@"
            <p>Hola Viviana,</p>
            <p>Código de reserva: {cita.CitaId}</p>
            <p>Se ha agendado una nueva cita para el servicio {servicio.Nombre} con {cliente.Nombre} el {cita.Fecha:dd/MM/yyyy} a las {cita.Hora:hh\:mm}.</p>
            <p>Notas: {cita.Notas}</p>";
                            try
                            {
                                await _emailSender.SendEmailAsync(estilistaEmail, "Nueva Cita Agendada", estilistaEmailContent);
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "Hubo un problema al enviar el correo de notificación al estilista. Por favor, inténtalo de nuevo más tarde.");
                            }
                        }
                        return RedirectToAction(nameof(Index));
                    }
                }

                // Recargar datos de Cliente y Servicio en caso de error de validación
                if (User.IsInRole("Administrador") || User.IsInRole("Estilista"))
                {
                    var usuariosCliente = await _userManager.GetUsersInRoleAsync("Cliente");
                    ViewBag.ClienteId = new SelectList(
            usuariosCliente.Select(u => new
            {
                Id = u.Id,
                NombreCompleto = $"{u.Nombre} {u.Apellido}"
            }).ToList(),
            "Id",
            "NombreCompleto"
        );

                }
                else
                {
                    var user = await _userManager.FindByIdAsync(idUsuarioLogeado);
                    ViewBag.ClienteId = new SelectList(
            new List<ApplicationUser> { user }.Select(u => new
            {
                Id = u.Id,
                NombreCompleto = $"{u.Nombre} {u.Apellido}"
            }).ToList(),
            "Id",
            "NombreCompleto"
        );
                }
                ViewBag.ServicioId = new SelectList(await _context.Servicios.ToListAsync(), "ServicioId", "Nombre", cita.ServicioId);
                
            }
            return View(cita);
        }


        /* [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> Create([Bind("CitaId,Estado,ClienteId,ServicioId,Fecha,Hora,Notas,FacturaId")] Cita cita)
         {

             var identidad = User.Identity as ClaimsIdentity;
             string idUsuarioLogeado = identidad.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

             if (!User.IsInRole("Administrador") && !User.IsInRole("Estilista"))
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


                 // Crear la factura
                 var servicio = await _context.Servicios.FindAsync(cita.ServicioId);
                 var factura = new Factura
                 {
                     ClienteId = cita.ClienteId,
                     PrecioTotal = servicio.Precio,
                     Fecha = DateTime.Now
                 };

                 _context.Add(factura);
                 await _context.SaveChangesAsync();

                 // Crear el detalle de la factura
                 var detalleFactura = new DetalleFactura
                 {
                     FacturaId = factura.FacturaId,
                     ServicioId = cita.ServicioId,
                     PrecioUnitario = servicio.Precio 
                 };

                 _context.Add(detalleFactura);
                 await _context.SaveChangesAsync();

                 // Vincular la cita con la factura
                 cita.FacturaId = factura.FacturaId;
                 _context.Update(cita);
                 await _context.SaveChangesAsync();


                 // Enviar correo de confirmación al cliente y al estilista
                 var cliente = await _userManager.FindByIdAsync(cita.ClienteId);
                // var servicio = await _context.Servicios.FindAsync(cita.ServicioId);
                 var estilistaEmail = _configuration["Estilista:Email"];

                 if (cliente != null && !string.IsNullOrWhiteSpace(cliente.Email))
                 {
                     var emailContent = $@"
                 <p>Hola {cliente.Nombre},</p>
                 <p>Código de reserva: {cita.CitaId}</p>
                 <p>Tu cita para el servicio {servicio.Nombre} ha sido agendada para el {cita.Fecha:dd/MM/yyyy} a las {cita.Hora:hh\:mm}.</p>
                 <p>Notas: {cita.Notas}</p>
                 <p>Gracias por elegirnos!</p>";
                     try
                     {
                         await _emailSender.SendEmailAsync(cliente.Email, "Confirmación de Cita", emailContent);
                     }
                     catch (Exception ex)
                     {
                         ModelState.AddModelError("", "Hubo un problema al enviar el correo de confirmación al cliente. Por favor, inténtalo de nuevo más tarde.");
                     }
                 }
                 else
                 {
                     ModelState.AddModelError("", "No se pudo enviar el correo de confirmación porque la dirección de correo del cliente no es válida.");
                 }

                 if (!string.IsNullOrWhiteSpace(estilistaEmail))
                 {
                     var estilistaEmailContent = $@"
                 <p>Hola Viviana,</p>
                 <p>Código de reserva: {cita.CitaId}</p>
                 <p>Se ha agendado una nueva cita para el servicio {servicio.Nombre} con {cliente.Nombre} el {cita.Fecha:dd/MM/yyyy} a las {cita.Hora:hh\:mm}.</p>
                 <p>Notas: {cita.Notas}</p>";
                     try
                     {
                         await _emailSender.SendEmailAsync(estilistaEmail, "Nueva Cita Agendada", estilistaEmailContent);
                     }
                     catch (Exception ex)
                     {
                         ModelState.AddModelError("", "Hubo un problema al enviar el correo de notificación al estilista. Por favor, inténtalo de nuevo más tarde.");
                     }
                 }
                 //await _emailSender.SendEmailAsync(cliente.Email, "Confirmación de Cita", emailContent);
                 //await _emailSender.SendEmailAsync(estilistaEmail, "Nueva Cita Agendada", estilistaEmailContent);

                 return RedirectToAction(nameof(Index));
             }


             ViewData["ClienteId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Cliente"), "Id", "Nombre", cita.ClienteId);
             ViewBag.ServicioId = new SelectList(await _context.Servicios.ToListAsync(), "ServicioId", "Nombre", cita.ServicioId);
             return View(cita);

         }*/


        // GET: Citas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Citas == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound();
            }

            // Allow editing only if the user is an Admin, Stylist, or the customer who booked the appointment
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (cita.ClienteId != userId && !User.IsInRole("Administrador") && !User.IsInRole("Estilista"))
            {
                return Forbid();
            }

            // Cargar todos los clientes si el usuario es un "Administrador" o "Estilista"
            if (User.IsInRole("Administrador") || User.IsInRole("Estilista"))
            {
                var usuariosCliente = await _userManager.GetUsersInRoleAsync("Cliente");

                ViewData["ClienteId"] = new SelectList(
            usuariosCliente.Select(u => new
            {
                Id = u.Id,
                NombreCompleto = $"{u.Nombre} {u.Apellido}"
            }).ToList(),
            "Id",
            "NombreCompleto",
            cita.ClienteId
        );


            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                ViewData["ClienteId"] = new List<SelectListItem>
                
                {
            new SelectListItem
            {
                Text = $"{user.Nombre} {user.Apellido}",
                Value = userId

            }
        };
            }

            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre", cita.ServicioId);
            return View(cita);
        }

        // POST: Citas/Edit/5
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
                    var citaDb = await _context.Citas.FindAsync(id);
                    if (citaDb == null)
                    {
                        return NotFound();
                    }

                    citaDb.ClienteId = cita.ClienteId;
                    citaDb.Estado = cita.Estado;
                    citaDb.ServicioId = cita.ServicioId;
                    citaDb.Fecha = cita.Fecha;
                    citaDb.Hora = cita.Hora;
                    citaDb.Notas = cita.Notas;

                    _context.Update(citaDb);
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
            ViewData["ClienteId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Cliente"), "Id", "Nombre", cita.ClienteId);
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre", cita.ServicioId);
            return View(cita);
        }

        // Nueva acción para finalizar la cita
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verificar si el usuario es un estilista
            if (!User.IsInRole("Estilista"))
            {
                return Forbid();
            }

            // Actualizar el estado de la cita a "Finalizada"
            cita.Estado = 0;
            _context.Update(cita);
            await _context.SaveChangesAsync();

            var cliente = await _userManager.FindByIdAsync(cita.ClienteId);
            if (cliente != null && !string.IsNullOrWhiteSpace(cliente.Email))
            {
                string subject = "Gracias por tu cita";
                string message = $"Estimado/a {cliente.Nombre},\n\n" +
                                  $"Queremos agradecerte por tu visita, esperamos que el servicio haya sido de su agrado.\n\n" +
                                  $"Saludos cordiales,\n" +
                                  "El equipo de Beauty Salon";
                try
                {
                    await _emailSender.SendEmailAsync(cliente.Email, subject, message);
                }
                catch (Exception ex)
                {
                    // Puedes agregar un log aquí si es necesario o mostrar un mensaje de error
                    ModelState.AddModelError("", "Hubo un problema al enviar el correo de confirmación. Por favor, inténtalo de nuevo más tarde.");
                }
            }
            else
            {
                ModelState.AddModelError("", "No se pudo enviar el correo de confirmación porque la dirección de correo del cliente no es válida.");
            }

            return RedirectToAction(nameof(Index));
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
            var cita = await _context.Citas
                .Include(c => c.Factura) // Incluir la factura asociada
                .ThenInclude(f => f.DetalleFacturas) // Incluir los detalles de la factura
                .FirstOrDefaultAsync(c => c.CitaId == id);

            if (cita != null)
            {
                // Eliminar detalles de la factura
                if (cita.Factura != null)
                {
                    var detallesFactura = cita.Factura.DetalleFacturas.ToList();
                    _context.DetalleFacturas.RemoveRange(detallesFactura);

                    // Eliminar la factura
                    _context.Facturas.Remove(cita.Factura);
                }

                // Eliminar la cita
                _context.Citas.Remove(cita);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.CitaId == id);
        }


    }
}
