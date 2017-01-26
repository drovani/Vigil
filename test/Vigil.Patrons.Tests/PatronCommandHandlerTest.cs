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
            CreatePatron command = new CreatePatron("Create User", TestHelper.Now)
            {
                DisplayName = "Test Patron",
                IsAnonymous = false,
                PatronType = "Test Account"
            };

            var eventBus = new Mock<IEventBus>();
            PatronCreated publishedEvent = null;
            eventBus.Setup(bus => bus.Publish(It.IsAny<PatronCreated>()))
                .Callback<PatronCreated>(evnt => publishedEvent = evnt).Verifiable();

            ICommandHandler<CreatePatron> handler = new PatronCommandHandler(eventBus.Object);
            handler.Handle(command);

            Assert.NotEqual(Guid.Empty, command.Id);
            eventBus.VerifyPublish<PatronCreated>(Times.Once());
            Assert.NotNull(publishedEvent);
            Assert.Equal(command.DisplayName, publishedEvent.DisplayName);
            Assert.Equal(command.IsAnonymous, publishedEvent.IsAnonymous);
            Assert.Equal(command.PatronType, publishedEvent.PatronType);
            Assert.Equal(command.Id, publishedEvent.SourceId);
            Assert.Equal(command.PatronId, publishedEvent.PatronId);
            Assert.NotEqual(command.Id, publishedEvent.Id);
            Assert.NotEqual(command.PatronId, publishedEvent.Id);
            Assert.Equal(0, publishedEvent.Version);
        }

        [Fact]
        public void User_Can_Update_a_Patron_Header()
        {
            UpdatePatronHeader command = new UpdatePatronHeader("Update User", TestHelper.Now)
            {
                PatronId = Guid.NewGuid(),
                DisplayName = "Updated Test Patron",
                IsAnonymous = true,
                PatronType = "Updated Type"
            };

            var eventBus = new Mock<IEventBus>();
            PatronHeaderChanged publishedEvent = null;
            eventBus.Setup(bus => bus.Publish(It.IsAny<PatronHeaderChanged>()))
                .Callback<PatronHeaderChanged>(evnt => publishedEvent = evnt).Verifiable();

            ICommandHandler<UpdatePatronHeader> handler = new PatronCommandHandler(eventBus.Object);
            handler.Handle(command);

            Assert.NotEqual(Guid.Empty, command.Id);
            eventBus.VerifyPublish<PatronHeaderChanged>(Times.Once());
            Assert.NotNull(publishedEvent);
            Assert.Equal("Updated Test Patron", publishedEvent.DisplayName);
            Assert.Equal(command.IsAnonymous, publishedEvent.IsAnonymous);
            Assert.Equal(command.PatronType, publishedEvent.PatronType);
            Assert.Equal(command.Id, publishedEvent.SourceId);
            Assert.Equal(command.PatronId, publishedEvent.PatronId);
            Assert.NotEqual(command.Id, publishedEvent.Id);
            Assert.Equal("Update User", publishedEvent.GeneratedBy);
            Assert.Equal(TestHelper.Now, publishedEvent.GeneratedOn);
        }

        [Fact]
        public void User_Can_Delete_a_Patron()
        {
            DeletePatron command = new DeletePatron("Delete User", TestHelper.Later)
            {
                PatronId = Guid.NewGuid()
            };

            var eventBus = new Mock<IEventBus>();
            PatronDeleted publishedEvent = null;
            eventBus.Setup(bus => bus.Publish(It.IsAny<PatronDeleted>()))
                    .Callback<PatronDeleted>(evnt => publishedEvent = evnt)
                    .Verifiable();

            ICommandHandler<DeletePatron> handler = new PatronCommandHandler(eventBus.Object);
            handler.Handle(command);

            Assert.NotEqual(Guid.Empty, command.Id);
            eventBus.VerifyPublish<PatronDeleted>(Times.Once());
            Assert.NotNull(publishedEvent);
            Assert.Equal(command.Id, publishedEvent.SourceId);
            Assert.Equal(command.PatronId, publishedEvent.PatronId);
            Assert.NotEqual(command.Id, publishedEvent.Id);
            Assert.NotEqual(command.Id, publishedEvent.PatronId);
            Assert.NotEqual(Guid.Empty, publishedEvent.PatronId);
            Assert.NotEqual(Guid.Empty, publishedEvent.Id);
        }
    }
}
