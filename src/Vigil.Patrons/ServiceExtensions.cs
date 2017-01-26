using Microsoft.Extensions.DependencyInjection;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;
using Vigil.Patrons.Events;

namespace Vigil.Patrons
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPatronHandlers(this IServiceCollection services)
        {
            services.AddTransient<ICommandHandler<CreatePatron>, PatronCommandHandler>()
                    .AddTransient<ICommandHandler<UpdatePatronHeader>, PatronCommandHandler>()
                    .AddTransient<ICommandHandler<DeletePatron>, PatronCommandHandler>();
            services.AddTransient<IEventHandler<PatronCreated>, PatronEventHandler>()
                    .AddTransient<IEventHandler<PatronHeaderChanged>, PatronEventHandler>()
                    .AddTransient<IEventHandler<PatronDeleted>, PatronEventHandler>();

            return services;
        }
    }
}
