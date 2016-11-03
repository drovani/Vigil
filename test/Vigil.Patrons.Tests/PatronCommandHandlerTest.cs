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
            CreatePatron command = new CreatePatron()
            {
                DisplayName = "Test Patron",
                IsAnonymous = false,
                PatronType = "Test Account"
            };

            var eventBus = new Mock<IEventBus>();
            eventBus.Setup(bus => bus.Publish(It.IsAny<PatronCreated>()))
                .Callback<PatronCreated>((@event) =>
                {
                    Assert.Equal(command.DisplayName, @event.DisplayName);
                    Assert.Equal(command.IsAnonymous, @event.IsAnonymous);
                    Assert.Equal(command.PatronType, @event.PatronType);
                    Assert.Equal(command.Id, @event.SourceId);
                    Assert.NotEqual(command.Id, @event.Id);
                    Assert.NotEqual(command.Id, @event.PatronId);
                    Assert.NotEqual(Guid.Empty, @event.PatronId);
                    Assert.NotEqual(Guid.Empty, @event.Id);
                }).Verifiable();
            var repo = new Mock<ICommandRepository>();
            repo.Setup(re => re.Save(It.Is<CreatePatron>(cpc => cpc.Id == command.Id))).Verifiable();

            ICommandHandler<CreatePatron> handler = new PatronCommandHandler(eventBus.Object, repo.Object);
            handler.Handle(command);

            Assert.NotEqual(Guid.Empty, command.Id);
            Mock.Verify(eventBus, repo);
        }

        [Fact]
        public void User_Can_Update_a_Patron_Header()
        {
            UpdatePatronHeader command = new UpdatePatronHeader()
            {
                PatronId = Guid.NewGuid(),
                DisplayName = "Updated Test Patron"
            };

            var eventBus = new Mock<IEventBus>();
            eventBus.Setup(bus => bus.Publish(It.IsAny<PatronHeaderChanged>()))
                .Callback<PatronHeaderChanged>((@event) =>
                {
                    Assert.Equal(command.DisplayName, @event.DisplayName);
                    Assert.Equal(command.IsAnonymous, @event.IsAnonymous);
                    Assert.Equal(command.PatronType, @event.PatronType);
                    Assert.Equal(command.Id, @event.SourceId);
                    Assert.Equal(command.PatronId, @event.PatronId);
                    Assert.NotEqual(command.Id, @event.Id);
                    Assert.NotEqual(Guid.Empty, @event.Id);
                }).Verifiable();
            var repo = new Mock<ICommandRepository>();
            repo.Setup(re => re.Save(It.Is<UpdatePatronHeader>(cpc => cpc.Id == command.Id))).Verifiable();

            ICommandHandler<UpdatePatronHeader> handler = new PatronCommandHandler(eventBus.Object, repo.Object);
            handler.Handle(command);

            Assert.NotEqual(Guid.Empty, command.Id);
            Mock.Verify(eventBus, repo);
        }
    }
}
