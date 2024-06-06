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
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }

        public DbSet<Factura> Facturas { get; set; }

    }
}
