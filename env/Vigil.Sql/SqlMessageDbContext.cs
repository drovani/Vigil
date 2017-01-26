using Microsoft.EntityFrameworkCore;

namespace Vigil.Sql
{
    public class SqlMessageDbContext : DbContext
    {
        internal DbSet<Command> Commands { get; set; }
        internal DbSet<Event> Events { get; set; }

        public SqlMessageDbContext(DbContextOptions options) : base(options) { }
        public SqlMessageDbContext(DbContextOptions<SqlMessageDbContext> options) : base(options) { }
    }
}
