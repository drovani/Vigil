using System;
using System.ComponentModel;

namespace Vigil.Patron.Model
{
    public class PatronReadModel
    {
        [ReadOnly(true)]
        public Guid Id { get; set; }
        public string PatronType { get; set; }
        [ReadOnly(true)]
        public string AccountNumber { get; set; }
        public string DisplayName { get; set; }
        public bool IsAnonymous { get; set; }
        [ReadOnly(true)]
        public string CreatedBy { get; set; }
        [ReadOnly(true)]
        public DateTime CreatedOn { get; set; }
        [ReadOnly(true)]
        public string ModifiedBy { get; set; }
        [ReadOnly(true)]
        public DateTime? ModifiedOn { get; set; }
        [ReadOnly(true)]
        public string DeletedBy { get; set; }
        [ReadOnly(true)]
        public DateTime? DeletedOn { get; set; }
    }
}
