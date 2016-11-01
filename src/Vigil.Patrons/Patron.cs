using System;
using Vigil.Domain;

namespace Vigil.Patrons
{
    public class Patron : IKeyIdentity
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public bool IsAnonymous { get; set; }
        public string PatronType { get; set; }

        public Patron(Guid patronId, string displayName, bool isAnonymous, string patronType)
        {
            Id = patronId;
            DisplayName = displayName;
            IsAnonymous = isAnonymous;
            PatronType = patronType;
        }
    }
}