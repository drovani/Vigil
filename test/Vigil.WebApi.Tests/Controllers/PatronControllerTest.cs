using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;
using Vigil.Patrons.Events;
using Xunit;

namespace Vigil.WebApi.Controllers
{
    public class PatronControllerTest
    {
        protected readonly ICommandQueue cmdQueue;
        protected readonly Mock<ICommandQueue> cmdQueueMock;
        protected readonly Func<VigilWebContext> dbContextFactory;

        public PatronControllerTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<VigilWebContext>()
                .UseInMemoryDatabase(databaseName: "PatronControllerTestHelper")
                .UseInternalServiceProvider(serviceProvider);

            dbContextFactory = () => new VigilWebContext(builder.Options);

            cmdQueueMock = new Mock<ICommandQueue>();
            cmdQueue = cmdQueueMock.Object;
        }

        [Fact]
        public void Create_With_NullCommand_Returns_BadRequest()
        {
            PatronController controller = new PatronController(cmdQueue, dbContextFactory);

            BadRequestObjectResult result = controller.Create(null) as BadRequestObjectResult;

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.NotNull(result);
            Assert.IsType<SerializableError>(result.Value);
        }

        [Fact]
        public void Create_With_ModelStateError_Returns_BadRequest()
        {
            PatronController controller = new PatronController(cmdQueue, dbContextFactory);
            controller.ModelState.AddModelError("DisplayName", "Display Name error is being tested.");

            BadRequestObjectResult result = controller.Create(new CreatePatron("Patron Web Test", TestHelper.Now)) as BadRequestObjectResult;

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.NotNull(result);
            Assert.IsType<SerializableError>(result.Value);
        }

        [Fact]
        public void Create_With_ValidCommand_IsPublished_AndReturnsAcceptedResult_WithLocation()
        {
            CreatePatron cmd = new CreatePatron("Patron Web Test", TestHelper.Now)
            {
                DisplayName = "Patron Display Name",
                IsAnonymous = false,
                PatronType = "Test Patron"
            };
            cmdQueueMock.Setup(c => c.Publish(It.IsAny<CreatePatron>())).Verifiable();

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            Expression<Func<IUrlHelper, string>> urlSetup = url => url.Action(It.Is<UrlActionContext>(uac => uac.Action == "Get" && GetId(uac.Values) != cmd.Id));
            mockUrlHelper.Setup(urlSetup).Returns("a/mock/url/for/testing").Verifiable();

            var controller = new PatronController(cmdQueue, dbContextFactory)
            {
                Url = mockUrlHelper.Object
            };

            AcceptedResult result = controller.Create(cmd) as AcceptedResult;

            Assert.NotNull(result);
            mockUrlHelper.Verify(urlSetup, Times.Once());
            cmdQueueMock.Verify(cq => cq.Publish(It.IsAny<CreatePatron>()), Times.Once());
            Assert.Equal("a/mock/url/for/testing", result.Location);
        }

        [Fact]
        public void UpdateHeader_With_NullCommand_Returns_BadRequest()
        {
            PatronController controller = new PatronController(cmdQueue, dbContextFactory);

            BadRequestObjectResult result = controller.UpdateHeader(Guid.NewGuid(), null) as BadRequestObjectResult;

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.NotNull(result);
            Assert.IsType<SerializableError>(result.Value);
        }

        [Fact]
        public void UpdateHeader_With_ModelStateError_Returns_BadRequest()
        {
            PatronController controller = new PatronController(cmdQueue, dbContextFactory);
            controller.ModelState.AddModelError("DisplayName", "Display Name error is being tested.");

            BadRequestObjectResult result = controller.UpdateHeader(Guid.NewGuid(), new UpdatePatronHeader("Patron Web Test", TestHelper.Now)) as BadRequestObjectResult;

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.NotNull(result);
            Assert.IsType<SerializableError>(result.Value);
        }

        [Fact]
        public void UpdateHeader_With_ValidCommand_ButPatronNotFound_Returns_NotFound()
        {
            PatronController controller = new PatronController(cmdQueue, dbContextFactory);
            Guid patronId = Guid.NewGuid();

            NotFoundObjectResult result = controller.UpdateHeader(patronId, new UpdatePatronHeader("Patron Web Test", TestHelper.Now)) as NotFoundObjectResult;

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.NotNull(result);
            Assert.IsType<Guid>(result.Value);
            Assert.Equal(patronId, (Guid)result.Value);
        }

        [Fact]
        public void UpdateHeader_With_ValidCommand_ForFoundPatron_IsPublished_AndReturnsAcceptedResult_WithLocation()
        {
            Guid patronId = MakeTestPatron();
            UpdatePatronHeader cmd = new UpdatePatronHeader("Patron Web Test", TestHelper.Now)
            {
                DisplayName = "Updated Patron Display Name",
                IsAnonymous = false,
                PatronType = "Patron Updated"
            };

            cmdQueueMock.Setup(c => c.Publish(It.IsAny<UpdatePatronHeader>())).Verifiable();

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            Expression<Func<IUrlHelper, string>> urlSetup = m => m.Action(It.Is<UrlActionContext>(uac => uac.Action == "Get" && GetId(uac.Values) == patronId));
            mockUrlHelper.Setup(urlSetup).Returns("a/mock/url/for/testing").Verifiable();

            var controller = new PatronController(cmdQueue, dbContextFactory)
            {
                Url = mockUrlHelper.Object
            };

            AcceptedResult result = controller.UpdateHeader(patronId, cmd) as AcceptedResult;

            mockUrlHelper.Verify(urlSetup, Times.Once());
            cmdQueueMock.Verify(c => c.Publish(It.IsAny<UpdatePatronHeader>()), Times.Once());
            Assert.NotNull(result);
            Assert.Equal(patronId, cmd.PatronId);
            Assert.Equal("a/mock/url/for/testing", result.Location);
        }

        [Fact]
        public void Delete_WhenPatronNotFound_Returns_NotFound()
        {
            PatronController controller = new PatronController(cmdQueue, dbContextFactory);
            Guid patronId = Guid.NewGuid();

            NotFoundObjectResult result = controller.Delete(patronId) as NotFoundObjectResult;

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.NotNull(result);
            Assert.IsType<Guid>(result.Value);
            Assert.Equal(patronId, (Guid)result.Value);
        }

        [Fact]
        public void Delete_WhenPatronFound_Returns_Accepted()
        {
            Guid patronId = MakeTestPatron();
            PatronController controller = new PatronController(cmdQueue, dbContextFactory)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new Mock<HttpContext>().Object
                }
            };
            SetupUser(controller, "Delete User");

            Expression<Action<ICommandQueue>> publishSetup = cq => cq.Publish(It.Is<DeletePatron>(dp => dp.PatronId == patronId));
            cmdQueueMock.Setup(publishSetup).Verifiable();

            AcceptedResult result = controller.Delete(patronId) as AcceptedResult;

            cmdQueueMock.Verify(publishSetup, Times.Once);
            Assert.NotNull(result);
            Assert.Null(result.Value);
        }

        private Guid? GetId(object values)
        {
            return values?.GetType().GetProperty("id")?.GetValue(values, null) as Guid?;
        }
        private Guid MakeTestPatron()
        {
            Guid patronId = Guid.NewGuid();
            using (var ctx = dbContextFactory())
            {
                ctx.Patrons.Add(new Patrons.Patron(patronId, new[] {
                    new PatronCreated("Patron Web Test", TestHelper.Now.AddDays(-1), Guid.NewGuid())
                    {
                        DisplayName = "Patron Display Name",
                        PatronType = "Test Patron",
                        PatronId = patronId
                    }
                }));
                ctx.SaveChanges();
            }
            return patronId;
        }
        private void SetupUser(Controller controller, string username)
        {
            var mockContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockContext.SetupGet(hc => hc.User.Identity.Name).Returns(username);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
        }
    }
}