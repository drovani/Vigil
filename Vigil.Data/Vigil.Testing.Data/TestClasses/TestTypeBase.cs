using System;
using System.Diagnostics.Contracts;
using Vigil.Data.Core;

namespace Vigil.Testing.Data.TestClasses
{
    [ContractVerification(false)]
    internal class TestTypeBase : TypeBase
    {
        protected TestTypeBase(string typeName)
            : base(typeName)
        {
        }

        internal static TestTypeBase CreateType(string typeName)
        {
            Contract.Requires<ArgumentNullException>(typeName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(typeName.Trim()));
            Contract.Ensures(Contract.Result<TestTypeBase>() != null);

            return new TestTypeBase(typeName);
        }
    }
}
