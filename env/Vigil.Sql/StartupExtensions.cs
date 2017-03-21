using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Vigil.Domain.Messaging;
using Vigil.Sql;

namespace Vigil.WebApi.Configuration
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddVigilSqlServices(this IServiceCollection services, string databaseName)
        {
            services.AddTransient<IEventBus, SqlEventBus>()
                    .AddTransient<ICommandQueue, SqlCommandQueue>();

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(databaseName: databaseName)
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            services.AddSingleton<Func<SqlMessageDbContext>>(srv => () => new SqlMessageDbContext(options));

            return services;
        }
    }
}
