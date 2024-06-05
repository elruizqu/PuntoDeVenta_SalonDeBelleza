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

        //DbSet<clase>
    }
}
