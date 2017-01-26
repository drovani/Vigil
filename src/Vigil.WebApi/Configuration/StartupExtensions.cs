using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Net;
using Vigil.Patrons;
using Vigil.Sql;
using Vigil.WebApi.Controllers;

namespace Vigil.WebApi.Configuration
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, string databaseName)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(databaseName: databaseName)
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            services.AddSingleton<Func<VigilWebContext>>(srv => () => new VigilWebContext(options))
                    .AddSingleton<Func<IPatronContext>>(srv => () => new VigilWebContext(options))
                    .AddSingleton<Func<SqlMessageDbContext>>(srv => () => new SqlMessageDbContext(options));

            return services;
        }

        public static IApplicationBuilder UseJsonExceptionResponse(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(options =>
                    {
                        options.Run(
                            async context =>
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                context.Response.ContentType = "application/json";
                                var ex = context.Features.Get<IExceptionHandlerFeature>();
                                if (ex != null)
                                {
                                    string[] stackTrace = ex.Error.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                                    var obj = new
                                    {
                                        ExceptionType = ex.GetType().FullName,
                                        Message = ex.Error.Message,
                                        StackTrace = stackTrace
                                    };
                                    string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);
                                    await context.Response.WriteAsync(jsonString).ConfigureAwait(false);
                                }
                            });
                    });
            return app;
        }
    }
}
