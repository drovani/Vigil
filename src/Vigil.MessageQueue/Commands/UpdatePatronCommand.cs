using System.ComponentModel.DataAnnotations;
using Vigil.Domain;

namespace Vigil.MessageQueue.Commands
{
    public class UpdatePatronCommand : ICommand
    {
        [Required]
        public IKeyIdentity TargetPatron { get; set; }

        [StringLength(250)]
        public string DisplayName { get; set; }
        public bool? IsAnonymous { get; set; }
        [StringLength(250)]
        public string PatronType { get; set; }
    }
}
