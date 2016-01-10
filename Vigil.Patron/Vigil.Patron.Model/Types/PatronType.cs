using Vigil.Data.Core;

namespace Vigil.Patron.Model.Types
{
    public class PatronType : ValueObject<PatronType>
    {
        public string TypeName { get; private set; }
        public string Description { get; private set; }
        public bool IsOrganization { get; private set; }

        public PatronType (string typeName, string description, bool isOrganization)
        {
            TypeName = typeName;
            Description = description;
            IsOrganization = isOrganization;
        }
    }
}
