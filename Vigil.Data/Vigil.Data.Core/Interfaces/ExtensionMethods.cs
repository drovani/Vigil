using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Vigil.Data.Core
{
    public static class InterfaceExtensionMethods
    {
        public static bool MarkModified(this IModified modified, string modifiedBy, DateTime modifiedOn)
        {
            Contract.Requires<ArgumentNullException>(modified != null);
            Contract.Requires<ArgumentNullException>(modifiedBy != null);
            Contract.Requires<ArgumentException>(modifiedBy.Trim() != string.Empty);
            Contract.Requires<ArgumentException>(modifiedOn != default(DateTime));
            Contract.Requires<ArgumentOutOfRangeException>(modifiedOn >= modified.CreatedOn);

            modified.GetType()
                    .GetProperty(nameof(modified.ModifiedBy))
                    .SetValue(modified, modifiedBy);
            modified.GetType()
                    .GetProperty(nameof(modified.ModifiedOn))
                    .SetValue(modified, modifiedOn.ToUniversalTime());

            return true;
        }
        public static bool MarkDeleted(this IDeleted deleted, string deletedBy, DateTime deletedOn)
        {
            Contract.Requires<ArgumentNullException>(deleted != null);
            Contract.Requires<ArgumentNullException>(deletedBy != null);
            Contract.Requires<ArgumentException>(deletedBy.Trim() != string.Empty);
            Contract.Requires<ArgumentException>(deletedOn != default(DateTime));
            Contract.Requires<ArgumentOutOfRangeException>(deletedOn >= deleted.CreatedOn);

            if (deleted.DeletedBy == null && deleted.DeletedOn == null)
            {
                deleted.GetType()
                        .GetProperty(nameof(deleted.DeletedBy))
                        .SetValue(deleted, deletedBy);
                deleted.GetType()
                        .GetProperty(nameof(deleted.DeletedOn))
                        .SetValue(deleted, deletedOn.ToUniversalTime());
                return true;
            }

            return false;
        }
    }
}
