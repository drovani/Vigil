using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Testing.Identity.TestClasses;
using Vigil.Web.Controllers;
using Vigil.Web.Models;

namespace Vigil.Testing.Web.Controllers
{
    [TestClass()]
    public class AccountControllerTests
    {
        private VigilSignInManager signInMgr;

        [TestInitialize]
        public void TestInitialize()
        {
            var uman = new VigilUserManager(new InMemoryUserStore());
            var authman = new InMemoryAuthenticationManager();
            signInMgr = new VigilSignInManager(uman, authman);
        }

        [TestMethod()]
        public void AccountController_Default_Constructor()
        {
            var ctrl = new AccountController();
            Assert.IsNotNull(ctrl);
        }

        [TestMethod()]
        public void AccountController_Explicit_Constructor()
        {
            var ctrl = new AccountController(signInMgr);

            Assert.IsNotNull(ctrl);
            Assert.AreSame(signInMgr, ctrl.SignInManager);
            Assert.AreSame(signInMgr.UserManager, ctrl.UserManager);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AccountController_Explicit_Constructor_Null_UserManager_Throws_Exception()
        {
            new AccountController(null);
        }

        [TestMethod()]
        public void Login_Without_ReturnUrl_Returns_View_And_No_ViewBag_ReturnUrl()
        {
            var ctrl = new AccountController(signInMgr);
            var result = ctrl.Login(null);
            Assert.IsNotNull(result);
            Assert.IsNull(ctrl.ViewBag.ReturnUrl);
        }

        [TestMethod]
        public void Login_With_ReturnUrl_Sets_ViewBag_ReturnUrl_And_Returns_View()
        {
            var ctrl = new AccountController(signInMgr);
            var result = ctrl.Login("/Home");
            Assert.IsNotNull(result);
            Assert.AreEqual("/Home", ctrl.ViewBag.ReturnUrl);
        }

        [TestMethod()]
        public async Task Login_POST_Invalid_Model_Returns_View()
        {
            var ctrl = new AccountController(signInMgr);
            var model = new LoginViewModel()
            {
                Email = "notevalidemail",
                Password = "testPassword.01",
                RememberMe = false
            };
            ActionResult result = await ctrl.Login(model, null);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreSame(model, (result as ViewResult).Model);
        }

        [TestMethod]
        public async Task Login_POST_Successful_Login_Without_ReturnUrl_Returns_Redirect_To_Home()
        {
            var ctrl = new AccountController(signInMgr);
            var createResult = await signInMgr.UserManager.CreateAsync(new VigilUser()
            {
                UserName = "TestUser",
                Email = "valid@example.com"
            }, "testPassword.01");
            Assert.IsTrue(createResult.Succeeded);

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            ActionResult result = await ctrl.Login(model, null);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Home", (result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("Index", (result as RedirectToRouteResult).RouteValues["Action"]);
        }

        [TestMethod]
        public async Task Login_POST_LockedOut_Returns_Lockout_View()
        {
            var ctrl = new AccountController(signInMgr);
            var user = new VigilUser()
            {
                UserName = "TestUser",
                Email = "valid@example.com",
                LockoutEnabled = true,
                LockoutEndDateUtc = DateTime.UtcNow.AddDays(1)
            };
            var createResult = await signInMgr.UserManager.CreateAsync(user, "testPassword.01");
            Assert.IsTrue(createResult.Succeeded);
            Assert.IsTrue(await signInMgr.UserManager.IsLockedOutAsync(user.Id));

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            ActionResult result = await ctrl.Login(model, null);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Lockout", (result as ViewResult).ViewName);
        }

        [TestMethod]
        public async Task Login_POST_RequiresVerification_Redirects_To_SendCode_Action()
        {
            var ctrl = new AccountController(signInMgr);
            var user = new VigilUser()
            {
                UserName = "TestUser",
                Email = "valid@example.com",
                TwoFactorEnabled = true
            };
            var createResult = await signInMgr.UserManager.CreateAsync(user, "testPassword.01");
            Assert.IsTrue(createResult.Succeeded);

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            ActionResult result = await ctrl.Login(model, null);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Account", (result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("SendCode", (result as RedirectToRouteResult).RouteValues["Action"]);
        }

        [TestMethod()]
        public void VerifyCode_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void VerifyCode_Test1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void Register_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void Register_Test1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ConfirmEmail_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ForgotPassword_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ForgotPassword_Test1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ForgotPasswordConfirmation_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ResetPassword_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ResetPassword_Test1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ResetPasswordConfirmation_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ExternalLogin_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void SendCode_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void SendCode_Test1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ExternalLoginCallback_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ExternalLoginConfirmation_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void LogOff_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ExternalLoginFailure_Test()
        {
            throw new NotImplementedException();
        }
    }
}
