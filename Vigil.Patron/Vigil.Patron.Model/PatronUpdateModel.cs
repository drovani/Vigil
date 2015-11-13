using System;

namespace Vigil.Patron.Model
{
    public class PatronUpdateModel
    {
        public Guid Id { get; set; }
        public string PatronType { get; set; }
        public string AccountNumber { get; set; }
        public string DisplayName { get; set; }
        public bool? IsAnonymous { get; set; }
    }
}
