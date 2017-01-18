using Microsoft.EntityFrameworkCore;

namespace Vigil.Sql
{
    internal class CommandDbContext : DbContext
    {
        public DbSet<Command> Commands { get; set; }
        public DbSet<Event> Events { get; set; }

        public CommandDbContext() : base() { }
        public CommandDbContext(DbContextOptions<CommandDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "CommandDb");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
