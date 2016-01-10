using System;
using Vigil.Data.Core;

namespace Vigil.Patron.Model
{
    public class PatronUpdateModel
    {
        public string PatronType { get; set; }
        public string DisplayName { get; set; }
        public bool? IsAnonymous { get; set; }
    }
}
