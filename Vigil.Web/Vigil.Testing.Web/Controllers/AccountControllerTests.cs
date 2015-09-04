using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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
    public class AccountControllerTests : ControllerTests
    {
        private VigilSignInManager signInManager;
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
            uman.RegisterTwoFactorProvider("EmailConfirmation", tokenProvider);
            signInManager = new VigilSignInManager(uman, authman);
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
            var ctrl = new AccountController(signInManager);

            Assert.IsNotNull(ctrl);
            Assert.AreSame(signInManager, ctrl.SignInManager);
            Assert.AreSame(signInManager.UserManager, ctrl.UserManager);
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
            var ctrl = new AccountController(signInManager);
            var result = ctrl.Login(null);
            Assert.IsNotNull(result);
            Assert.IsNull(ctrl.ViewBag.ReturnUrl);
        }

        [TestMethod]
        public void Login_With_ReturnUrl_Sets_ViewBag_ReturnUrl_And_Returns_View()
        {
            var ctrl = new AccountController(signInManager);
            var result = ctrl.Login("/Home");
            Assert.IsNotNull(result);
            Assert.AreEqual("/Home", ctrl.ViewBag.ReturnUrl);
        }

        [TestMethod]
        public async Task Login_POST_Invalid_Model_Returns_View()
        {
            var ctrl = new AccountController(signInManager);
            var model = new LoginViewModel()
            {
                Email = "notevalidemail",
                Password = "testPassword.01",
                RememberMe = false
            };
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.Login(model, null) as ViewResult;

            Assert.IsFalse(isModelStateValid);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task Login_POST_Successful_Login_Without_ReturnUrl_Returns_Redirect_To_Home()
        {
            var ctrl = new AccountController(signInManager);
            await CreateUserAsync();

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            bool isModelStateValid = BindModel(ctrl, model);

            RedirectToRouteResult result = await ctrl.Login(model, null) as RedirectToRouteResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.AreEqual("Home", result.RouteValues["Controller"]);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
        }

        [TestMethod]
        public async Task Login_POST_LockedOut_Returns_Lockout_View()
        {
            var ctrl = new AccountController(signInManager);
            validUser.LockoutEnabled = true;
            validUser.LockoutEndDateUtc = DateTime.UtcNow.AddDays(1);

            await CreateUserAsync();
            Assert.IsTrue(await signInManager.UserManager.IsLockedOutAsync(validUser.Id));

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.Login(model, null) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("Lockout", result.ViewName);
        }

        [TestMethod]
        public async Task Login_POST_RequiresVerification_Redirects_To_SendCode_Action()
        {
            var ctrl = new AccountController(signInManager);
            validUser.TwoFactorEnabled = true;
            await CreateUserAsync();

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            bool isModelStateValid = BindModel(ctrl, model);

            RedirectToRouteResult result = await ctrl.Login(model, null) as RedirectToRouteResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues["ReturnUrl"]);
            Assert.AreEqual(model.RememberMe, result.RouteValues["RememberMe"]);
            Assert.AreEqual("SendCode", result.RouteValues["Action"]);
        }

        [TestMethod]
        public async Task Login_POST_RequiresVerification_Redirects_To_SendCode_Action_And_Retains_ReturnUrl()
        {
            var ctrl = new AccountController(signInManager);
            validUser.TwoFactorEnabled = true;
            await CreateUserAsync();

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = true
            };
            bool isModelStateValid = BindModel(ctrl, model);

            RedirectToRouteResult result = await ctrl.Login(model, "/Home/Index") as RedirectToRouteResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("/Home/Index", result.RouteValues["ReturnUrl"]);
            Assert.AreEqual(model.RememberMe, result.RouteValues["RememberMe"]);
            Assert.AreEqual("SendCode", result.RouteValues["Action"]);
        }

        [TestMethod]
        public async Task Login_POST_Using_Incorrect_Password_Returns_View_With_Error()
        {
            var ctrl = new AccountController(signInManager);
            await CreateUserAsync();

            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "incorrectPassword",
                RememberMe = true
            };
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.Login(model, null) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.AreEqual("Invalid login attempt.", ctrl.ModelState[""].Errors[0].ErrorMessage);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task Login_POST_Using_Incorrect_Credentials_Returns_View_With_Error()
        {
            var ctrl = new AccountController(signInManager);
            var model = new LoginViewModel()
            {
                Email = "invalid@example.com",
                Password = "incorrectPassword",
                RememberMe = true
            };
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.Login(model, null) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.AreEqual("Invalid login attempt.", ctrl.ModelState[""].Errors[0].ErrorMessage);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task VerifyCode_With_Verified_User_Returns_ViewResult_With_VerifyCodeViewModel()
        {
            var ctrl = new AccountController(signInManager);
            await CreateUserAsync();

            ViewResult result = await ctrl.VerifyCode("InMemory", null, false) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(VerifyCodeViewModel));
            Assert.AreEqual("InMemory", (result.Model as VerifyCodeViewModel).Provider);
        }

        [TestMethod]
        public async Task VerifyCode_Without_Verified_User_Returns_Error_View()
        {
            var ctrl = new AccountController(signInManager);
            await CreateUserAsync();

            // somehow get signInMgr.HasBeenVerifiedAsync() to return false

            ViewResult result = await ctrl.VerifyCode("InMemory", null, false) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task VerifyCode_POST_Invalid_ModelState_Returns_View()
        {
            var ctrl = new AccountController(signInManager);
            var model = new VerifyCodeViewModel();
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.VerifyCode(model) as ViewResult;

            Assert.IsFalse(isModelStateValid);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task VerifyCode_POST_Successful_Login_Without_ReturnUrl_Redirect_To_Home()
        {
            var ctrl = new AccountController(signInManager);
            await CreateUserAsync();
            var code = signInManager.UserManager.GenerateTwoFactorToken(validUser.Id, "InMemory");
            var model = new VerifyCodeViewModel
            {
                Provider = "InMemory",
                Code = code
            };
            bool isModelStateValid = BindModel(ctrl, model);

            RedirectToRouteResult result = await ctrl.VerifyCode(model) as RedirectToRouteResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.AreEqual("Home", result.RouteValues["Controller"]);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
        }

        [TestMethod]
        public async Task VerifyCode_POST_LockedOut_Returns_Lockout_View()
        {
            var ctrl = new AccountController(signInManager);
            validUser.LockoutEnabled = true;
            validUser.LockoutEndDateUtc = DateTime.UtcNow.AddDays(-1);
            await CreateUserAsync();

            var code = signInManager.UserManager.GenerateTwoFactorToken(validUser.Id, "InMemory");
            var model = new VerifyCodeViewModel
            {
                Provider = "InMemory",
                Code = code
            };
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.VerifyCode(model) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("Lockout", result.ViewName);
        }

        [TestMethod]
        public async Task VerifyCode_POST_Failure_Returns_View_With_Error()
        {
            var ctrl = new AccountController(signInManager);
            await CreateUserAsync();
            var code = signInManager.UserManager.GenerateTwoFactorToken(validUser.Id, "InMemory");
            var model = new VerifyCodeViewModel
            {
                Provider = "InMemory",
                Code = "InvalidCode"
            };
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.VerifyCode(model) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public void Register_Returns_View()
        {
            var ctrl = new AccountController();
            ViewResult result = ctrl.Register();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Register_POST_Invalid_Model_Returns_View()
        {
            var ctrl = new AccountController(signInManager);
            var model = new RegisterViewModel();
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.Register(model) as ViewResult;

            Assert.IsFalse(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task ConfirmEmail_Without_Code_Returns_Error_View()
        {
            var ctrl = new AccountController(signInManager);

            ViewResult result = await ctrl.ConfirmEmail(Guid.Empty, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task ConfirmEmail_With_Incorrect_UserId_And_Code_Returns_Error_View()
        {
            var ctrl = new AccountController(signInManager);

            ViewResult result = await ctrl.ConfirmEmail(Guid.Empty, "invalidCode") as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task ConfirmEmail_With_Correct_UserId_And_Code_Returns_ConfirmEmail_View()
        {
            var ctrl = new AccountController(signInManager);
            await CreateUserAsync();
            string code = await signInManager.UserManager.GenerateEmailConfirmationTokenAsync(validUser.Id);

            ViewResult result = await ctrl.ConfirmEmail(validUser.Id, code) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ConfirmEmail", result.ViewName);
        }

        [TestMethod]
        public void ForgotPassword_Returns_View()
        {
            var ctrl = new AccountController();

            ViewResult result = ctrl.ForgotPassword() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ForgotPassword_POST_Invalid_Model_Returns_View_With_Model()
        {
            var ctrl = new AccountController(signInManager);
            var model = new ForgotPasswordViewModel();
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.ForgotPassword(model) as ViewResult;

            Assert.IsFalse(isModelStateValid);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task ForgotPassword_POST_User_Does_Not_Exist_Returns_ForgotPasswordConfirmation_View()
        {
            var ctrl = new AccountController(signInManager);
            var model = new ForgotPasswordViewModel
            {
                Email = "notfound@example.com"
            };
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.ForgotPassword(model) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("ForgotPasswordConfirmation", result.ViewName);
        }

        [TestMethod]
        public async Task ForgotPassword_POST_Email_Not_Confirmed_Returns_View_With_Model()
        {
            var ctrl = new AccountController(signInManager);
            validUser.EmailConfirmed = false;
            await CreateUserAsync();
            var model = new ForgotPasswordViewModel
            {
                Email = validUser.Email
            };
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.ForgotPassword(model) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task ForgotPassword_POST_Email_Exists_And_Confirmed_Returns_ForgotPasswordConfirmation_View()
        {
            var ctrl = new AccountController(signInManager);
            validUser.EmailConfirmed = true;
            await CreateUserAsync();
            var model = new ForgotPasswordViewModel
            {
                Email = validUser.Email
            };
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.ForgotPassword(model) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.AreEqual("ForgotPasswordConfirmation", result.ViewName);
        }

        [TestMethod]
        public void ForgotPasswordConfirmation_Returns_View()
        {
            var ctrl = new AccountController();

            ViewResult result = ctrl.ForgotPasswordConfirmation() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ResetPassword_Null_Code_Returns_Error_View()
        {
            var ctrl = new AccountController();

            ViewResult result = ctrl.ResetPassword((string)null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public void ResetPassword_With_Code_Returns_View()
        {
            var ctrl = new AccountController();

            ViewResult result = ctrl.ResetPassword("code") as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreNotEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task ResetPassword_POST_Invalid_Model_Returns_View_With_Model()
        {
            var ctrl = new AccountController();
            var model = new ResetPasswordViewModel();
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.ResetPassword(model) as ViewResult;

            Assert.IsFalse(isModelStateValid);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task ResetPassword_POST_User_Not_Found_Returns_RedirectToRouteResult_ResetPasswordConfirmation()
        {
            var ctrl = new AccountController(signInManager);
            var model = new ResetPasswordViewModel()
            {
                Code = "invalid",
                Password = "newPassword.01",
                ConfirmPassword = "newPassword.01",
                Email = "notvalid@example.com"
            };
            var isModelStateValid = BindModel(ctrl, model);

            RedirectToRouteResult result = await ctrl.ResetPassword(model) as RedirectToRouteResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("ResetPasswordConfirmation", result.RouteValues["Action"]);
            Assert.AreEqual("Account", result.RouteValues["Controller"]);
        }

        [TestMethod]
        public async Task ResetPassword_POST_Incorrect_Code_Returns_View_With_ModelState_Errors()
        {
            var ctrl = new AccountController(signInManager);
            validUser.EmailConfirmed = true;
            await CreateUserAsync();
            var model = new ResetPasswordViewModel()
            {
                Code = "invalid",
                ConfirmPassword = "newPassword.01",
                Password = "newPassword.01",
                Email = validUser.Email
            };
            var isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.ResetPassword(model) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ResetPassword_POST_Correct_Code_Returns_RedirectToRouteResult_ResetPasswordConfirmation()
        {
            var ctrl = new AccountController(signInManager);
            validUser.EmailConfirmed = true;
            await CreateUserAsync();
            var model = new ResetPasswordViewModel()
            {
                Code = await signInManager.UserManager.GeneratePasswordResetTokenAsync(validUser.Id),
                ConfirmPassword = "newPassword.01",
                Password = "newPassword.01",
                Email = validUser.Email
            };
            var isModelStateValid = BindModel(ctrl, model);

            RedirectToRouteResult result = await ctrl.ResetPassword(model) as RedirectToRouteResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("ResetPasswordConfirmation", result.RouteValues["Action"]);
            Assert.AreEqual("Account", result.RouteValues["Controller"]);
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
            var result = await signInManager.UserManager.CreateAsync(validUser, "testPassword.01");
            Assert.IsTrue(result.Succeeded);
            return result;
        }
    }
}
