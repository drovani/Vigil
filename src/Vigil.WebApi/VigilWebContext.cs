using Microsoft.EntityFrameworkCore;
using Vigil.Patrons;

namespace Vigil.WebApi.Controllers
{
    public class VigilWebContext : DbContext
    {
        public VigilWebContext(DbContextOptions<VigilWebContext> options) : base(options) { }

        public DbSet<Patron> Patrons { get; set; }
    }
}