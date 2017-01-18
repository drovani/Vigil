using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using Vigil.Domain.Messaging;
using Vigil.Patrons;
using Vigil.Patrons.Commands;
using Vigil.Sql;
using Vigil.WebApi.Binders;
using Vigil.WebApi.Controllers;

namespace Vigil.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                .AddMvcOptions(options =>
                {
                    options.InputFormatters.Insert(0, new CommandInputFormatter());
                });
            services.AddTransient<IEventBus, SqlEventBus>()
                    .AddTransient<ICommandQueue, SqlCommandQueue>();
            services.AddPatronCommandHandlers();

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<VigilWebContext>()
                .UseInMemoryDatabase(databaseName: "PatronControllerTestHelper")
                .UseInternalServiceProvider(serviceProvider);

            services.AddSingleton<Func<VigilWebContext>>(srv => () => new VigilWebContext(builder.Options));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }

            app.UseMvc();
        }
    }
}
