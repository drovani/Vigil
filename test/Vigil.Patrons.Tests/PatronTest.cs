using System;
using System.Collections.Generic;
using Vigil.Domain.EventSourcing;
using Vigil.Patrons.Events;
using Xunit;

namespace Vigil.Patrons
{
    public class PatronTest
    {
        [Fact]
        public void Patron_Can_Be_Materialized_From_Patron_Created()
        {
            var patronId = Guid.NewGuid();

            PatronCreated created = new PatronCreated()
            {
                DisplayName = "Test Creation",
                IsAnonymous = false,
                PatronType = "Test Account",
                PatronId = patronId,
                SourceId = Guid.NewGuid(),
                Version = 0
            };
            Patron result = new Patron(patronId, new VersionedEvent[] { created });

            Assert.Equal("Test Creation", result.DisplayName);
            Assert.False(result.IsAnonymous);
            Assert.Equal("Test Account", result.PatronType);
            Assert.Equal(patronId, result.Id);
            Assert.Equal(0, result.Version);
        }

        [Fact]
        public void Patron_Can_Be_Materialized_From_Patron_Created_And_Updated()
        {
            var patronId = Guid.NewGuid();
            var evnts = new VersionedEvent[] {
                new PatronCreated()
                {
                    DisplayName = "Test Creation",
                    IsAnonymous = false,
                    PatronType = "Test Account",
                    PatronId = patronId,
                    SourceId = Guid.NewGuid(),
                    Version = 0
                },
                new PatronHeaderChanged()
                {
                    DisplayName = "Test Update",
                    IsAnonymous = true,
                    PatronType = "Test Updated",
                    PatronId = patronId,
                    SourceId = Guid.NewGuid(),
                    Version = 1
                }
            };
            Patron result = new Patron(patronId, evnts);

            Assert.Equal("Test Update", result.DisplayName);
            Assert.True(result.IsAnonymous);
            Assert.Equal("Test Updated", result.PatronType);
            Assert.Equal(patronId, result.Id);
            Assert.Equal(1, result.Version);
        }
    }
}
