using Vigil.Data.Core;
using Xunit;

namespace Vigil.Testing.Data.Core.ValueObjects
{
    [global::System.Diagnostics.Contracts.ContractVerification(false)]
    public class ValueObjectTests
    {
        // cribbed from http://grabbagoft.blogspot.com/2007/06/generic-value-object-equality.html


        private class Address : ValueObject<Address>
        {
            private readonly string _address1;
            private readonly string _city;
            private readonly string _state;

            public string Address1 { get { return _address1; } }
            public string City { get { return _city; } }
            public string State { get { return _state; } }

            public Address(string address1, string city, string state)
            {
                _address1 = address1;
                _city = city;
                _state = state;
            }
        }

        private class ExpandedAddress : Address
        {
            private readonly string _address2;

            public string Address2 { get { return _address2; } }

            public ExpandedAddress(string address1, string address2, string city, string state)
                : base(address1, city, state)
            {
                _address2 = address2;
            }
        }

        [Fact]
        public void Address_Equals_Is_True_With_Identical_Address()
        {
            Address address = new Address("Address1", "City", "State");
            Address address2 = new Address("Address1", "City", "State");

            Assert.True(address.Equals(address2));
        }
        [Fact]
        public void Address_Equals_Is_False_With_Different_Address()
        {
            Address address = new Address("Address1", "City", "State");
            Address address2 = new Address("Address2", "City", "State");

            Assert.False(address.Equals(address2));
        }
        [Fact]
        public void Address_Equals_Is_False_With_Null_Address1()
        {
            Address address = new Address(null, "City", "State");
            Address address2 = new Address("Address1", "City", "State");

            Assert.False(address.Equals(address2));
        }
        [Fact]
        public void Address_Equals_Is_False_With_Null_Address1_Other_Object()
        {
            Address address = new Address("Address1", "City", "State");
            Address address2 = new Address("Address1", null, "State");

            Assert.False(address.Equals(address2));
        }
        [Fact]
        public void Address_Equals_Is_True_When_Reflexive()
        {
            Address address = new Address("Address1", "City", "State");

            Assert.True(address.Equals(address));
        }
        [Fact]
        public void Address_Equals_Is_Symmetric()
        {
            Address address = new Address("Address1", "City", "State");
            Address address2 = new Address("Address2", "City", "State");

            Assert.False(address.Equals(address2));
            Assert.False(address2.Equals(address));
        }

        [Fact]
        public void AddressOperatorsWork()
        {
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address1", "Austin", "TX");
            Address address3 = new Address("Address2", "Austin", "TX");

            Assert.True(address == address2);
            Assert.True(address2 != address3);
        }

        [Fact]
        public void DerivedTypesBehaveCorrectly()
        {
            Address address = new Address("Address1", "Austin", "TX");
            ExpandedAddress address2 = new ExpandedAddress("Address1", "Apt 123", "Austin", "TX");

            Assert.False(address.Equals(address2));
            Assert.False(address == address2);
        }

        [Fact]
        public void EqualValueObjectsHaveSameHashCode()
        {
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address1", "Austin", "TX");

            Assert.Equal(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void TransposedValuesGiveDifferentHashCodes()
        {
            Address address = new Address(null, "Austin", "TX");
            Address address2 = new Address("TX", "Austin", null);

            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void UnequalValueObjectsHaveDifferentHashCodes()
        {
            Address address = new Address("Address1", "Austin", "TX");
            Address address2 = new Address("Address2", "Austin", "TX");

            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void TransposedValuesOfFieldNamesGivesDifferentHashCodes()
        {
            Address address = new Address("_city", null, null);
            Address address2 = new Address(null, "_address1", null);

            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void DerivedTypesHashCodesBehaveCorrectly()
        {
            ExpandedAddress address = new ExpandedAddress("Address99999", "Apt 123", "New Orleans", "LA");
            ExpandedAddress address2 = new ExpandedAddress("Address1", "Apt 123", "Austin", "TX");

            Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
        }
    }
}
