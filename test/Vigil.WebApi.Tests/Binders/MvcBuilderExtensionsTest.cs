using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Vigil.WebApi.Binders
{
    public class MvcBuilderExtensionsTest
    {
        [Fact]
        public void AddCommandFormatter_AddsACommandInputFormatter_ToTheListOfServices()
        {
            var manager = new ApplicationPartManager();
            var builder = new MvcBuilder(new ServiceCollection(), manager);

            IMvcBuilder result = builder.AddCommandFormatter();

            Assert.Same(result, builder);
            ServiceDescriptor descriptor = Assert.Single(result.Services);
            Assert.Equal(typeof(VigilMvcOptionsSetup), descriptor.ImplementationType);
            Assert.Equal(descriptor.Lifetime, ServiceLifetime.Transient);
            Assert.Equal(typeof(IConfigureOptions<MvcOptions>), descriptor.ServiceType);
        }
    }
}
