using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Vigil.Domain.Messaging;

namespace Vigil.Patrons.Commands
{
    public class CreatePatron : ICommand
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(250)]
        public string DisplayName { get; set; }
        [DefaultValue(false)]
        public bool IsAnonymous { get; set; } = false;
        [Required, StringLength(250)]
        public string PatronType { get; set; }
    }
}
