using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Events;
using Xunit;

namespace Vigil.Patrons
{
    public class PatronEventHandlerTest
    {
        private readonly DateTime Now = new DateTime(1981, 8, 25, 20, 17, 00, DateTimeKind.Utc);
        private readonly Mock<IPatronContext> context = new Mock<IPatronContext>();
        private readonly Mock<DbSet<Patron>> patrons = new Mock<DbSet<Patron>>();

        public PatronEventHandlerTest()
        {
            context.Setup(c => c.Patrons).Returns(patrons.Object);
        }

        [Fact]
        public void Handle_PatronCreated_Adds_NewPatron()
        {
            IEventHandler<PatronCreated> handler = new PatronEventHandler(() => context.Object);
            PatronCreated evnt = new PatronCreated("Create User", Now, Guid.NewGuid())
            {
                DisplayName = "New Patron",
                IsAnonymous = false,
                PatronType = "Test Account",
                PatronId = Guid.NewGuid(),
                Version = 0
            };

            handler.Handle(evnt);
            patrons.Verify(m => m.Add(It.Is<Patron>(p => p.Id == evnt.PatronId)));
            context.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Handle_PatronHeaderChanged_On_NonExistant_Patron_Does_Nothing()
        {
            IEventHandler<PatronHeaderChanged> handler = new PatronEventHandler(() => context.Object);

            handler.Handle(new PatronHeaderChanged("Change User", Now, Guid.NewGuid())
            {
                PatronId = Guid.NewGuid()
            });

            context.Verify(m => m.SaveChanges(), Times.Never);
        }
    }
}
