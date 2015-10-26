using System;
using System.Diagnostics.Contracts;
using Vigil.Data.Core.Patrons;
using Vigil.Data.Core.Patrons.Types;

namespace Vigil.Patron.Model
{
    public class Patron
    {
        public Guid Id { get; protected set; }
        public string PatronType { get; protected set; }
        public string AccountNumber { get; protected set; }
        public string DisplayName { get; protected set; }
        public bool IsAnonymous { get; protected set; }
        public string CreatedBy { get; protected set; }
        public DateTime CreatedOn { get; protected set; }
        public string ModifiedBy { get; protected set; }
        public DateTime? ModifiedOn { get; protected set; }
        public string DeletedBy { get; protected set; }
        public DateTime? DeletedOn { get; protected set; }

        public Patron(PatronState patronState)
        {
            Contract.Requires<ArgumentNullException>(patronState != null);

            Id = patronState.Id;
            PatronType = patronState.PatronType.TypeName;
            AccountNumber = patronState.AccountNumber;
            DisplayName = patronState.DisplayName;
            IsAnonymous = patronState.IsAnonymous;
            CreatedBy = patronState.CreatedBy.UserName;
            CreatedOn = patronState.CreatedOn;
            ModifiedBy = patronState.ModifiedBy.UserName;
            ModifiedOn = patronState.ModifiedOn;
            DeletedBy = patronState.DeletedBy.UserName;
            DeletedOn = patronState.DeletedOn;
        }
    }
}
