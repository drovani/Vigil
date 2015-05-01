using System;
using Vigil.Data.Core;

namespace Vigil.Testing.Data.TestClasses
{
    internal class TestIdentity : Identity
    {
        public static TestIdentity CreateTestIdentity(Guid id){
            return new TestIdentity
            {
                Id = id
            };
        }
    }
}
