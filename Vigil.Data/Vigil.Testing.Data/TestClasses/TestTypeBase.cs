using System;
using System.Diagnostics.Contracts;
using Vigil.Data.Core;

namespace Vigil.Testing.Data.TestClasses
{
    internal class TestTypeBase : TypeBase
    {
        protected TestTypeBase(string typeName)
            : base(typeName)
        {
            Contract.Assume(!String.IsNullOrEmpty(typeName));
        }

        internal static TestTypeBase CreateType(string typeName)
        {
            Contract.Assume(!String.IsNullOrEmpty(typeName));

            return new TestTypeBase(typeName);
        }
    }
}
