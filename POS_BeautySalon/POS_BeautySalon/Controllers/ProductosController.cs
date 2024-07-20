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
    public class ProductosController : Controller
    {
        private readonly SalonContext _context;

        public ProductosController(SalonContext context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var salonContext = _context.Productos.Include(p => p.Categoria).Include(p => p.Marca).Include(p => p.Proveedor);
            return View(await salonContext.ToListAsync());
        }

        public async Task<IActionResult> CatalogoProd(string searchTerm, int? categoriaId)
        {
            /*var salonContext = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Proveedor);
            return View(await salonContext.ToListAsync());
            var productos = _context.Productos
        .Include(p => p.Categoria)
        .Include(p => p.Marca)
        .Include(p => p.Proveedor)
        .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                productos = productos.Where(p => p.Nombre.Contains(searchTerm) || p.Categoria.Descripcion.Contains(searchTerm));
            }

            ViewData["searchTerm"] = searchTerm;

            return View(await productos.ToListAsync());*/
            var productos = _context.Productos
        .Include(p => p.Categoria)
        .Include(p => p.Marca)
        .Include(p => p.Proveedor)
        .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                productos = productos.Where(p => p.Nombre.Contains(searchTerm));
            }

            if (categoriaId.HasValue)
            {
                productos = productos.Where(p => p.CategoriaId == categoriaId.Value);
            }

            var categorias = await _context.Categorias.ToListAsync();
            ViewBag.Categorias = new SelectList(categorias, "CategoriaId", "Descripcion", categoriaId);
            ViewData["searchTerm"] = searchTerm;

            return View(await productos.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Descripcion");
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "MarcaId", "Descripcion");
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "ProveedorId", "Email");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductoId,Nombre,Descripcion,ImagenProducto,Precio,Estado,Cantidad,CategoriaId,MarcaId,ProveedorId")] Producto producto, IFormFile ImagenProducto)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un producto con el mismo nombre
                var productoExistente = await _context.Productos.FirstOrDefaultAsync(p => p.Nombre == producto.Nombre);

                if(productoExistente != null)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un producto con este nombre.");
                    return View(producto);
                }

                byte[]? imagen = null;

                if (ImagenProducto != null && ImagenProducto.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ImagenProducto.CopyToAsync(memoryStream);
                        imagen = memoryStream.ToArray();
                    }
                }

                Producto prod = new Producto
                {
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    ImagenProducto = imagen,
                    Precio = producto.Precio,
                    Estado = 1,
                    Cantidad = producto.Cantidad,
                    CategoriaId = producto.CategoriaId,
                    MarcaId = producto.MarcaId,
                    ProveedorId = producto.ProveedorId
                };
                
                _context.Add(prod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Descripcion", producto.CategoriaId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "MarcaId", "Descripcion", producto.MarcaId);
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "ProveedorId", "Email", producto.ProveedorId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Descripcion", producto.CategoriaId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "MarcaId", "Descripcion", producto.MarcaId);
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "ProveedorId", "Email", producto.ProveedorId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoId,Nombre,Descripcion,ImagenProducto,Precio,Estado,Cantidad,CategoriaId,MarcaId,ProveedorId")] Producto producto)
        {
            if (id != producto.ProductoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verificar si ya existe un producto con el mismo nombre
                var productoExistente = await _context.Productos.FirstOrDefaultAsync(p => p.Nombre == producto.Nombre &&
                p.ProductoId != id);

                if (productoExistente != null)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un producto con este nombre.");
                    return View(producto);
                }

                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductoId))
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
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Descripcion", producto.CategoriaId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "MarcaId", "Descripcion", producto.MarcaId);
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "ProveedorId", "Email", producto.ProveedorId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }

        public async Task<IActionResult> ToggleAgotado(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if(producto == null)
            {
                return NotFound();
            }

            producto.Estado = producto.Estado == 0 ? 1 : 0;

            _context.Update(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
