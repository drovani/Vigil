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
            return new TestTypeBase(typeName);
        }
    }
}
