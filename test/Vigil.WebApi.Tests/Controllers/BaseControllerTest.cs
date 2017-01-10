using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using Vigil.Domain.Messaging;
using Xunit;

namespace Vigil.WebApi.Controllers
{
    internal class TestBaseController : BaseController<TestEventSourced>
    {
        public TestBaseController(ICommandQueue commandQueue, Func<VigilWebContext> contextFactory) : base(commandQueue, contextFactory)
        {
        }
    }

    public class BaseControllerTest
    {
        private readonly ICommandQueue cmdQueue = Mock.Of<ICommandQueue>();
        private readonly Func<VigilWebContext> Context;
        private readonly TestEventSourced[] TestItems = new TestEventSourced[]
             {
                new TestEventSourced(Guid.NewGuid()) {
                    DeletedBy = "Test Delete",
                    DeletedOn = TestHelper.Now.AddDays(1)
                },
                new TestEventSourced(Guid.NewGuid()),
                new TestEventSourced(Guid.NewGuid())
             };

        public BaseControllerTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<VigilWebContext>()
                .UseInMemoryDatabase(databaseName: "TestHelper")
                .UseInternalServiceProvider(serviceProvider);

            Context = () => new TestVigilWebContext(builder.Options);
        }

        [Fact]
        public void Get_Returns_EmptyCollection_WhenThereAreNoEntities()
        {
            OkObjectResult result;
            using (var controller = new TestBaseController(cmdQueue, Context))
            {
                result = controller.Get();
            }

            Assert.NotNull(result);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public void Get_Returns_AllNonDeletedItems()
        {
            using (var context = Context())
            {
                context.AddRange(TestItems);
                context.SaveChanges();
            }

            OkObjectResult result;
            using (var controller = new TestBaseController(cmdQueue, Context))
            {
                result = controller.Get();
            }

            var data = result.Value as IReadOnlyCollection<TestEventSourced>;

            Assert.NotNull(result);
            Assert.NotNull(data);
            Assert.Equal(2, data.Count());
            Assert.DoesNotContain(data, d => d.Id == TestItems[0].Id);
            Assert.Contains(data, d => d.Id == TestItems[1].Id);
            Assert.Contains(data, d => d.Id == TestItems[2].Id);
        }

        [Fact]
        public void Get_ById_Returns_NotFound_WhenIdDoesNotMatch()
        {
            using (var context = Context())
            {
                context.AddRange(TestItems);
                context.SaveChanges();
            }

            NotFoundResult result;
            using (var controller = new TestBaseController(cmdQueue, Context))
            {
                result = controller.Get(Guid.NewGuid()) as NotFoundResult;
            }

            Assert.NotNull(result);
        }

        [Fact]
        public void Get_ById_Returns_FoundEntity()
        {
            using (var context = Context())
            {
                context.AddRange(TestItems);
                context.SaveChanges();
            }

            OkObjectResult result;
            using (var controller = new TestBaseController(cmdQueue, Context))
            {
                result = controller.Get(TestItems[1].Id) as OkObjectResult;
            }

            Assert.NotNull(result);
            Assert.NotNull(result.Value as TestEventSourced);
            Assert.Equal(TestItems[1].Id, (result.Value as TestEventSourced).Id);
        }
    }
}