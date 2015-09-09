using System;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Testing.Identity.TestClasses;
using Xunit;

namespace Vigil.Testing.Identity.Model
{
    public class VigilUserManagerTests
    {
        [Fact]
        public void VigilUserManager_Constructor_Accepts_UserStore()
        {
            var vuman = new VigilUserManager(new InMemoryUserStore());
            Assert.NotNull(vuman);
        }

        [Fact(Skip="Requires an IOwinContext implementation to obtain IdentityVigilContext.")]
        public void VigilUserManager_Static_Create_Returns_Valid_Manager()
        {
            IdentityFactoryOptions<VigilUserManager> options = new IdentityFactoryOptions<VigilUserManager>();
            IOwinContext context = null;

            var userManager = VigilUserManager.Create(options, context);

            Assert.NotNull(userManager);
        }

        [Fact]
        public void CreateAsync_Sets_Empty_Id()
        {
            var vuman = new VigilUserManager(new InMemoryUserStore());
            var user = new VigilUser { UserName = "TestUser" };
            user.Id = Guid.Empty;

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
            var vuman = new VigilUserManager(new InMemoryUserStore());
            var newguid = Guid.NewGuid();
            var user = new VigilUser {Id = newguid, UserName = "TestUser" };

            var result = vuman.CreateAsync(user).Result;
            Assert.Equal(newguid, user.Id);
            Assert.True(result.Succeeded);
        }
        
        [Fact]
        public void CreateAsync_With_Password_Sets_Empty_Id()
        {
            var vuman = new VigilUserManager(new InMemoryUserStore());
            var user = new VigilUser { UserName = "TestUser" };
            user.Id = Guid.Empty;

            var result = vuman.CreateAsync(user, "testPassword.01").Result;
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void CreateAsync_With_Password_Preserves_Specified_Id()
        {
            var vuman = new VigilUserManager(new InMemoryUserStore());
            var newguid = Guid.NewGuid();
            var user = new VigilUser { Id = newguid, UserName = "TestUser" };

            var result = vuman.CreateAsync(user, "testPassword.01").Result;
            Assert.Equal(newguid, user.Id);
            Assert.True(result.Succeeded);
        }
    }
}
