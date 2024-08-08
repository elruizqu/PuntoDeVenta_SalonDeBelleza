using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class SalonContext : DbContext
    {
        public SalonContext() { }
        public SalonContext (DbContextOptions<SalonContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer();
        }


        public DbSet<Marca> Marcas { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Promocion> Promociones { get; set; }

        public DbSet<Carrito> Carritos { get; set; }

        public DbSet<CarritoProducto> CarritoProductos { get; set; }

        public DbSet<Cita> Citas { get; set; }

        public DbSet<ListaDeseo> ListaDeseos { get; set; }

        public DbSet<ListaDeseoProducto> ListaDeseoProductos { get; set; }

        public DbSet<Cierre> Cierres { get; set; }

        public DbSet<DetalleFactura> DetalleFacturas { get; set; }

        public DbSet<CarritoProveedor> CarritoProveedores { get; set; }

        public DbSet<CarritoProvProducto> CarritoProvProductos { get; set; }

        public DbSet<FacturaProveedor> FacturaProveedores { get; set; }

        public DbSet<DetalleProveedorFactura> DetalleProveedorFacturas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
    }
}
