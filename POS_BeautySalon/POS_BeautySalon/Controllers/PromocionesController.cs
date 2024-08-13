using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;

namespace POS_BeautySalon.Controllers
{
    public class PromocionesController : Controller
    {
        private readonly SalonContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PromocionesController(SalonContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        // GET: Promociones
        public async Task<IActionResult> Index()
        {
            var salonContext = _context.Promociones.Include(p => p.Servicio);
            return View(await salonContext.ToListAsync());
        }

        public async Task<IActionResult> CatalogoProm()
        {
            var promociones = await _context.Promociones.Include(p => p.Servicio).ToListAsync();
            return View(await _context.Promociones.ToListAsync());
        }


        // GET: Promociones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promocion = await _context.Promociones
                .Include(p => p.Servicio)
                .FirstOrDefaultAsync(m => m.PromocionId == id);
            if (promocion == null)
            {
                return NotFound();
            }

            return View(promocion);
        }

        // GET: Promociones/Create
        public IActionResult Create()
        {
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre");
            return View();
        }

        public async Task<List<ApplicationUser>> GetUsuariosConRolClienteAsync()
        {
            var usuariosConRolCliente = new List<ApplicationUser>();
            var rolCliente = await _roleManager.FindByNameAsync("Cliente");

            if (rolCliente != null)
            {
                usuariosConRolCliente = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync(rolCliente.Name);
            }

            return usuariosConRolCliente;
        }

        // POST: Promociones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PromocionId,Descripcion,ImagenPromocion,ServicioId,ProductoId")] Promocion promocion, IFormFile ImagenPromocion)
        {
            

            if (ModelState.IsValid)
            {

                byte[]? imagen = null;

                if (ImagenPromocion != null && ImagenPromocion.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ImagenPromocion.CopyToAsync(memoryStream);
                        imagen = memoryStream.ToArray();
                    }
                }

                Promocion prom = new Promocion
                {
                    Descripcion = promocion.Descripcion,
                    ImagenPromocion = imagen,
                    ServicioId = promocion.ServicioId
                };

                _context.Add(prom);
                await _context.SaveChangesAsync();

                // Enviar correos a clientes
                await EnviarCorreoPromocionAsync(prom);

                return RedirectToAction(nameof(Index));

            }
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre", promocion.ServicioId);
            return View(promocion);
        }

        private async Task EnviarCorreoPromocionAsync(Promocion promocion)
        {
            var usuariosConRolCliente = await GetUsuariosConRolClienteAsync();
            var correosClientes = usuariosConRolCliente.Select(u => u.Email).ToList();

            // Convertir la imagen a base64 para incrustarla en el correo
            string imagenBase64 = Convert.ToBase64String(promocion.ImagenPromocion);
            string imagenSrc = $"data:image/png;base64,{imagenBase64}";

            var asunto = "Nueva Promoción en Nuestro Salón de Belleza";
            var mensaje = $@"
            <h2>¡Tenemos una nueva promoción para ti!</h2>
            <p>{promocion.Descripcion}</p>
            <img src='{imagenSrc}' alt='Imagen de la Promoción' style='max-width: 100%; height: auto;' />
            <p>No te lo pierdas. Visítanos pronto.</p>
        ";

            foreach (var email in correosClientes)
            {
                await _emailSender.SendEmailAsync(email, asunto, mensaje);
            }
        }

        public async Task SendEmailAsync(List<string> emails, string subject, string message)
        {
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true
            };

            foreach (var email in emails)
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Smtp:From"]),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (SmtpException ex)
                {
                    // Log the exception or handle it accordingly
                    throw new InvalidOperationException($"Failed to send email to {email}.", ex);
                }
            }
        }

        // GET: Promociones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion == null)
            {
                return NotFound();
            }
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre", promocion.ServicioId);
            return View(promocion);
        }

        // POST: Promociones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PromocionId,Descripcion,ImagenPromocion,ServicioId,ProductoId")] Promocion promocion, IFormFile ImagenPromocion)
        {
            if (id != promocion.PromocionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var promocionDb = await _context.Promociones.FindAsync(id);

                    if (promocionDb == null)
                    {
                        return NotFound();
                    }

                    // Si no se sube una nueva imagen, mantenemos la imagen existente
                    if (ImagenPromocion != null && ImagenPromocion.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await ImagenPromocion.CopyToAsync(memoryStream);
                            promocionDb.ImagenPromocion = memoryStream.ToArray();
                        }
                    }

                    promocionDb.Descripcion = promocion.Descripcion;
                    promocionDb.ServicioId = promocion.ServicioId;

                    _context.Update(promocionDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromocionExists(promocion.PromocionId))
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
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "ServicioId", "Nombre", promocion.ServicioId);
            return View(promocion);
        }

        // GET: Promociones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promocion = await _context.Promociones
                .Include(p => p.Servicio)
                .FirstOrDefaultAsync(m => m.PromocionId == id);
            if (promocion == null)
            {
                return NotFound();
            }

            return View(promocion);
        }

        // POST: Promociones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion != null)
            {
                _context.Promociones.Remove(promocion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PromocionExists(int id)
        {
            return _context.Promociones.Any(e => e.PromocionId == id);
        }
    }
}
