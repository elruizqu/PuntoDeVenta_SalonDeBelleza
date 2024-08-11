using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Microsoft.AspNetCore.Identity;

namespace POS_BeautySalon.Controllers
{
    public class ProductoLealtadController : Controller
    {
        private readonly SalonContext _salonContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductoLealtadController(SalonContext salonContext, UserManager<ApplicationUser> userManager)
        {
            _salonContext = salonContext;
            _userManager = userManager;
        }


        // GET: ProductoLealtad/CatalogoProdLealtad
        public async Task<IActionResult> CatalogoProdLealtad()
        {
            /*var productos = await _salonContext.ProductosLealtad.ToListAsync();
            return View(productos); Asi estaba antes de pasar los puntos a la vista*/ 

            var userId = _userManager.GetUserId(User);
            var cliente = await _userManager.FindByIdAsync(userId);
            var puntosCliente = cliente.PuntosProgramaLealtad;

            ViewBag.PuntosCliente = puntosCliente;

            var productos = await _salonContext.ProductosLealtad.ToListAsync();
            return View(productos);
        }


        // GET: ProductoLealtad
        public async Task<IActionResult> Index()
        {
            return View(await _salonContext.ProductosLealtad.ToListAsync());
        }

        // GET: ProductoLealtad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productoLealtad = await _salonContext.ProductosLealtad
                .FirstOrDefaultAsync(m => m.ProductoLealtadId == id);
            if (productoLealtad == null)
            {
                return NotFound();
            }

            return View(productoLealtad);
        }

        // GET: ProductoLealtad/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductoLealtad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductoLealtadId,Nombre,Descripcion,PrecioPuntos")] ProductoLealtad productoLealtad, IFormFile ImagenProductoLealtad)
        {
            if (ModelState.IsValid)
            {
                if (ImagenProductoLealtad != null && ImagenProductoLealtad.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ImagenProductoLealtad.CopyToAsync(memoryStream);
                        productoLealtad.ImagenProductolealtad = memoryStream.ToArray();
                    }
                }

                _salonContext.Add(productoLealtad);
                await _salonContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productoLealtad);
        }

        // POST: ProductoLealtad/CanjearProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CanjearProducto(int productoLealtadId)
        {
            var userId = _userManager.GetUserId(User);
            var cliente = await _userManager.FindByIdAsync(userId);
            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(CatalogoProdLealtad));
            }

            var producto = await _salonContext.ProductosLealtad.FindAsync(productoLealtadId);
            if (producto == null)
            {
                TempData["ErrorMessage"] = "Producto no encontrado.";
                return RedirectToAction(nameof(CatalogoProdLealtad));
            }

            if (cliente.PuntosProgramaLealtad < producto.PrecioPuntos)
            {
                TempData["ErrorMessage"] = "No tienes suficientes puntos para canjear este producto.";
                return RedirectToAction(nameof(CatalogoProdLealtad));
            }

            // Restar puntos del usuario
            cliente.PuntosProgramaLealtad -= producto.PrecioPuntos;
            _salonContext.Update(cliente);
            await _salonContext.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Has canjeado {producto.Nombre}. Puedes retirar el producto en el salón de belleza con tu correo electronico.";
            return RedirectToAction(nameof(CatalogoProdLealtad));
        }

        // GET: ProductoLealtad/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productoLealtad = await _salonContext.ProductosLealtad.FindAsync(id);
            if (productoLealtad == null)
            {
                return NotFound();
            }
            return View(productoLealtad);
        }

        // POST: ProductoLealtad/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoLealtadId,Nombre,Descripcion,ImagenProductolealtad,PrecioPuntos")] ProductoLealtad productoLealtad, IFormFile ImagenProductolealtad)
        {
            if (id != productoLealtad.ProductoLealtadId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productoLealtadDb = await _salonContext.ProductosLealtad.FindAsync(id);

                    if (productoLealtadDb == null)
                    {
                        return NotFound();
                    }
                    // Si no se sube una nueva imagen, mantenemos la imagen existente
                    if (ImagenProductolealtad != null && ImagenProductolealtad.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await ImagenProductolealtad.CopyToAsync(memoryStream);
                            productoLealtadDb.ImagenProductolealtad = memoryStream.ToArray();
                        }
                    }

                    // Actualizar otros campos del producto
                    productoLealtadDb.Nombre = productoLealtad.Nombre;
                    productoLealtadDb.Descripcion = productoLealtad.Descripcion;
                    productoLealtadDb.PrecioPuntos = productoLealtad.PrecioPuntos;

                    _salonContext.Update(productoLealtadDb);
                    await _salonContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoLealtadExists(productoLealtad.ProductoLealtadId))
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
            return View(productoLealtad);
        }

        // GET: ProductoLealtad/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productoLealtad = await _salonContext.ProductosLealtad
                .FirstOrDefaultAsync(m => m.ProductoLealtadId == id);
            if (productoLealtad == null)
            {
                return NotFound();
            }

            return View(productoLealtad);
        }

        // POST: ProductoLealtad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productoLealtad = await _salonContext.ProductosLealtad.FindAsync(id);
            if (productoLealtad != null)
            {
                _salonContext.ProductosLealtad.Remove(productoLealtad);
            }

            await _salonContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoLealtadExists(int id)
        {
            return _salonContext.ProductosLealtad.Any(e => e.ProductoLealtadId == id);
        }
    }
}
