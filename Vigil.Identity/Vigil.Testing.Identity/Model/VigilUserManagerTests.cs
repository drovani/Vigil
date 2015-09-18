using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Moq;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    [System.Diagnostics.Contracts.ContractVerification(false)]
    public class VigilUserManagerTests
    {
        [Fact]
        public void VigilUserManager_Constructor_Accepts_UserStore()
        {
            var vuman = new VigilUserManager(Mock.Of<IUserStore<VigilUser, Guid>>());
            Assert.NotNull(vuman);
        }

        [Fact]
        public void VigilUserManager_Static_Create_Returns_Valid_Manager()
        {
            var context = new Mock<IOwinContext>();
            context.Setup(c => c.Get<IdentityVigilContext>(IdentityGlobalConstant.IdentityKeyPrefix + typeof(IdentityVigilContext).AssemblyQualifiedName)).Returns(new IdentityVigilContext());

            var userManager = VigilUserManager.Create(context.Object);

            Assert.NotNull(userManager);
        }

        [Fact]
        public void CreateAsync_Sets_Empty_Id()
        {
            var user = new VigilUser { UserName = "TestUser" };
            user.Id = Guid.Empty;
            var store = new Mock<IUserStore<VigilUser, Guid>>();
            store.Setup(st => st.CreateAsync(It.IsAny<VigilUser>()))
                 .Returns(Task.FromResult(IdentityResult.Success));
            store.Setup(st => st.FindByNameAsync(It.Is<string>(s => s == "TestUser")))
                 .ReturnsAsync(user);
            var vuman = new VigilUserManager(store.Object);

            var result = vuman.CreateAsync(user).Result;
            var retrievedUser = vuman.FindByNameAsync("TestUser").Result;

            Assert.True(result.Succeeded);
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.NotNull(retrievedUser);
            Assert.Equal(user, retrievedUser);
        }

        [Fact]
        public void CreateAsync_Preserves_Specified_Id()
        {
            var vuman = new VigilUserManager(Mock.Of<IUserPasswordStore<VigilUser, Guid>>());
            var newguid = Guid.NewGuid();
            var user = new VigilUser {Id = newguid, UserName = "TestUser" };

            var result = vuman.CreateAsync(user).Result;
            Assert.Equal(newguid, user.Id);
            Assert.True(result.Succeeded);
        }
        
        [Fact]
        public void CreateAsync_With_Password_Sets_Empty_Id()
        {
            var vuman = new VigilUserManager(Mock.Of<IUserPasswordStore<VigilUser, Guid>>());
            var user = new VigilUser { UserName = "TestUser" };
            user.Id = Guid.Empty;

            var result = vuman.CreateAsync(user, "testPassword.01").Result;
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void CreateAsync_With_Password_Preserves_Specified_Id()
        {
            var vuman = new VigilUserManager(Mock.Of<IUserPasswordStore<VigilUser, Guid>>());
            var newguid = Guid.NewGuid();
            var user = new VigilUser { Id = newguid, UserName = "TestUser" };

            var result = vuman.CreateAsync(user, "testPassword.01").Result;
            Assert.Equal(newguid, user.Id);
            Assert.True(result.Succeeded);
        }
    }
}
