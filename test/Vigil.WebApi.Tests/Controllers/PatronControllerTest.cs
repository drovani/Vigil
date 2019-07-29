using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
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
            BadRequestObjectResult result;
            using (PatronController controller = new PatronController(cmdQueue, dbContextFactory))
            {
                result = controller.Create(null) as BadRequestObjectResult;
            }

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(result.Value);
        }

        [Fact]
        public void Create_With_ModelStateError_Returns_BadRequest()
        {
            BadRequestObjectResult result;
            using (PatronController controller = new PatronController(cmdQueue, dbContextFactory))
            {
                controller.ModelState.AddModelError("DisplayName", "Display Name error is being tested.");

                result = controller.Create(new CreatePatron("Patron Web Test", TestHelper.Now)) as BadRequestObjectResult;
            }

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.IsType<BadRequestObjectResult>(result);
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

            AcceptedResult result;
            using (PatronController controller = new PatronController(cmdQueue, dbContextFactory)
            {
                Url = mockUrlHelper.Object
            })
            {
                result = controller.Create(cmd) as AcceptedResult;
            }

            Assert.IsType<AcceptedResult>(result);
            mockUrlHelper.Verify(urlSetup, Times.Once());
            cmdQueueMock.Verify(cq => cq.Publish(It.IsAny<CreatePatron>()), Times.Once());
            Assert.Equal("a/mock/url/for/testing", result.Location);
        }

        [Fact]
        public void UpdateHeader_With_NullCommand_Returns_BadRequest()
        {
            BadRequestObjectResult result;
            using (PatronController controller = new PatronController(cmdQueue, dbContextFactory))
            {
                result = controller.UpdateHeader(Guid.NewGuid(), null) as BadRequestObjectResult;
            }

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(result.Value);
        }

        [Fact]
        public void UpdateHeader_With_ModelStateError_Returns_BadRequest()
        {
            BadRequestObjectResult result;
            using (PatronController controller = new PatronController(cmdQueue, dbContextFactory))
            {
                controller.ModelState.AddModelError("DisplayName", "Display Name error is being tested.");
                var command = new UpdatePatronHeader("Patron Web Test", TestHelper.Now)
                {
                    PatronId = Guid.NewGuid(),
                    DisplayName = string.Join(" - ", Enumerable.Repeat("A Really Long String", 1000))
                };

                result = controller.UpdateHeader(command.PatronId, command) as BadRequestObjectResult;
            }

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(result.Value);
        }

        [Fact]
        [Trait("FailsBecause", "Find(id) throws NullReferenceException instead of returning null")]
        public void UpdateHeader_With_ValidCommand_ButPatronNotFound_Returns_NotFound()
        {
            NotFoundObjectResult result;
            UpdatePatronHeader command;
            using (PatronController controller = new PatronController(cmdQueue, dbContextFactory))
            {
                command = new UpdatePatronHeader("Patron Web Test", TestHelper.Now) { PatronId = Guid.NewGuid() };

                result = controller.UpdateHeader(command.PatronId, command) as NotFoundObjectResult;
            }

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<Guid>(result.Value);
            Assert.Equal(command.PatronId, (Guid)result.Value);
        }

        [Fact]
        public void UpdateHeader_With_ValidCommand_ButMismatchedPatronIds_Returns_BadRequest()
        {
            UpdatePatronHeader command;
            BadRequestObjectResult result;
            Guid newPatronGuid = Guid.NewGuid();
            Guid somethingDifferentGuid = Guid.NewGuid();
            using (PatronController controller = new PatronController(cmdQueue, dbContextFactory))
            {
                command = new UpdatePatronHeader("Patron Web Test", TestHelper.Now) { PatronId = newPatronGuid };

                result = controller.UpdateHeader(somethingDifferentGuid, command) as BadRequestObjectResult;
            }

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.NotEqual(newPatronGuid, somethingDifferentGuid);
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(result.Value);
        }

        [Fact]
        public void UpdateHeader_With_ValidCommand_ForFoundPatron_IsPublished_AndReturnsAcceptedResult_WithLocation()
        {
            Guid patronId = MakeTestPatron();
            UpdatePatronHeader cmd = new UpdatePatronHeader("Patron Web Test", TestHelper.Now)
            {
                PatronId = patronId,
                DisplayName = "Updated Patron Display Name",
                IsAnonymous = false,
                PatronType = "Patron Updated"
            };

            cmdQueueMock.Setup(c => c.Publish(It.IsAny<UpdatePatronHeader>())).Verifiable();

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            Expression<Func<IUrlHelper, string>> urlSetup = m => m.Action(It.Is<UrlActionContext>(uac => uac.Action == "Get" && GetId(uac.Values) == patronId));
            mockUrlHelper.Setup(urlSetup).Returns("a/mock/url/for/testing").Verifiable();

            AcceptedResult result;
            using (var controller = new PatronController(cmdQueue, dbContextFactory)
            {
                Url = mockUrlHelper.Object
            })
            {
                result = controller.UpdateHeader(cmd.PatronId, cmd) as AcceptedResult;
            }

            Assert.IsType<AcceptedResult>(result);
            Assert.Equal(patronId, cmd.PatronId);
            mockUrlHelper.Verify(urlSetup, Times.Once());
            cmdQueueMock.Verify(c => c.Publish(It.IsAny<UpdatePatronHeader>()), Times.Once());
            Assert.Equal("a/mock/url/for/testing", result.Location);
        }

        [Fact]
        public void Delete_WhenPatronNotFound_Returns_NotFound()
        {
            Guid patronId = Guid.NewGuid();
            NotFoundObjectResult result;
            using (PatronController controller = new PatronController(cmdQueue, dbContextFactory))
            {

                result = controller.Delete(patronId) as NotFoundObjectResult;
            }

            cmdQueueMock.Verify(c => c.Publish(It.IsAny<ICommand>()), Times.Never());
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<Guid>(result.Value);
            Assert.Equal(patronId, (Guid)result.Value);
        }

        [Fact]
        public void Delete_WhenPatronFound_Returns_Accepted()
        {
            Guid patronId = MakeTestPatron();
            Expression<Action<ICommandQueue>> publishSetup = cq => cq.Publish(It.Is<DeletePatron>(dp => dp.PatronId == patronId));

            AcceptedResult result;
            using (PatronController controller = new PatronController(cmdQueue, dbContextFactory)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new Mock<HttpContext>().Object
                }
            })
            {
                SetupUser(controller, "Delete User");

                cmdQueueMock.Setup(publishSetup).Verifiable();

                result = controller.Delete(patronId) as AcceptedResult;
            }

            cmdQueueMock.Verify(publishSetup, Times.Once);
            Assert.IsType<AcceptedResult>(result);
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