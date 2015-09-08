using System;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Testing.Identity.TestClasses;
using Vigil.Web.Controllers;
using Vigil.Web.Controllers.Results;
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
            // @TODO: Register InMemoryTokenProvider with user manager. This does not work: uman.RegisterTwoFactorProvider("InMemory", new InMemoryTokenProvider());
            var uman = new VigilUserManager(new InMemoryUserStore());
            var authman = new InMemoryAuthenticationManager();

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
            var ctrl = GetAccountController();

            Assert.IsNotNull(ctrl);
            Assert.AreSame(signInManager, ctrl.SignInManager);
            Assert.AreSame(signInManager.UserManager, ctrl.UserManager);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AccountController_Explicit_Constructor_Null_UserManager_Throws_Exception()
        {
            new AccountController(null, null, null);
        }

        [TestMethod]
        public void Login_Without_ReturnUrl_Returns_View_And_No_ViewBag_ReturnUrl()
        {
            var ctrl = GetAccountController();
            var result = ctrl.Login(null);
            Assert.IsNotNull(result);
            Assert.IsNull(ctrl.ViewBag.ReturnUrl);
        }

        [TestMethod]
        public void Login_With_ReturnUrl_Sets_ViewBag_ReturnUrl_And_Returns_View()
        {
            var ctrl = GetAccountController();
            var result = ctrl.Login("/Home");
            Assert.IsNotNull(result);
            Assert.AreEqual("/Home", ctrl.ViewBag.ReturnUrl);
        }

        [TestMethod]
        public async Task Login_POST_Invalid_Model_Returns_View()
        {
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
            await CreateUserAsync();

            ViewResult result = await ctrl.VerifyCode("InMemory", null, false) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(VerifyCodeViewModel));
            Assert.AreEqual("InMemory", (result.Model as VerifyCodeViewModel).Provider);
        }

        /// <summary>Tests the AccountController.VerifyCode method when SignInManager.HasBeenVerifiedAsync returns false.
        /// <remarks>Requires SignInManager.HasBeenVerifiedAsync to return false.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task VerifyCode_Without_Verified_User_Returns_Error_View()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();

            // @TODO: Cause SignInManager.HasBeenVerifiedAsync to return false

            ViewResult result = await ctrl.VerifyCode("InMemory", null, false) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task VerifyCode_POST_Invalid_ModelState_Returns_View()
        {
            var ctrl = GetAccountController();
            var model = new VerifyCodeViewModel();
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.VerifyCode(model) as ViewResult;

            Assert.IsFalse(isModelStateValid);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreSame(model, result.Model);
        }

        /// <summary>Tests the AccountController.VerifyCode method when everything is correct.
        /// <remarks>Requires an IUserTokenProvider registered with the UserManager.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task VerifyCode_POST_Successful_Login_Without_ReturnUrl_Redirect_To_Home()
        {
            var ctrl = GetAccountController();
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

        /// <summary>Tests the AccountController.VerifyCode method when a correct code is used, but the user is locked out.
        /// <remarks>Requires an IUserTokenProvider registered with the UserManager.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task VerifyCode_POST_LockedOut_Returns_Lockout_View()
        {
            var ctrl = GetAccountController();
            validUser.LockoutEnabled = true;
            validUser.LockoutEndDateUtc = DateTime.UtcNow.AddDays(1);
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

        /// <summary>Tests the AccountController.VerifyCode method when an incorrect code is used.
        /// <remarks>Requires an IUserTokenProvider registered with the UserManager.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task VerifyCode_POST_Failure_Returns_View_With_Error()
        {
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();

            ViewResult result = await ctrl.ConfirmEmail(Guid.Empty, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ConfirmEmail_With_NonExistent_UserId_Throws_InvalidOperationException()
        {
            var ctrl = GetAccountController();

            await ctrl.ConfirmEmail(Guid.Empty, "invalidCode");
        }

        /// <summary>Tests the AccountController.ConfirmEmail method when an incorrect code is used.
        /// <remarks>Requires an IUserTokenProvider registered with the UserManager.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task ConfirmEmail_With_Incorrect_Code_Returns_Error_View()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();

            ViewResult result = await ctrl.ConfirmEmail(validUser.Id, "invalidCode") as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        /// <summary>Tests the AccountController.ConfirmEmail method when everything is valid.
        /// <remarks>Requires an IUserTokenProvider registered with the UserManager.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task ConfirmEmail_With_Correct_UserId_And_Code_Returns_ConfirmEmail_View()
        {
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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
            var ctrl = GetAccountController();
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

        /// <summary>Tests the AccountController.ResetPassword method when an incorrect code is used.
        /// <remarks>Requires an IUserTokenProvider registered with the UserManager.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task ResetPassword_POST_Incorrect_Code_Returns_View_With_ModelState_Errors()
        {
            var ctrl = GetAccountController();
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

        /// <summary>Tests the AccountController.ResetPassword method when everything is valid.
        /// <remarks>Requires an IUserTokenProvider registered with the UserManager.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task ResetPassword_POST_Correct_Code_Returns_RedirectToRouteResult_ResetPasswordConfirmation()
        {
            var ctrl = GetAccountController();
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

        [TestMethod]
        public void ResetPasswordConfirmation_Returns_View()
        {
            var ctrl = new AccountController();

            ViewResult result = ctrl.ResetPasswordConfirmation() as ViewResult;

            Assert.IsNotNull(result);
        }

        /// <summary>Tests the AccountController.ExternalLogin with an "InMemory" provider.
        /// <remarks>Requires a UrlHelper on the AccountController.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public void ExternalLogin_Returns_ChallengeResult()
        {
            var ctrl = new AccountController();

            ChallengeResult result = ctrl.ExternalLogin("InMemory", "/Home/Index");

            Assert.AreEqual("InMemory", result.LoginProvider);
            Assert.AreEqual("/Account/ExternalLoginCallback/?ReturnUrl=/Home/Index", result.RedirectUri);
            Assert.AreEqual(Guid.Empty, result.UserId);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendCode_When_No_Users_Verified_Throws_InvalidOperationException()
        {
            var ctrl = GetAccountController();
            await ctrl.SendCode(null, false);
        }

        /// <summary>Tests AccountController.SendCode with an UnverifiedUser, but logged in user.
        /// <remarks>Requires SignInManager.GetVerifiedUserIdAsync to return null without throwing InvalidOperationException because UserId was not found.</remarks>
        /// </summary>
        /// <returns></returns>
        [Ignore]
        [TestMethod]
        public async Task SendCode_UnverifiedUser_Returns_Error_View()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();

            ViewResult result = await ctrl.SendCode(null, false) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task SendCode_For_Verified_User_Returns_View_With_SendCodeViewModel_With_Providers()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();

            ViewResult result = await ctrl.SendCode(null, false) as ViewResult;
            SendCodeViewModel model = result.Model as SendCodeViewModel;

            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.IsNull(model.ReturnUrl);
            Assert.IsFalse(model.RememberMe);
            Assert.AreSame(1, model.Providers.Count);
        }

        [TestMethod]
        public async Task SendCode_POST_Invalid_ModelState_Returns_View()
        {
            var ctrl = new AccountController();
            var model = new SendCodeViewModel();
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.SendCode(model) as ViewResult;

            Assert.IsFalse(isModelStateValid);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task SendCode_POST_Unable_To_SendTwoFactorCode_Returns_Error_View()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();
            var model = new SendCodeViewModel()
            {
                SelectedProvider = "InMemory"
            };
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.SendCode(model) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task SendCode_POST_Code_Sent_Returns_RedirectToAction_VerifyCode()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();
            var model = new SendCodeViewModel()
            {
                SelectedProvider = "InMemory"
            };
            bool isModelStateValid = BindModel(ctrl, model);

            RedirectToRouteResult result = await ctrl.SendCode(model) as RedirectToRouteResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("VerifyCode", result.RouteValues["Action"]);
            Assert.AreEqual("Account", result.RouteValues["Controller"]);
            Assert.AreEqual("Provider", "InMemory");
            Assert.AreEqual("RememberMe", false);
            Assert.IsNull("ReturnUrl");
        }

        [TestMethod]
        public async Task ExternalLoginCallback_With_No_ExternalLoginInfo_Returns_RedirectToAction_To_Login()
        {
            var ctrl = GetAccountController();

            RedirectToRouteResult result = await ctrl.ExternalLoginCallback(null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues["Controller"]);
            Assert.AreEqual("Login", result.RouteValues["Action"]);
        }

        /// <summary>Tests the AccountController.ExternalLoginCallback method when the ExternalSignInAsync issues a failure.
        /// <remarks>Requires AuthenticationManager.GetExternalLoginInfoAsync to return an ExternalLoginInfo.
        /// Requires SignInManager.ExternalSignInAsync to return SignInStatus.Failure.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task EternalLoginCallback_Failure_Returns_ExternalLoginConfirmation_View_With_User_Email()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();

            // @TODO: Cause AuthenticationManager.GetExternalLoginInfoAsync to return value
            // @TODO: Cause SignInManager.ExternalSignInAsync to return Failure
            ViewResult result = await ctrl.ExternalLoginCallback(null) as ViewResult;
            ExternalLoginConfirmationViewModel model = result.Model as ExternalLoginConfirmationViewModel;

            Assert.IsNotNull(result);
            Assert.AreEqual("ExternalLoginConfirmation", result.ViewName);
            Assert.IsNotNull(model);
            Assert.AreEqual("valid@example.com", model.Email);
        }

        /// <summary>Tests the AccountController.ExternalLoginCallback method when SignInManager.ExternalSignInAsync returns SignInStatus.RequiresVerification.
        /// <remarks>Requires AuthenticationManager.GetExternalLoginInfoAsync to return an ExternalLoginInfo.
        /// Requires SignInManager.ExternalSignInAsync to return SignInStatus.RequiresVerification.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task ExternalLoginCallback_RequiresVerification_Returns_RedirectToRouteResult_To_SendCode()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();

            // @TODO: Cause AuthenticationManager.GetExternalLoginInfoAsync to return value
            // @TODO: Cause SignInManager.ExternalSignInAsync to return RequiresVerification
            RedirectToRouteResult result = await ctrl.ExternalLoginCallback(null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues["Controller"]);
            Assert.AreEqual("SendCode", result.RouteValues["Action"]);
            Assert.IsNull(result.RouteValues["ReturnUrl"]);
            Assert.AreEqual("RememberMe", false);
        }

        /// <summary>Tests the AccountController.ExternalLoginCallback method when SignInManager.ExternalSignInAsync returns SignInStatus.LockedOut.
        /// <remarks>Requires AuthenticationManager.GetExternalLoginInfoAsync to return an ExternalLoginInfo.
        /// Requires SignInManager.ExternalSignInAsync to return SignInStatus.LockedOut.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task ExternalLoginCallback_LockedOut_Returns_Lockout_View()
        {
            var ctrl = GetAccountController();
            validUser.LockoutEnabled = true;
            validUser.LockoutEndDateUtc = DateTime.UtcNow.AddDays(1);
            await CreateUserAsync();

            // @TODO: Cause AuthenticationManager.GetExternalLoginInfoAsync to return value
            // @TODO: Cause SignInManager.ExternalSignInAsync to return LockedOut
            ViewResult result = await ctrl.ExternalLoginCallback(null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Lockout", result.ViewName);
        }

        /// <summary>Tests the AccountController.ExternalLoginCallback method when SignInManager.ExternalSignInAsync returns SignInStatus.Success.
        /// <remarks>Requires AuthenticationManager.GetExternalLoginInfoAsync to return an ExternalLoginInfo.
        /// Requires SignInManager.ExternalSignInAsync to return SignInStatus.Success.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task ExternalLoginCallback_Success_Without_ReturnUrl_Returns_RedirectToRouteResult_To_Home()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();

            // @TODO: Cause AuthenticationManager.GetExternalLoginInfoAsync to return value
            // @TODO: Cause SignInManager.ExternalSignInAsync to return Success
            RedirectToRouteResult result = await ctrl.ExternalLoginCallback(null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues["Controller"]);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
        }

        [TestMethod]
        public async Task ExternalLoginConfirmation_POST_User_Not_Authenticated_Returns_RedirectToRouteResult_To_Manage()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();
            signInManager.AuthenticationManager.SignOut();

            RedirectToRouteResult result = await ctrl.ExternalLoginConfirmation(null, null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Manage", result.RouteValues["Controller"]);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
        }

        [TestMethod]
        public async Task ExternalLoginConfirmation_POST_Invalid_Model_Returns_View_With_Model()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel();
            bool isModelStateValid = BindModel(ctrl, model);

            ViewResult result = await ctrl.ExternalLoginConfirmation(model, null) as ViewResult;

            Assert.IsFalse(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task ExternalLoginConfirmation_POST_Valid_Model_No_External_Login_Info_Returns_ExternalLoginFailure_View()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel()
            {
                Email = validUser.Email
            };
            bool isModelStateValid = BindModel(ctrl, model);

            // @TODO: Cause AuthenticationManager.GetExternalLoginInfoAsync to return null
            ViewResult result = await ctrl.ExternalLoginConfirmation(model, null) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.AreEqual("ExternalLoginFailure", result.ViewName);
        }

        [TestMethod]
        public async Task ExternalLoginConfirmation_POST_Valid_Model_Create_User_Failure_Invalidates_ModelState_And_Returns_View()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel()
            {
                Email = validUser.Email
            };
            bool isModelStateValid = BindModel(ctrl, model);

            // @TODO: Cause AuthenticationManager.GetExternalLoginInfoAsync to return value
            ViewResult result = await ctrl.ExternalLoginConfirmation(model, null) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(result);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.AreNotEqual("ExternalLoginFailure", result.ViewName);
            Assert.AreSame(model, result.Model);
        }

        /// <summary>Tests the AccountController.ExternalLoginConfirmation method when AuthenticationManager.GetExternalLoginInfoAsync returns an ExternalLoginInfo
        /// and UserManager.AddLoginAsync returns a failure.
        /// <remarks>Requires AuthenticationManager.GetExternalLoginInfoAsync to return an ExternalLoginInfo.
        /// Requires UserManager.AddLoginAsync to return a failure.
        /// </remarks>
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task ExternalLoginConfirmation_POST_AddLoginAsync_Failure_Invalidates_ModelState_And_Returns_View()
        {
            var ctrl = GetAccountController();
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel()
            {
                Email = "external@example.com"
            };
            bool isModelStateValid = BindModel(ctrl, model);

            // @TODO: Cause AuthenticationManager.GetExternalLoginInfoAsync to return value
            // @TODO: Cuase UserManager.AddLoginAsync to fail using newly created user
            ViewResult result = await ctrl.ExternalLoginConfirmation(model, null) as ViewResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(signInManager.UserManager.FindByEmail("external@example.com"));
            Assert.IsNotNull(result);
            Assert.IsFalse(ctrl.ModelState.IsValid);
            Assert.AreNotEqual("ExternalLoginFailure", result.ViewName);
            Assert.AreSame(model, result.Model);
        }

        [TestMethod]
        public async Task ExternalLoginConfirmation_POST_Successful_User_Creation_And_Login_And_SignIn_Returns_RedirectToRouteResult_Home()
        {
            var ctrl = GetAccountController();
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel()
            {
                Email = "external@example.com"
            };
            bool isModelStateValid = BindModel(ctrl, model);

            // @TODO: Cause AuthenticationManager.GetExternalLoginInfoAsync to return value
            // @TODO: Cuase UserManager.AddLoginAsync to succeed using newly created user
            RedirectToRouteResult result = await ctrl.ExternalLoginConfirmation(model, null) as RedirectToRouteResult;

            Assert.IsTrue(isModelStateValid);
            Assert.IsNotNull(signInManager.UserManager.FindByEmail("external@example.com"));
            Assert.IsNotNull(result);
            Assert.IsTrue(ctrl.ModelState.IsValid);
            Assert.AreEqual("Home", result.RouteValues["Controller"]);
            Assert.AreEqual("Index", result.RouteValues["Index"]);
        }

        [TestMethod]
        public void LogOff_With_No_User_Returns_RedirectToRouteResult_To_Home()
        {
            var ctrl = GetAccountController();

            RedirectToRouteResult result = ctrl.LogOff() as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Home", result.RouteValues["Controller"]);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
        }

        [TestMethod]
        public async Task LogOff_LogsOff_User_And_Returns_RedirectToRouteResult_To_Home()
        {
            var ctrl = GetAccountController();
            await CreateUserAsync();
            await signInManager.SignInAsync(validUser, false, false);
            ClaimsPrincipal signedInUser = signInManager.AuthenticationManager.User;

            RedirectToRouteResult result = ctrl.LogOff() as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(signedInUser);
            Assert.IsNull(signInManager.AuthenticationManager.User);
            Assert.AreEqual("Home", result.RouteValues["Controller"]);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
        }

        [TestMethod]
        public void ExternalLoginFailure_Returns_View()
        {
            var ctrl = new AccountController();

            ViewResult result = ctrl.ExternalLoginFailure() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreNotEqual("Error", result.ViewName);
        }

        private async Task<IdentityResult> CreateUserAsync(string password = "testPassword.01")
        {
            var result = await signInManager.UserManager.CreateAsync(validUser, password);
            Assert.IsTrue(result.Succeeded);
            return result;
        }

        private AccountController GetAccountController()
        {
            return new AccountController(signInManager, signInManager.UserManager as VigilUserManager, signInManager.AuthenticationManager);
        }
    }
}
