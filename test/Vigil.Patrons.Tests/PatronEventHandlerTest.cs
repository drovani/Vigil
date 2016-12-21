using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Events;
using Xunit;
using Vigil.Domain.EventSourcing;

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
            var mContext = new Mock<IPatronContext>();
            mContext.Setup(m => m.Patrons.Find(It.IsAny<Guid>())).Returns(default(Patron));
            mContext.Setup(m => m.SaveChanges()).Verifiable();

            IEventHandler<PatronHeaderChanged> handler = new PatronEventHandler(() => mContext.Object);

            handler.Handle(new PatronHeaderChanged("Change User", Now, Guid.NewGuid())
            {
                PatronId = Guid.NewGuid()
            });

            mContext.Verify(m => m.Patrons.Find(It.IsAny<Guid>()), Times.Once);
            mContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Handle_PatronHeaderChanged_Updates_Existing_Patron()
        {
            Guid newId = Guid.NewGuid();
            using (var context = Context())
            {
                context.Patrons.Add(new Patron(newId, new[] {
                    new PatronCreated("Create User", Now, Guid.NewGuid())
                    {
                        DisplayName = "New Patron",
                        IsAnonymous = false,
                        PatronType = "Test Account",
                        PatronId = newId,
                        Version = 0
                    }
                }));
                context.SaveChanges();
            }

            IEventHandler<PatronHeaderChanged> handler = new PatronEventHandler(Context);
            handler.Handle(new PatronHeaderChanged("Change User", Now, Guid.NewGuid())
            {
                DisplayName = "Changed Name",
                PatronType = "New Type",
                Version = 1,
                PatronId = newId
            });

            using (var context = Context())
            {
                var patron = context.Patrons.Find(newId);
                Assert.Equal("Changed Name", patron.DisplayName);
                Assert.Equal("New Type", patron.PatronType);
                Assert.Equal(1, patron.Version);
                Assert.Equal(Now, patron.ModifiedOn);
                Assert.Equal("Change User", patron.ModifiedBy);
            }
        }

        [Fact]
        public void Handle_PatronDeleted_On_NonExistant_Patron_Does_Nothing()
        {
            var mContext = new Mock<IPatronContext>();
            mContext.Setup(m => m.Patrons.Find(It.IsAny<Guid>())).Returns(default(Patron));
            mContext.Setup(m => m.SaveChanges()).Verifiable();

            IEventHandler<PatronDeleted> handler = new PatronEventHandler(() => mContext.Object);

            handler.Handle(new PatronDeleted("Delete User", Now, Guid.NewGuid())
            {
                PatronId = Guid.NewGuid()
            });

            mContext.Verify(m => m.Patrons.Find(It.IsAny<Guid>()), Times.Once);
            mContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Handle_PatronDeleted_Sets_DeletedByOn_Fields()
        {
            Guid newId = Guid.NewGuid();
            using (var context = Context())
            {
                context.Patrons.Add(new Patron(newId, new[] {
                    new PatronCreated("Create User", Now, Guid.NewGuid())
                    {
                        DisplayName = "New Patron",
                        IsAnonymous = false,
                        PatronType = "Test Account",
                        PatronId = newId,
                        Version = 0
                    }
                }));
                context.SaveChanges();
            }

            IEventHandler<PatronDeleted> handler = new PatronEventHandler(Context);
            handler.Handle(new PatronDeleted("Delete User", Now.AddDays(1), Guid.NewGuid())
            {
                PatronId = newId
            });

            using (var context = Context())
            {
                var patron = context.Patrons.Find(newId);
                Assert.Equal("Delete User", patron.ModifiedBy);
                Assert.Equal(Now.AddDays(1), patron.ModifiedOn);
                Assert.Equal("Delete User", patron.DeletedBy);
                Assert.Equal(Now.AddDays(1), patron.DeletedOn);
            }

        }
    }
}
