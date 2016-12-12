using System;
using System.Collections.Generic;
using Vigil.Domain.EventSourcing;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Events;
using Xunit;

namespace Vigil.Patrons
{
    public class PatronTest
    {
        private readonly DateTime Now = new DateTime(1981, 8, 25, 20, 17, 00, DateTimeKind.Utc);

        [Fact]
        public void Patron_Can_Be_Materialized_From_Patron_Created()
        {
            var patronId = Guid.NewGuid();

            PatronCreated created = new PatronCreated("Create User", Now, Guid.NewGuid())
            {
                DisplayName = "Test Creation",
                IsAnonymous = false,
                PatronType = "Test Account",
                PatronId = patronId,
                Version = 0
            };
            Patron result = new Patron(patronId, new[] { created });

            Assert.Equal("Test Creation", result.DisplayName);
            Assert.False(result.IsAnonymous);
            Assert.Equal("Test Account", result.PatronType);
            Assert.Equal(patronId, result.Id);
            Assert.Equal(0, result.Version);
            Assert.Equal("Create User", result.CreatedBy);
            Assert.Equal(Now, result.CreatedOn);
            Assert.Null(result.ModifiedBy);
            Assert.Null(result.ModifiedOn);
            Assert.Null(result.DeletedBy);
            Assert.Null(result.DeletedOn);
        }

        [Fact]
        public void Patron_Can_Be_Materialized_From_Patron_Created_And_Updated()
        {
            var patronId = Guid.NewGuid();
            var evnts = new VersionedEvent[] {
                new PatronCreated("Create User", Now, Guid.NewGuid())
                {
                    DisplayName = "Test Creation",
                    IsAnonymous = false,
                    PatronType = "Test Account",
                    PatronId = patronId,
                    Version = 0
                },
                new PatronHeaderChanged("Change User", Now.AddDays(1), Guid.NewGuid())
                {
                    DisplayName = "Test Update",
                    IsAnonymous = true,
                    PatronType = "Test Updated",
                    PatronId = patronId,
                    Version = 1
                }
            };
            Patron result = new Patron(patronId, evnts);

            Assert.Equal("Test Update", result.DisplayName);
            Assert.True(result.IsAnonymous);
            Assert.Equal("Test Updated", result.PatronType);
            Assert.Equal(patronId, result.Id);
            Assert.Equal(1, result.Version);
            Assert.Equal("Create User", result.CreatedBy);
            Assert.Equal(Now, result.CreatedOn);
            Assert.Equal("Change User", result.ModifiedBy);
            Assert.Equal(Now.AddDays(1), result.ModifiedOn);
            Assert.Null(result.DeletedBy);
            Assert.Null(result.DeletedOn);
        }

        [Fact]
        public void Patron_Can_Be_Materialized_From_Patron_Created_And_Deleted()
        {
            var patronId = Guid.NewGuid();
            var evnts = new VersionedEvent[] {
                new PatronCreated("Create User", Now, Guid.NewGuid())
                {
                    DisplayName = "Test Creation",
                    IsAnonymous = false,
                    PatronType = "Test Account",
                    PatronId = patronId,
                    Version = 0
                },
                new PatronDeleted("Delete User", Now.AddDays(1), Guid.NewGuid())
                {
                    PatronId = patronId,
                    Version = 1
                }
            };
            Patron result = new Patron(patronId, evnts);

            Assert.Equal("Test Creation", result.DisplayName);
            Assert.False(result.IsAnonymous);
            Assert.Equal(patronId, result.Id);
            Assert.Equal(1, result.Version);
            Assert.Equal("Create User", result.CreatedBy);
            Assert.Equal(Now, result.CreatedOn);
            Assert.Equal("Delete User", result.ModifiedBy);
            Assert.Equal("Delete User", result.DeletedBy);
            Assert.Equal(Now.AddDays(1), result.ModifiedOn);
            Assert.Equal(Now.AddDays(1), result.DeletedOn);
        }
    }
}
