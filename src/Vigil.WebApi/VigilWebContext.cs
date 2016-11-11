using Microsoft.EntityFrameworkCore;
using Vigil.Patrons;

namespace Vigil.WebApi.Controllers
{
    public class VigilWebContext : DbContext
    {
        public DbSet<Patron> Patrons { get; set; }
    }
}