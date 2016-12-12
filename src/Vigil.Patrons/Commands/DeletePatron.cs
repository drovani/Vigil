using System;

namespace Vigil.Patrons.Commands
{
    public class DeletePatron : PatronCommand
    {
        public DeletePatron(string generatedBy, DateTime generatedOn) : base(generatedBy, generatedOn) { }
    }
}
