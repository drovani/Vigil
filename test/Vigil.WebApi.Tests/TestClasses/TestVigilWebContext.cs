using Microsoft.EntityFrameworkCore;
using Vigil.WebApi.Controllers;

namespace Vigil.WebApi
{
    public class TestVigilWebContext : VigilWebContext
    {
        public TestVigilWebContext(DbContextOptions<VigilWebContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEventSourced>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
