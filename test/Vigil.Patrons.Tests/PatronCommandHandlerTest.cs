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
        private readonly DateTime Now = new DateTime(1981, 8, 25, 20, 17, 00, DateTimeKind.Utc);

        [Fact]
        public void User_Can_Create_New_Patron()
        {
            CreatePatron command = new CreatePatron("Create User", Now)
            {
                DisplayName = "Test Patron",
                IsAnonymous = false,
                PatronType = "Test Account"
            };

            var eventBus = new Mock<IEventBus>();
            eventBus.Setup(bus => bus.Publish(It.IsAny<PatronCreated>()))
                .Callback<PatronCreated>((evnt) =>
                {
                    Assert.Equal(command.DisplayName, evnt.DisplayName);
                    Assert.Equal(command.IsAnonymous, evnt.IsAnonymous);
                    Assert.Equal(command.PatronType, evnt.PatronType);
                    Assert.Equal(command.Id, evnt.SourceId);
                    Assert.NotEqual(command.Id, evnt.Id);
                    Assert.NotEqual(command.Id, evnt.PatronId);
                    Assert.NotEqual(Guid.Empty, evnt.PatronId);
                    Assert.NotEqual(Guid.Empty, evnt.Id);
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
            UpdatePatronHeader command = new UpdatePatronHeader("Update User", Now)
            {
                PatronId = Guid.NewGuid(),
                DisplayName = "Updated Test Patron",
                IsAnonymous = true,
                PatronType = "Updated Type"
            };

            var eventBus = new Mock<IEventBus>();
            eventBus.Setup(bus => bus.Publish(It.IsAny<PatronHeaderChanged>()))
                .Callback<PatronHeaderChanged>((evnt) =>
                {
                    Assert.Equal("Updated Test Patron", evnt.DisplayName);
                    Assert.Equal(command.IsAnonymous, evnt.IsAnonymous);
                    Assert.Equal(command.PatronType, evnt.PatronType);
                    Assert.Equal(command.Id, evnt.SourceId);
                    Assert.Equal(command.PatronId, evnt.PatronId);
                    Assert.NotEqual(command.Id, evnt.Id);
                    Assert.Equal("Update User", evnt.GeneratedBy);
                    Assert.Equal(Now, evnt.GeneratedOn);
                }).Verifiable();
            var repo = new Mock<ICommandRepository>();
            repo.Setup(re => re.Save(It.Is<UpdatePatronHeader>(cpc => cpc.Id == command.Id))).Verifiable();

            ICommandHandler<UpdatePatronHeader> handler = new PatronCommandHandler(eventBus.Object, repo.Object);
            handler.Handle(command);

            Assert.NotEqual(Guid.Empty, command.Id);
            Mock.Verify(eventBus, repo);
        }

        [Fact]
        public void User_Can_Delete_a_Patron()
        {
            DeletePatron command = new DeletePatron("Delete User", Now.AddDays(1))
            {
                PatronId = Guid.NewGuid()
            };

            var eventBus = new Mock<IEventBus>();
            eventBus.Setup(bus => bus.Publish(It.IsAny<PatronDeleted>()))
                .Callback<PatronDeleted>((evnt) =>
                {
                    Assert.Equal(command.Id, evnt.SourceId);
                    Assert.Equal(command.PatronId, evnt.PatronId);
                    Assert.NotEqual(command.Id, evnt.Id);
                    Assert.NotEqual(command.Id, evnt.PatronId);
                    Assert.NotEqual(Guid.Empty, evnt.PatronId);
                    Assert.NotEqual(Guid.Empty, evnt.Id);
                }).Verifiable();
            var repo = new Mock<ICommandRepository>();
            repo.Setup(re => re.Save(It.Is<DeletePatron>(cpc => cpc.Id == command.Id))).Verifiable();

            ICommandHandler<DeletePatron> handler = new PatronCommandHandler(eventBus.Object, repo.Object);
            handler.Handle(command);

            Assert.NotEqual(Guid.Empty, command.Id);
            Mock.Verify(eventBus, repo);
        }
    }
}
