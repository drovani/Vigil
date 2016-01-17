using Vigil.Data.Core;

namespace Vigil.Patrons.Model.Types
{
    public class PatronTypeModel : ValueObject<PatronTypeModel>
    {
        public string TypeName { get; private set; }
        public string Description { get; private set; }
        public bool IsOrganization { get; private set; }

        public PatronTypeModel (string typeName, string description, bool isOrganization)
        {
            TypeName = typeName;
            Description = description;
            IsOrganization = isOrganization;
        }
    }
}
