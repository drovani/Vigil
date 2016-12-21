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
        private readonly Func<PatronContext> Context;


        public PatronEventHandlerTest()
        {
            var options = new DbContextOptionsBuilder<PatronContext>()
                .UseInMemoryDatabase(databaseName: "PatronEventHandlerTest")
                .Options;
            Context = () => new PatronContext(options);
        }

        [Fact]
        public void Handle_PatronCreated_Adds_NewPatron()
        {
            IEventHandler<PatronCreated> handler = new PatronEventHandler(Context);
            PatronCreated evnt = new PatronCreated("Create User", Now, Guid.NewGuid())
            {
                DisplayName = "New Patron",
                IsAnonymous = false,
                PatronType = "Test Account",
                PatronId = Guid.NewGuid(),
                Version = 0
            };

            handler.Handle(evnt);

            using (var context = Context.Invoke())
            {
                Patron result = context.Patrons.Single();
                Assert.Equal("New Patron", result.DisplayName);
                Assert.Equal("Test Account", result.PatronType);
                Assert.Equal(evnt.PatronId, result.Id);
                Assert.False(result.IsAnonymous);
                Assert.Equal("Create User", result.CreatedBy);
                Assert.Equal(Now, result.CreatedOn);
                Assert.Equal(0, result.Version);
                Assert.Null(result.ModifiedBy);
                Assert.Null(result.ModifiedOn);
                Assert.Null(result.DeletedBy);
                Assert.Null(result.DeletedOn);
            }
        }

        [Fact]
        public void Handle_PatronHeaderChanged_On_NonExistant_Patron_Does_Nothing()
        {
            IEventHandler<PatronHeaderChanged> handler = new PatronEventHandler(Context);

            handler.Handle(new PatronHeaderChanged("Change User", Now, Guid.NewGuid())
            {
                PatronId = Guid.NewGuid()
            });

        }
    }
}
