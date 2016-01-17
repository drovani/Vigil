using System;
using System.ComponentModel;

namespace Vigil.Patrons.Model
{
    public class PatronSearchModel
    {
        [DefaultValue(100)]
        public int ResultLimit { get; set; }

        public string PatronType { get; set; }
        public string AccountNumber { get; set; }
        public string DisplayName { get; set; }
        public bool? IsAnonymous { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOnAfter { get; set; }
        public DateTime? CreatedOnBefore { get; set; }
        public bool? HasBeenDeleted { get; set; }

        public PatronSearchModel()
        {
            ResultLimit = 100;
        }
    }

}
