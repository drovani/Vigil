using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.Diagnostics;
using Vigil.Identity.Model;

namespace Vigil.Testing.Web.TestClasses
{
    internal class TestStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();
            app.UseWelcomePage("/Welcome");
            app.CreatePerOwinContext(IdentityVigilContext.Create);
            app.CreatePerOwinContext<VigilUserManager>(InMemoryUserManager.Create);
            app.CreatePerOwinContext<VigilSignInManager>(VigilSignInManager.Create);
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello world using OWIN TestServer");
            });
        }
    }
}
