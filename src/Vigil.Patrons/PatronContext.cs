using Microsoft.EntityFrameworkCore;

namespace Vigil.Patrons
{
    public class PatronContext : DbContext, IPatronContext
    {
        public DbSet<Patron> Patrons { get; set; }

        public PatronContext() { }
        public PatronContext(DbContextOptions<PatronContext> options) : base(options) { }
    }
}
