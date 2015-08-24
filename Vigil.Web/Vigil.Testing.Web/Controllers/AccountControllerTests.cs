using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Testing.Identity.TestClasses;
using Vigil.Web.Controllers;
using Vigil.Web.Models;

namespace Vigil.Testing.Web.Controllers
{
    [TestClass]
    [ContractVerification(false)]
    public class AccountControllerTests
    {
        private VigilSignInManager signInMgr;
        private VigilUser validUser = new VigilUser
        {
            UserName = "ValidUser",
            Email = "valid@example.com"
        };

        [TestInitialize]
        public void TestInitialize()
        {
            var uman = new VigilUserManager(new InMemoryUserStore());
            var authman = new InMemoryAuthenticationManager();
            var tokenProvider = new InMemoryTokenProvider();
            uman.RegisterTwoFactorProvider("InMemory", tokenProvider);
            signInMgr = new VigilSignInManager(uman, authman);
        }

        [TestMethod]
        public void AccountController_Default_Constructor()
        {
            var ctrl = new AccountController();
            Assert.IsNotNull(ctrl);
        }

        [TestMethod]
        public void AccountController_Explicit_Constructor()
        {
            var ctrl = new AccountController(signInMgr);

            Assert.IsNotNull(ctrl);
            Assert.AreSame(signInMgr, ctrl.SignInManager);
            Assert.AreSame(signInMgr.UserManager, ctrl.UserManager);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AccountController_Explicit_Constructor_Null_UserManager_Throws_Exception()
        {
            new AccountController(null);
        }

        [TestMethod]
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

        [TestMethod]
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
            await CreateUserAsync();

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
            validUser.LockoutEnabled = true;
            validUser.LockoutEndDateUtc = DateTime.UtcNow.AddDays(1);

            await CreateUserAsync();
            Assert.IsTrue(await signInMgr.UserManager.IsLockedOutAsync(validUser.Id));

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
            validUser.TwoFactorEnabled = true;
            await CreateUserAsync();

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            ActionResult result = await ctrl.Login(model, null);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.IsNull((result as RedirectToRouteResult).RouteValues["ReturnUrl"]);
            Assert.AreEqual(model.RememberMe, (result as RedirectToRouteResult).RouteValues["RememberMe"]);
            Assert.AreEqual("SendCode", (result as RedirectToRouteResult).RouteValues["Action"]);
        }

        [TestMethod]
        public async Task Login_POST_RequiresVerification_Redirects_To_SendCode_Action_And_Retains_ReturnUrl()
        {
            var ctrl = new AccountController(signInMgr);
            validUser.TwoFactorEnabled = true;
            await CreateUserAsync();

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = true
            };
            ActionResult result = await ctrl.Login(model, "/Home/Index");

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("/Home/Index", (result as RedirectToRouteResult).RouteValues["ReturnUrl"]);
            Assert.AreEqual(model.RememberMe, (result as RedirectToRouteResult).RouteValues["RememberMe"]);
            Assert.AreEqual("SendCode", (result as RedirectToRouteResult).RouteValues["Action"]);
        }

        [TestMethod]
        public async Task Login_POST_Using_Incorrect_Password_Returns_View_With_Error()
        {
            var ctrl = new AccountController(signInMgr);
            await CreateUserAsync();

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "incorrectPassword",
                RememberMe = true
            };
            var result = await ctrl.Login(model, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.AreEqual("Invalid login attempt.", ctrl.ModelState[""].Errors[0].ErrorMessage);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task Login_POST_Using_Invalid_Credentials_Returns_View_With_Error()
        {
            var ctrl = new AccountController(signInMgr);
            var model = new LoginViewModel()
            {
                Email = "invalid@example.com",
                Password = "incorrectPassword",
                RememberMe = true
            };
            var result = await ctrl.Login(model, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.AreEqual("Invalid login attempt.", ctrl.ModelState[""].Errors[0].ErrorMessage);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task VerifyCode_With_Verified_User_Returns_ViewResult_With_VerifyCodeViewModel()
        {
            var ctrl = new AccountController(signInMgr);
            await CreateUserAsync();

            var result = await ctrl.VerifyCode("InMemory", null, false) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(VerifyCodeViewModel));
            Assert.AreEqual("InMemory", (result.Model as VerifyCodeViewModel).Provider);
        }

        [TestMethod]
        public async Task VerifyCode_Without_Verified_User_Returns_Error_View()
        {
            var ctrl = new AccountController(signInMgr);
            await CreateUserAsync();

            // somehow get signInMgr.HasBeenVerifiedAsync() to return false

            var result = await ctrl.VerifyCode("InMemory", null, false) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task VerifyCode_POST_Invalid_ModelState_Returns_View()
        {
            var ctrl = new AccountController(signInMgr);
            var model = new VerifyCodeViewModel();

            var result = await ctrl.VerifyCode(model) as ViewResult;

            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod()]
        [Ignore]
        public void VerifyCode_POST_Successful_Login_Without_ReturnUrl_Redirect_To_Home()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        [Ignore]
        public void VerifyCode_POST_LockedOut_Redirects_To_Lockout()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        [Ignore]
        public void VerifyCode_POST_Failure_Returns_View_With_Error()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Register_Returns_View()
        {
            var ctrl = new AccountController();
            ViewResult result = ctrl.Register();

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task Register_POST_Invalid_Model_Returns_View()
        {
            var ctrl = new AccountController(signInMgr);
            var model = new RegisterViewModel();
            ViewResult result = await ctrl.Register(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreSame(model, result.Model);
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

        private async Task<IdentityResult> CreateUserAsync()
        {
            var result = await signInMgr.UserManager.CreateAsync(validUser, "testPassword.01");
            Assert.IsTrue(result.Succeeded);
            return result;
        }
    }
}
