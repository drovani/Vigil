using System;
using System.Diagnostics.Contracts;
using Vigil.Data.Core;

namespace Vigil.Testing.Data.TestClasses
{
    internal class TestIdentity : Identity
    {
        public static TestIdentity CreateTestIdentity(Guid id)
        {
            Contract.Requires(id != Guid.Empty);
            Contract.Ensures(Contract.Result<TestIdentity>().Id != Guid.Empty);

            return new TestIdentity
            {
                Id = id
            };
        }
    }
}
