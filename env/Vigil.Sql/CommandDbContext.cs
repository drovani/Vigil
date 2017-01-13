using Microsoft.EntityFrameworkCore;

namespace Vigil.Sql
{
    internal class CommandDbContext : DbContext
    {
        public DbSet<Command> Commands { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}
