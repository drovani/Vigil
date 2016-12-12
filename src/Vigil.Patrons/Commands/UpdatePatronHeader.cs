using System;
using System.ComponentModel.DataAnnotations;

namespace Vigil.Patrons.Commands
{
    public class UpdatePatronHeader : PatronCommand
    {
        [StringLength(250)]
        public string DisplayName { get; set; }
        public bool? IsAnonymous { get; set; }
        [StringLength(250)]
        public string PatronType { get; set; }

        public UpdatePatronHeader(string generatedBy, DateTime generatedOn) : base(generatedBy, generatedOn) { }
    }
}
