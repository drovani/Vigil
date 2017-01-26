using Microsoft.EntityFrameworkCore;

namespace Vigil.Patrons
{
    public class PatronContext : DbContext, IPatronContext
    {
        public DbSet<Patron> Patrons { get; set; }

        public PatronContext(DbContextOptions options) : base(options) { }
        public PatronContext(DbContextOptions<PatronContext> options) : base(options) { }
    }
}
