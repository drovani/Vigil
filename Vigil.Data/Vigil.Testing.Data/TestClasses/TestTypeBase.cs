using System;
using System.Diagnostics.Contracts;
using Vigil.Data.Core;

namespace Vigil.Testing.Data.TestClasses
{
    internal class TestTypeBase : TypeBase
    {
        public TestTypeBase(string typeName)
            : base(typeName)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(typeName));
        }
    }
}
