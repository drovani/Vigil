using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Vigil.Patrons.Commands
{
    public class CreatePatron : PatronCommand
    {
        [Required, StringLength(250)]
        public string DisplayName { get; set; }
        [DefaultValue(false)]
        public bool IsAnonymous { get; set; } = false;
        [Required, StringLength(250)]
        public string PatronType { get; set; }

        public CreatePatron(string generatedBy, DateTime generatedOn) : base(generatedBy, generatedOn) { }
    }
}
