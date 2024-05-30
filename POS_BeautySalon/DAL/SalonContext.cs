using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class SalonContext : DbContext
    {
        public SalonContext() { }
        public SalonContext (DbContextOptions<SalonContext> options) : base(options) { }

        //DbSet<clase>
    }
}
