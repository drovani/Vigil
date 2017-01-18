using Microsoft.Extensions.DependencyInjection;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;

namespace Vigil.Patrons
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPatronCommandHandlers(this IServiceCollection services)
        {
            services.AddTransient<ICommandHandler<CreatePatron>, PatronCommandHandler>()
                    .AddTransient<ICommandHandler<UpdatePatronHeader>, PatronCommandHandler>()
                    .AddTransient<ICommandHandler<DeletePatron>, PatronCommandHandler>();

            return services;
        }
    }
}
