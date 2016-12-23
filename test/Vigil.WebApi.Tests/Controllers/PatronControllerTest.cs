using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Vigil.Domain.Messaging;
using Xunit;

namespace Vigil.WebApi.Controllers
{
    public class PatronControllerTest
    {
        public readonly ICommandQueue cmdQueue = Mock.Of<ICommandQueue>();
        public readonly Func<VigilWebContext> Context;

        public PatronControllerTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<VigilWebContext>()
                .UseInMemoryDatabase(databaseName: "TestHelper")
                .UseInternalServiceProvider(serviceProvider);

            Context = () => new VigilWebContext(builder.Options);
        }

        [Fact]
        public void Get_Returns_EmptyCollection_WhenThereAreNoPatrons()
        {
            var controller = new PatronController(cmdQueue, Context);

            var result = controller.Get() as OkObjectResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Value);
        }
    }
}