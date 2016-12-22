using System;
using Vigil.Domain.EventSourcing;
using Vigil.Patrons.Events;
using Xunit;

namespace Vigil.Patrons
{
    public class PatronTest
    {
        [Fact]
        public void Patron_Can_Be_Hydrated_From_Patron_Created()
        {
            var patronId = Guid.NewGuid();

            PatronCreated created = new PatronCreated("Create User", TestHelper.Now, Guid.NewGuid())
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
            Assert.Equal(TestHelper.Now, result.CreatedOn);
            Assert.Null(result.ModifiedBy);
            Assert.Null(result.ModifiedOn);
            Assert.Null(result.DeletedBy);
            Assert.Null(result.DeletedOn);
        }

        [Fact]
        public void Patron_Can_Be_Hydrated_From_Patron_Created_And_Updated()
        {
            var patronId = Guid.NewGuid();
            var evnts = new VersionedEvent[] {
                new PatronCreated("Create User", TestHelper.Now, Guid.NewGuid())
                {
                    DisplayName = "Test Creation",
                    IsAnonymous = false,
                    PatronType = "Test Account",
                    PatronId = patronId,
                    Version = 0
                },
                new PatronHeaderChanged("Change User", TestHelper.Later, Guid.NewGuid())
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
            Assert.Equal(TestHelper.Now, result.CreatedOn);
            Assert.Equal("Change User", result.ModifiedBy);
            Assert.Equal(TestHelper.Later, result.ModifiedOn);
            Assert.Null(result.DeletedBy);
            Assert.Null(result.DeletedOn);
        }

        [Fact]
        public void Patron_Can_Be_Hydrated_From_Patron_Created_And_Empty_Updated()
        {
            var patronId = Guid.NewGuid();
            var evnts = new VersionedEvent[] {
                new PatronCreated("Create User", TestHelper.Now, Guid.NewGuid())
                {
                    DisplayName = "Test Creation",
                    IsAnonymous = false,
                    PatronType = "Test Account",
                    PatronId = patronId,
                    Version = 0
                },
                new PatronHeaderChanged("Change User", TestHelper.Later, Guid.NewGuid())
                {
                    PatronId = patronId,
                    Version = 1
                }
            };
            Patron result = new Patron(patronId, evnts);

            Assert.Equal("Test Creation", result.DisplayName);
            Assert.False(result.IsAnonymous);
            Assert.Equal("Test Account", result.PatronType);
            Assert.Equal(patronId, result.Id);
            Assert.Equal(1, result.Version);
            Assert.Equal("Create User", result.CreatedBy);
            Assert.Equal(TestHelper.Now, result.CreatedOn);
            Assert.Equal("Change User", result.ModifiedBy);
            Assert.Equal(TestHelper.Later, result.ModifiedOn);
            Assert.Null(result.DeletedBy);
            Assert.Null(result.DeletedOn);
        }

        [Fact]
        public void Patron_Can_Be_Hydrated_From_Patron_Created_And_Deleted()
        {
            var patronId = Guid.NewGuid();
            var evnts = new VersionedEvent[] {
                new PatronCreated("Create User", TestHelper.Now, Guid.NewGuid())
                {
                    DisplayName = "Test Creation",
                    IsAnonymous = false,
                    PatronType = "Test Account",
                    PatronId = patronId,
                    Version = 0
                },
                new PatronDeleted("Delete User", TestHelper.Later, Guid.NewGuid())
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
            Assert.Equal(TestHelper.Now, result.CreatedOn);
            Assert.Equal("Delete User", result.ModifiedBy);
            Assert.Equal("Delete User", result.DeletedBy);
            Assert.Equal(TestHelper.Later, result.ModifiedOn);
            Assert.Equal(TestHelper.Later, result.DeletedOn);
        }
    }
}
