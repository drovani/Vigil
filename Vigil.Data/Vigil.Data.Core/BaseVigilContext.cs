using System.Data.Entity;

namespace Vigil.Data.Core
{
    public class BaseVigilContext<TContext> : DbContext where TContext : DbContext
    {
        static BaseVigilContext()
        {
            Database.SetInitializer<TContext>(new NullDatabaseInitializer<TContext>());
        }

        protected BaseVigilContext()
            : base("VigilContextConnection")
        {
        }
    }
}
