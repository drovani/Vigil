using System;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;

namespace Vigil.Patron.Model
{
    public class PatronReadModel
    {
        public Guid Id { get; set; }
        public string PatronType { get; set; }
        public string AccountNumber { get; set; }
        public string DisplayName { get; set; }
        public bool IsAnonymous { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
