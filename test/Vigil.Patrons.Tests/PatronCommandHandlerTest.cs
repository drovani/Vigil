using Moq;
using System;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;
using Vigil.Patrons.Events;
using Xunit;

namespace Vigil.Patrons
{
    public class PatronCommandHandlerTest
    {
        [Fact]
        public void User_Can_Create_New_Patron()
        {
            CreatePatronCommand command = new CreatePatronCommand()
            {
                DisplayName = "Test User",
                IsAnonymous = false,
                PatronType = "Test Account"
            };

            var eventBus = new Mock<IEventBus>();
            eventBus.Setup(bus => bus.Publish(It.IsAny<PatronCreated>())).Verifiable();
            var repo = new Mock<ICommandRepository>();
            repo.Setup(re => re.Save(It.Is<CreatePatronCommand>(cpc => cpc.Id == command.Id))).Verifiable();

            PatronCommandHandler handler = new PatronCommandHandler(eventBus.Object, repo.Object);
            handler.Handle(command);

            Assert.NotEqual(Guid.Empty, command.Id);
            Mock.Verify(eventBus, repo);
        }

        [Fact]
        public void User_Cannot_Create_Patron_That_Fails_Validation()
        {
            //var queue = new Mock<ICommandQueue>(MockBehavior.Strict);
            //queue.Setup(q => q.QueueCommand(It.IsAny<ICommand>())).Verifiable();
            //PatronFactory factory = new PatronFactory(queue.Object);

            //FactoryResult result = factory.CreatePatron(new CreatePatronCommand());

            //queue.Verify(q => q.QueueCommand(It.IsAny<ICommand>()), Times.Never);
            //Assert.Equal(Guid.Empty, result.AffectedId);
            //Assert.NotEmpty(result.ValidationResults);
        }

        [Fact]
        public void User_Can_Update_a_Patron()
        {
            //var queue = new Mock<ICommandQueue>(MockBehavior.Strict);
            //queue.Setup(q => q.QueueCommand(It.IsAny<ICommand>())).Verifiable();
            //PatronFactory factory = new PatronFactory(queue.Object);
            //UpdatePatronCommand command = new UpdatePatronCommand()
            //{
            //    PatronId = Guid.NewGuid(),
            //    DisplayName = "Updated Patron Name",
            //    IsAnonymous = null,
            //    PatronType = null
            //};

            //FactoryResult result = factory.UpdatePatron(command);

            //queue.VerifyAll();
            //Assert.Equal(command.PatronId, result.AffectedId);
            //Assert.Empty(result.ValidationResults);
        }

        [Fact]
        public void User_Cannot_Update_Patron_That_Fails_Validation()
        {
            //var queue = new Mock<ICommandQueue>(MockBehavior.Strict);
            //queue.Setup(q => q.QueueCommand(It.IsAny<ICommand>())).Verifiable();
            //PatronFactory factory = new PatronFactory(queue.Object);

            //FactoryResult result = factory.UpdatePatron(new UpdatePatronCommand());

            //queue.Verify(q => q.QueueCommand(It.IsAny<ICommand>()), Times.Never);
            //Assert.Equal(Guid.Empty, result.AffectedId);
            //Assert.NotEmpty(result.ValidationResults);
        }
    }
}
