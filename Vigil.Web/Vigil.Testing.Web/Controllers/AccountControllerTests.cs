using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Moq;
using Moq.Protected;
using Vigil.Data.Core.Identity;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Testing.Identity;
using Vigil.Web.Controllers;
using Vigil.Web.Controllers.Results;
using Vigil.Web.Models;
using Xunit;

namespace Vigil.Testing.Web.Controllers
{
    [System.Diagnostics.Contracts.ContractVerification(false)]
    public partial class AccountControllerTests : IDisposable
    {
        private readonly Mock<IPrincipal> MockUser;
        private readonly Mock<VigilUserManager> MockUserManager;
        private readonly Mock<IVigilSignInManager> MockSignInManager;
        private readonly Mock<IAuthenticationManager> MockAuthenticationManager = new Mock<IAuthenticationManager>();
        private readonly AccountController Controller;

        public AccountControllerTests()
        {
            MockUser = new Mock<IPrincipal>();
            MockUserManager = new Mock<VigilUserManager>(Mock.Of<IUserStore<VigilUser, Guid>>());
            MockAuthenticationManager = new Mock<IAuthenticationManager>();
            MockSignInManager = new Mock<IVigilSignInManager>();
            MockSignInManager.SetupGet(msim => msim.AuthenticationManager).Returns(MockAuthenticationManager.Object);
            MockSignInManager.SetupGet(msim => msim.UserManager).Returns(MockUserManager.Object);

            Controller = new AccountController(MockUserManager.Object, MockSignInManager.Object, MockAuthenticationManager.Object);

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.SetupGet(ctx => ctx.User).Returns(MockUser.Object);
            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.SetupGet(con => con.HttpContext).Returns(mockHttpContext.Object);

            Controller.ControllerContext = mockControllerContext.Object;
        }

        [Fact]
        public void AccountController_Default_Constructor()
        {
            using (var ctrl = new AccountController())
            {
                Assert.NotNull(ctrl);
            }
        }

        [Fact]
        public void AccountController_Explicit_Constructor()
        {
            Assert.NotNull(Controller);
            Assert.Same(MockSignInManager.Object, Controller.SignInManager);
            Assert.Same(MockUserManager.Object, Controller.UserManager);
        }

        [Fact]
        public void Login_Without_ReturnUrl_Returns_View_And_No_ViewBag_ReturnUrl()
        {
            var result = Controller.Login(null);
            Assert.NotNull(result);
            Assert.Null(Controller.ViewBag.ReturnUrl);
        }

        [Fact]
        public void Login_With_ReturnUrl_Sets_ViewBag_ReturnUrl_And_Returns_View()
        {
            var result = Controller.Login("/Home");
            Assert.NotNull(result);
            Assert.Equal("/Home", Controller.ViewBag.ReturnUrl);
        }

        [Fact]
        public async Task Login_POST_Invalid_Model_Returns_View()
        {
            var model = new LoginViewModel()
            {
                Email = "notevalidemail",
                Password = "testPassword.01",
                RememberMe = false
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.Login(model, null) as ViewResult;

            Assert.False(isModelStateValid);
            Assert.False(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task Login_POST_Successful_Login_Without_ReturnUrl_Returns_RedirectToRouteResult_To_Home()
        {
            MockSignInManager.Setup(msim => msim.PasswordSignInAsync(It.Is<string>(s => s == "valid@example.com"),
                                                                    It.Is<string>(s => s == "testPassword.01"),
                                                                    It.IsAny<bool>(), It.IsAny<bool>()))
                             .ReturnsAsync(SignInStatus.Success);
            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            bool isModelStateValid = Controller.BindModel(model);

            RedirectToRouteResult result = await Controller.Login(model, null) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.True(Controller.ModelState.IsValid);
            Assert.Equal("Home", result.RouteValues["Controller"]);
            Assert.Equal("Index", result.RouteValues["Action"]);
        }

        [Fact]
        public async Task Login_POST_Successful_Login_Without_Local_Url_Returns_RedirectToRouteResult_To_Home()
        {
            MockSignInManager.Setup(msim => msim.PasswordSignInAsync(It.Is<string>(s => s == "valid@example.com"),
                                                                    It.Is<string>(s => s == "testPassword.01"),
                                                                    It.IsAny<bool>(), It.IsAny<bool>()))
                             .ReturnsAsync(SignInStatus.Success);
            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(muh => muh.IsLocalUrl(It.Is<string>(s => s == "notlocal.example.com"))).Returns(false);
            Controller.Url = mockUrlHelper.Object;
            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            bool isModelStateValid = Controller.BindModel(model);

            RedirectToRouteResult result = await Controller.Login(model, "notlocal.example.com") as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.True(Controller.ModelState.IsValid);
            Assert.Equal("Home", result.RouteValues["Controller"]);
            Assert.Equal("Index", result.RouteValues["Action"]);
        }

        [Fact]
        public async Task Login_POST_Successful_Login_With_LocalUrl_Returns_RedirectResult_To_Url()
        {
            MockSignInManager.Setup(msim => msim.PasswordSignInAsync(It.Is<string>(s => s == "valid@example.com"),
                                                                    It.Is<string>(s => s == "testPassword.01"),
                                                                    It.IsAny<bool>(), It.IsAny<bool>()))
                             .ReturnsAsync(SignInStatus.Success);
            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(muh => muh.IsLocalUrl(It.Is<string>(s => s == "/Account/Login"))).Returns(true);
            Controller.Url = mockUrlHelper.Object;
            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            bool isModelStateValid = Controller.BindModel(model);

            RedirectResult result = await Controller.Login(model, "/Account/Login") as RedirectResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.True(Controller.ModelState.IsValid);
            Assert.Equal("/Account/Login", result.Url);
        }
        [Fact]
        public async Task Login_POST_LockedOut_Returns_Lockout_View()
        {
            MockSignInManager.Setup(msim => msim.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                             .ReturnsAsync(SignInStatus.LockedOut);
            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.Login(model, null) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.True(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("Lockout", result.ViewName);
        }

        [Fact]
        public async Task Login_POST_RequiresVerification_Redirects_To_SendCode_Action()
        {
            MockSignInManager.Setup(msim => msim.PasswordSignInAsync(
                                    It.Is<string>(s => s == "valid@example.com"),
                                    It.Is<string>(s => s == "testPassword.01"),
                                    It.IsAny<bool>(),
                                    It.IsAny<bool>()))
                 .ReturnsAsync(SignInStatus.RequiresVerification);
            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = false
            };
            bool isModelStateValid = Controller.BindModel(model);

            RedirectToRouteResult result = await Controller.Login(model, null) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.True(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Null(result.RouteValues["ReturnUrl"]);
            Assert.Equal(model.RememberMe, result.RouteValues["RememberMe"]);
            Assert.Equal("SendCode", result.RouteValues["Action"]);
        }

        [Fact]
        public async Task Login_POST_RequiresVerification_Redirects_To_SendCode_Action_And_Retains_ReturnUrl()
        {
            MockSignInManager.Setup(msim => msim.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                 .ReturnsAsync(SignInStatus.RequiresVerification);
            var model = new LoginViewModel()
            {
                Email = "valid@example.com",
                Password = "testPassword.01",
                RememberMe = true
            };
            bool isModelStateValid = Controller.BindModel(model);

            RedirectToRouteResult result = await Controller.Login(model, "/Home/Index") as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.True(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("/Home/Index", result.RouteValues["ReturnUrl"]);
            Assert.Equal(model.RememberMe, result.RouteValues["RememberMe"]);
            Assert.Equal("SendCode", result.RouteValues["Action"]);
        }

        [Fact]
        public async Task Login_POST_Using_Incorrect_Credentials_Returns_View_With_Error()
        {
            MockSignInManager.Setup(msim => msim.PasswordSignInAsync(It.Is<string>(s => s == "invalid@example.com"),
                                                                     It.Is<string>(s => s == "incorrectPassword"),
                                                                     It.IsAny<bool>(),
                                                                     It.IsAny<bool>()))
                             .ReturnsAsync(SignInStatus.Failure);
            var model = new LoginViewModel()
            {
                Email = "invalid@example.com",
                Password = "incorrectPassword",
                RememberMe = true
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.Login(model, null) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.False(Controller.ModelState.IsValid);
            Assert.Equal("Invalid login attempt.", Controller.ModelState[""].Errors[0].ErrorMessage);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task VerifyCode_With_Verified_User_Returns_ViewResult_With_VerifyCodeViewModel()
        {
            MockSignInManager.Setup(msim => msim.HasBeenVerifiedAsync())
                 .ReturnsAsync(true);

            ViewResult result = await Controller.VerifyCode("InMemory", null, false) as ViewResult;

            Assert.NotNull(result);
            Assert.IsType<VerifyCodeViewModel>(result.Model);
            Assert.Equal("InMemory", (result.Model as VerifyCodeViewModel).Provider);
        }

        [Fact]
        public async Task VerifyCode_Without_Verified_User_Returns_Error_View()
        {
            MockSignInManager.Setup(msim => msim.HasBeenVerifiedAsync())
                 .ReturnsAsync(false);


            ViewResult result = await Controller.VerifyCode("InMemory", null, false) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task VerifyCode_POST_Invalid_ModelState_Returns_View()
        {
            var model = new VerifyCodeViewModel();
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.VerifyCode(model) as ViewResult;

            Assert.False(isModelStateValid);
            Assert.False(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task VerifyCode_POST_Successful_Login_Without_ReturnUrl_Redirect_To_Home()
        {
            MockSignInManager.Setup(msim => msim.TwoFactorSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                 .ReturnsAsync(SignInStatus.Success);
            var model = new VerifyCodeViewModel
            {
                Provider = "InMemory",
                Code = "correctCode"
            };
            bool isModelStateValid = Controller.BindModel(model);

            RedirectToRouteResult result = await Controller.VerifyCode(model) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.True(Controller.ModelState.IsValid);
            Assert.Equal("Home", result.RouteValues["Controller"]);
            Assert.Equal("Index", result.RouteValues["Action"]);
        }

        [Fact]
        public async Task VerifyCode_POST_LockedOut_Returns_Lockout_View()
        {
            MockSignInManager.Setup(msim => msim.TwoFactorSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                 .ReturnsAsync(SignInStatus.LockedOut);
            var model = new VerifyCodeViewModel
            {
                Provider = "InMemory",
                Code = "anyCode"
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.VerifyCode(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.Equal("Lockout", result.ViewName);
        }

        [Fact]
        public async Task VerifyCode_POST_Failure_Returns_View_With_Error()
        {
            MockSignInManager.Setup(msim => msim.TwoFactorSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                             .ReturnsAsync(SignInStatus.Failure);
            var model = new VerifyCodeViewModel
            {
                Provider = "InMemory",
                Code = "InvalidCode"
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.VerifyCode(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.False(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public void Register_Returns_View()
        {
            using (var ctrl = new AccountController())
            {
                ViewResult result = ctrl.Register();

                Assert.NotNull(result);
            }
        }

        [Fact]
        public async Task Register_POST_Invalid_Model_Returns_View()
        {
            var model = new RegisterViewModel();
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.Register(model) as ViewResult;

            Assert.False(isModelStateValid);
            Assert.NotNull(result);
            Assert.False(Controller.ModelState.IsValid);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task Register_POST_Valid_Model_Failed_CreateAsync_Returns_View_With_Errors()
        {
            MockUserManager.Setup(mum => mum.CreateAsync(It.IsAny<VigilUser>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Failed("Failed."));
            var model = new RegisterViewModel()
            {
                Email = "valid@example.com",
                Password = "password.01",
                ConfirmPassword = "password.01"
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.Register(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.False(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task Register_POST_Valid_Model_Succeeded_CreateAsync_Returns_RedirectToAction_Index()
        {
            MockUserManager.Setup(mum => mum.CreateAsync(It.IsAny<VigilUser>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);
            MockSignInManager.Setup(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()))
                           .Returns(Task.FromResult(IdentityResult.Success));
            var model = new RegisterViewModel()
            {
                Email = "valid@example.com",
                Password = "password.01",
                ConfirmPassword = "password.01"
            };
            bool isModelStateValid = Controller.BindModel(model);

            RedirectToRouteResult result = await Controller.Register(model) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.True(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("Home", result.RouteValues["Controller"]);
            Assert.Equal("Index", result.RouteValues["Action"]);
        }

        [Fact]
        public async Task ConfirmEmail_Without_Code_Returns_Error_View()
        {
            ViewResult result = await Controller.ConfirmEmail(Guid.Empty, null) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmail_With_Incorrect_Code_Returns_Error_View()
        {
            MockUserManager.Setup(mum => mum.ConfirmEmailAsync(It.IsAny<Guid>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Failed());

            ViewResult result = await Controller.ConfirmEmail(Guid.NewGuid(), "invalidCode") as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmail_With_Correct_UserId_And_Code_Returns_ConfirmEmail_View()
        {
            MockUserManager.Setup(mum => mum.ConfirmEmailAsync(It.IsAny<Guid>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Success);

            ViewResult result = await Controller.ConfirmEmail(Guid.NewGuid(), "correctCode") as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("ConfirmEmail", result.ViewName);
        }

        [Fact]
        public void ForgotPassword_Returns_View()
        {
            ViewResult result = Controller.ForgotPassword() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ForgotPassword_POST_Invalid_Model_Returns_View_With_Model()
        {
            var model = new ForgotPasswordViewModel();
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.ForgotPassword(model) as ViewResult;

            Assert.False(isModelStateValid);
            Assert.False(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task ForgotPassword_POST_User_Does_Not_Exist_Returns_ForgotPasswordConfirmation_View()
        {
            MockUserManager.Setup(mum => mum.FindByNameAsync(It.Is<string>(s => s == "notfound@example.com")))
                           .ReturnsAsync(null);
            var model = new ForgotPasswordViewModel
            {
                Email = "notfound@example.com"
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.ForgotPassword(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.Equal("ForgotPasswordConfirmation", result.ViewName);
        }

        [Fact]
        public async Task ForgotPassword_POST_Email_Not_Confirmed_Returns_View_With_Model()
        {
            var user = new VigilUser();
            MockUserManager.Setup(mum => mum.FindByEmailAsync(It.Is<string>(s => s == "confirmed@example.com")))
                           .ReturnsAsync(user);
            MockUserManager.Setup(mum => mum.IsEmailConfirmedAsync(It.Is<Guid>(g => g.Equals(user.Id))))
                           .ReturnsAsync(true);
            var model = new ForgotPasswordViewModel
            {
                Email = "confirmed@example.com"
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.ForgotPassword(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task ForgotPassword_POST_User_Found_By_Email_Exists_And_NotConfirmed_Returns_ForgotPasswordConfirmation_View()
        {
            var user = new VigilUser();
            MockUserManager.Setup(mum => mum.FindByEmailAsync(It.Is<string>(s => s == "notconfirmed@example.com")))
                           .ReturnsAsync(user);
            MockUserManager.Setup(mum => mum.IsEmailConfirmedAsync(It.Is<Guid>(g => g.Equals(user.Id))))
                           .ReturnsAsync(false);
            var model = new ForgotPasswordViewModel
            {
                Email = "notconfirmed@example.com"
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.ForgotPassword(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.True(Controller.ModelState.IsValid);
            Assert.Equal("ForgotPasswordConfirmation", result.ViewName);
        }

        [Fact]
        public void ForgotPasswordConfirmation_Returns_View()
        {
            ViewResult result = Controller.ForgotPasswordConfirmation() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void ResetPassword_Null_Code_Returns_Error_View()
        {
            ViewResult result = Controller.ResetPassword((string)null) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public void ResetPassword_With_Code_Returns_View()
        {
            ViewResult result = Controller.ResetPassword("code") as ViewResult;

            Assert.NotNull(result);
            Assert.NotEqual("Error", result.ViewName);
        }

        [Fact]
        public async Task ResetPassword_POST_Invalid_Model_Returns_View_With_Model()
        {
            var model = new ResetPasswordViewModel();
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.ResetPassword(model) as ViewResult;

            Assert.False(isModelStateValid);
            Assert.False(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task ResetPassword_POST_User_Not_Found_Returns_RedirectToRouteResult_ResetPasswordConfirmation()
        {
            var model = new ResetPasswordViewModel()
{
    Code = "invalid",
    Password = "newPassword.01",
    ConfirmPassword = "newPassword.01",
    Email = "notvalid@example.com"
};
            var isModelStateValid = Controller.BindModel(model);

            RedirectToRouteResult result = await Controller.ResetPassword(model) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.True(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("ResetPasswordConfirmation", result.RouteValues["Action"]);
            Assert.Equal("Account", result.RouteValues["Controller"]);
        }

        [Fact]
        public async Task ResetPassword_POST_Incorrect_Code_Returns_View_With_ModelState_Errors()
        {
            MockUserManager.Setup(mum => mum.FindByNameAsync(It.IsAny<string>()))
                           .ReturnsAsync(new VigilUser());
            MockUserManager.Setup(mum => mum.ResetPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Failed("Invalid Code"));
            var model = new ResetPasswordViewModel()
            {
                Code = "invalid",
                ConfirmPassword = "newPassword.01",
                Password = "newPassword.01",
                Email = "valid@example.com"
            };
            var isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.ResetPassword(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.False(Controller.ModelState.IsValid);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ResetPassword_POST_Correct_Code_Returns_RedirectToRouteResult_ResetPasswordConfirmation()
        {
            MockUserManager.Setup(mum => mum.FindByNameAsync(It.IsAny<string>()))
                           .ReturnsAsync(new VigilUser());
            MockUserManager.Setup(mum => mum.ResetPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);
            var model = new ResetPasswordViewModel()
            {
                Code = "correct",
                ConfirmPassword = "newPassword.01",
                Password = "newPassword.01",
                Email = "valid@example.com"
            };
            var isModelStateValid = Controller.BindModel(model);

            RedirectToRouteResult result = await Controller.ResetPassword(model) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.True(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("ResetPasswordConfirmation", result.RouteValues["Action"]);
            Assert.Equal("Account", result.RouteValues["Controller"]);
        }

        [Fact]
        public void ResetPasswordConfirmation_Returns_View()
        {
            ViewResult result = Controller.ResetPasswordConfirmation() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void ExternalLogin_Returns_ChallengeResult()
        {
            var urlhelper = new Mock<UrlHelper>();
            urlhelper.Setup(url => url.Action(It.Is<string>(s => s == "ExternalLoginCallback"),
                                              It.Is<string>(s => s == "Account"),
                                              It.IsAny<object>()))
                     .Returns("/Account/ExternalLoginCallback/?ReturnUrl=/Home/Index");
            Controller.Url = urlhelper.Object;

            ChallengeResult result = Controller.ExternalLogin("InMemory", "/Home/Index");

            Assert.Equal("InMemory", result.LoginProvider);
            Assert.Equal("/Account/ExternalLoginCallback/?ReturnUrl=/Home/Index", result.RedirectUri);
            Assert.Equal(Guid.Empty, result.UserId);
        }

        [Fact]
        public async Task SendCode_UnverifiedUser_Returns_Error_View()
        {
            MockSignInManager.Setup(msim => msim.GetVerifiedUserIdAsync())
                 .ReturnsAsync(Guid.Empty);

            ViewResult result = await Controller.SendCode(null, false) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task SendCode_For_Verified_User_Returns_View_With_SendCodeViewModel_With_Providers()
        {
            MockSignInManager.Setup(msim => msim.GetVerifiedUserIdAsync())
                             .ReturnsAsync(Guid.NewGuid());
            MockUserManager.Setup(mum => mum.GetValidTwoFactorProvidersAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(new List<string>() { "Test Factor" });

            ViewResult result = await Controller.SendCode(null, false) as ViewResult;
            SendCodeViewModel model = result.Model as SendCodeViewModel;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Null(model.ReturnUrl);
            Assert.False(model.RememberMe);
            Assert.Equal(1, model.Providers.Count);
        }

        [Fact]
        public async Task SendCode_POST_Invalid_ModelState_Returns_View()
        {
            var model = new SendCodeViewModel();
            Controller.ModelState.AddModelError("TestError", "Test Error to invalidate ModelState.");

            ViewResult result = await Controller.SendCode(model) as ViewResult;

            Assert.False(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.NotEqual("Error", result.ViewName);
        }

        [Fact]
        public async Task SendCode_POST_Unable_To_SendTwoFactorCode_Returns_Error_View()
        {
            MockSignInManager.Setup(msim => msim.SendTwoFactorCodeAsync(It.Is<string>(s => s == "InMemory")))
                             .ReturnsAsync(false);
            var model = new SendCodeViewModel()
            {
                SelectedProvider = "InMemory"
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.SendCode(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.True(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task SendCode_POST_Code_Sent_Returns_RedirectToAction_VerifyCode()
        {
            MockSignInManager.Setup(msim => msim.SendTwoFactorCodeAsync(It.Is<string>(s => s == "InMemory")))
                             .ReturnsAsync(true);
            var model = new SendCodeViewModel()
            {
                SelectedProvider = "InMemory"
            };
            bool isModelStateValid = Controller.BindModel(model);

            RedirectToRouteResult result = await Controller.SendCode(model) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.True(Controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("VerifyCode", result.RouteValues["Action"]);
            Assert.Null(result.RouteValues["Controller"]);
            Assert.Equal("InMemory", result.RouteValues["Provider"]);
            Assert.Equal(false, result.RouteValues["RememberMe"]);
            Assert.Null(result.RouteValues["ReturnUrl"]);
        }

        [Fact]
        public async Task ExternalLoginCallback_With_No_ExternalLoginInfo_Returns_RedirectToAction_To_Login()
        {
            RedirectToRouteResult result = await Controller.ExternalLoginCallback(null) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Null(result.RouteValues["Controller"]);
            Assert.Equal("Login", result.RouteValues["Action"]);
        }

        [Fact]
        public async Task EternalLoginCallback_Failure_Returns_ExternalLoginConfirmation_View_With_User_Email()
        {
            ArrangeGetExternalLoginInfoAsyncMethod("TestUser", "failure@example.com");
            MockSignInManager.Setup(msim => msim.ExternalSignInAsync(It.IsAny<ExternalLoginInfo>(), It.IsAny<bool>()))
                             .ReturnsAsync(SignInStatus.Failure);

            ViewResult result = await Controller.ExternalLoginCallback(null) as ViewResult;
            ExternalLoginConfirmationViewModel model = result.Model as ExternalLoginConfirmationViewModel;

            Assert.NotNull(result);
            Assert.Equal("ExternalLoginConfirmation", result.ViewName);
            Assert.NotNull(model);
            Assert.Equal("failure@example.com", model.Email);
        }

        [Fact]
        public async Task ExternalLoginCallback_RequiresVerification_Returns_RedirectToRouteResult_To_SendCode()
        {
            ArrangeGetExternalLoginInfoAsyncMethod("TestUser", "requiresverification@example.com");
            MockSignInManager.Setup(msim => msim.ExternalSignInAsync(It.IsAny<ExternalLoginInfo>(), It.IsAny<bool>()))
                             .ReturnsAsync(SignInStatus.RequiresVerification);

            RedirectToRouteResult result = await Controller.ExternalLoginCallback(null) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Null(result.RouteValues["Controller"]);
            Assert.Equal("SendCode", result.RouteValues["Action"]);
            Assert.Null(result.RouteValues["ReturnUrl"]);
            Assert.Equal(false, result.RouteValues["RememberMe"]);
        }

        [Fact]
        public async Task ExternalLoginCallback_LockedOut_Returns_Lockout_View()
        {
            ArrangeGetExternalLoginInfoAsyncMethod("TestUser", "lockedout@example.com");
            MockSignInManager.Setup(msim => msim.ExternalSignInAsync(It.IsAny<ExternalLoginInfo>(), It.IsAny<bool>()))
                             .ReturnsAsync(SignInStatus.LockedOut);

            ViewResult result = await Controller.ExternalLoginCallback(null) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Lockout", result.ViewName);
        }

        [Fact]
        public async Task ExternalLoginCallback_Success_Without_ReturnUrl_Returns_RedirectToRouteResult_To_Home()
        {
            ArrangeGetExternalLoginInfoAsyncMethod("TestUser", "success@example.com");
            MockSignInManager.Setup(msim => msim.ExternalSignInAsync(It.IsAny<ExternalLoginInfo>(), It.IsAny<bool>()))
                             .ReturnsAsync(SignInStatus.Success);

            RedirectToRouteResult result = await Controller.ExternalLoginCallback(null) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Home", result.RouteValues["Controller"]);
            Assert.Equal("Index", result.RouteValues["Action"]);

        }
        [Fact]
        public async Task ExternalLoginConfirmation_POST_User_Is_Authenticated_Returns_RedirectToRouteResult_To_Manage()
        {
            MockUser.SetupGet(mu => mu.Identity.IsAuthenticated).Returns(true);

            RedirectToRouteResult result = await Controller.ExternalLoginConfirmation(null, null) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Manage", result.RouteValues["Controller"]);
            Assert.Equal("Index", result.RouteValues["Action"]);
        }

        [Fact]
        public async Task ExternalLoginConfirmation_POST_Invalid_Model_Returns_View_With_Model()
        {
            MockUser.SetupGet(mu => mu.Identity.IsAuthenticated).Returns(false);
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel();
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.ExternalLoginConfirmation(model, null) as ViewResult;

            Assert.False(isModelStateValid);
            Assert.NotNull(result);
            Assert.False(Controller.ModelState.IsValid);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task ExternalLoginConfirmation_POST_Valid_Model_No_External_Login_Info_Returns_ExternalLoginFailure_View()
        {
            MockUser.SetupGet(mu => mu.Identity.IsAuthenticated).Returns(false);
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel()
            {
                Email = "valid@example.com"
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.ExternalLoginConfirmation(model, null) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.Equal("ExternalLoginFailure", result.ViewName);
        }

        [Fact]
        public async Task ExternalLoginConfirmation_POST_Valid_Model_Create_User_Failure_Invalidates_ModelState_And_Returns_View()
        {
            MockUser.SetupGet(mu => mu.Identity.IsAuthenticated).Returns(false);
            ArrangeGetExternalLoginInfoAsyncMethod("TestUser", "valid@example.com");
            MockUserManager.Setup(mum => mum.CreateAsync(It.IsAny<VigilUser>())).ReturnsAsync(IdentityResult.Failed("Expected UserManager.CreateAsync Failure."));
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel()
            {
                Email = "valid@example.com"
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.ExternalLoginConfirmation(model, null) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.False(Controller.ModelState.IsValid);
            Assert.NotEqual("ExternalLoginFailure", result.ViewName);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task ExternalLoginConfirmation_POST_AddLoginAsync_Failure_Invalidates_ModelState_And_Returns_View()
        {
            MockUser.SetupGet(mu => mu.Identity.IsAuthenticated).Returns(false);
            ArrangeGetExternalLoginInfoAsyncMethod("TestUser", "external@example.com");
            MockUserManager.Setup(mum => mum.CreateAsync(It.IsAny<VigilUser>()))
                           .ReturnsAsync(IdentityResult.Failed("Expected Failure"));
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel()
            {
                Email = "external@example.com"
            };
            bool isModelStateValid = Controller.BindModel(model);

            ViewResult result = await Controller.ExternalLoginConfirmation(model, null) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.False(Controller.ModelState.IsValid);
            Assert.NotEqual("ExternalLoginFailure", result.ViewName);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task ExternalLoginConfirmation_POST_Successful_User_Creation_And_Login_And_SignIn_Returns_RedirectToRouteResult_Home()
        {
            ArrangeGetExternalLoginInfoAsyncMethod("TestUser", "external@example.com");
            MockUser.SetupGet(mu => mu.Identity.IsAuthenticated).Returns(false);
            MockUserManager.Setup(mam => mam.CreateAsync(It.IsAny<VigilUser>()))
                           .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(mam => mam.AddLoginAsync(It.IsAny<Guid>(), It.IsAny<UserLoginInfo>()))
                           .ReturnsAsync(IdentityResult.Success);
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel()
            {
                Email = "external@example.com"
            };
            bool isModelStateValid = Controller.BindModel(model);

            RedirectToRouteResult result = await Controller.ExternalLoginConfirmation(model, null) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.NotNull(result);
            Assert.True(Controller.ModelState.IsValid);
            Assert.Equal("Home", result.RouteValues["Controller"]);
            Assert.Equal("Index", result.RouteValues["Action"]);
        }

        [Fact]
        public void LogOff_With_No_User_Returns_RedirectToRouteResult_To_Home()
        {
            RedirectToRouteResult result = Controller.LogOff() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Home", result.RouteValues["Controller"]);
            Assert.Equal("Index", result.RouteValues["Action"]);
        }

        [Fact]
        public void LogOff_LogsOff_User_And_Returns_RedirectToRouteResult_To_Home()
        {
            MockAuthenticationManager.Setup(mam => mam.SignOut())
                                     .Verifiable();

            RedirectToRouteResult result = Controller.LogOff() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Home", result.RouteValues["Controller"]);
            Assert.Equal("Index", result.RouteValues["Action"]);
        }

        [Fact]
        public void ExternalLoginFailure_Returns_View()
        {
            ViewResult result = Controller.ExternalLoginFailure() as ViewResult;

            Assert.NotNull(result);
            Assert.NotEqual("Error", result.ViewName);
        }

        [Fact]
        public void Dispose_When_Disposing_Disposes_UserManager_And_SignInManager()
        {
            MockUserManager.Protected().Setup("Dispose", ItExpr.IsAny<bool>())
                .Verifiable();

            using (AccountController ctrl = new AccountController(MockUserManager.Object, MockSignInManager.Object, MockAuthenticationManager.Object))
            {
            }

            MockUserManager.Protected().Verify("Dispose", Times.AtLeastOnce(), ItExpr.IsAny<bool>());
            MockSignInManager.As<IDisposable>().Verify(msim => msim.Dispose());
        }

        [Fact]
        public void VigilUserManager_Property_Gets_Manager_From_OwinContext_When_Not_Explicitly_Specified()
        {
            // HttpContext.GetOwinContext() is an extension method that calls context.Items[OwinEnvironmentKey] which passes that to a new OwinContext(environment);
            // owinContext.Get<VigilUserManager>() is an extension method that calls IOwinContext.Get<VigilUserManager>(string)
            // Thus, adding the dictionary for the Owin Environment, and adding the VigilUserManager to that dictionary results in everything mocked up just properly.

            using (var ctrl = new AccountController())
            {
                var mockHttpContext = new Mock<HttpContextBase>();
                var mockOwinContext = new Mock<IOwinContext>();
                var owinEnvironment = new Dictionary<string, object>();
                owinEnvironment.Add(IdentityGlobalConstant.IdentityKeyPrefix + typeof(VigilUserManager).AssemblyQualifiedName, MockUserManager.Object);
                mockOwinContext.Setup(oc => oc.Get<VigilUserManager>(IdentityGlobalConstant.IdentityKeyPrefix + typeof(VigilUserManager).AssemblyQualifiedName))
                               .Returns(MockUserManager.Object);
                mockHttpContext.Setup(ctx => ctx.Items[It.Is<string>(s => s == IdentityGlobalConstant.OwinEnvironmentKey)])
                               .Returns(owinEnvironment);

                var mockControllerContext = new Mock<ControllerContext>();
                mockControllerContext.SetupGet(con => con.HttpContext).Returns(mockHttpContext.Object);
                ctrl.ControllerContext = mockControllerContext.Object;

                Assert.Same(MockUserManager.Object, ctrl.UserManager);
            }
        }

        [Fact]
        public void IVigilSignInManager_Property_Gets_Manager_From_OwinContext_When_Not_Explicitly_Specified()
        {
            // HttpContext.GetOwinContext() is an extension method that calls context.Items[OwinEnvironmentKey] which passes that to a new OwinContext(environment);
            // owinContext.Get<IVigilSignInManager>() is an extension method that calls IOwinContext.Get<IVigilSignInManager>(string)
            // Thus, adding the dictionary for the Owin Environment, and adding the IVigilSignInManager to that dictionary results in everything mocked up just properly.

            using (var ctrl = new AccountController())
            {
                var mockHttpContext = new Mock<HttpContextBase>();
                var mockOwinContext = new Mock<IOwinContext>();
                var owinEnvironment = new Dictionary<string, object>();
                owinEnvironment.Add(IdentityGlobalConstant.IdentityKeyPrefix + typeof(IVigilSignInManager).AssemblyQualifiedName, MockSignInManager.Object);
                mockOwinContext.Setup(oc => oc.Get<IVigilSignInManager>(IdentityGlobalConstant.IdentityKeyPrefix + typeof(IVigilSignInManager).AssemblyQualifiedName))
                               .Returns(MockSignInManager.Object);
                mockHttpContext.Setup(ctx => ctx.Items[It.Is<string>(s => s == IdentityGlobalConstant.OwinEnvironmentKey)])
                               .Returns(owinEnvironment);

                var mockControllerContext = new Mock<ControllerContext>();
                mockControllerContext.SetupGet(con => con.HttpContext).Returns(mockHttpContext.Object);
                ctrl.ControllerContext = mockControllerContext.Object;

                Assert.Same(MockSignInManager.Object, ctrl.SignInManager);
            }
        }

        private void ArrangeGetExternalLoginInfoAsyncMethod(string nameIdentifierValue, string emailValue)
        {
            // AuthenticationManager.GetExternalLoginInfoAsync() is an extension method that calls IAuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie)
            // and creates the ExternalLoginInfo object from the AuthenticateResult returned from that method.
            var mockClaimsIdentity = new Mock<ClaimsIdentity>();
            mockClaimsIdentity.Setup(mci => mci.FindFirst(It.Is<string>(s => s == ClaimTypes.NameIdentifier)))
                          .Returns(new Claim(ClaimTypes.NameIdentifier, nameIdentifierValue));
            mockClaimsIdentity.SetupGet(mci => mci.Name)
                          .Returns(nameIdentifierValue);
            mockClaimsIdentity.Setup(mci => mci.FindFirst(It.Is<string>(s => s == ClaimTypes.Email)))
                          .Returns(new Claim(ClaimTypes.Email, emailValue));
            MockAuthenticationManager.Setup(mam => mam.AuthenticateAsync(It.Is<string>(s => s == DefaultAuthenticationTypes.ExternalCookie)))
                                     .ReturnsAsync(new AuthenticateResult(mockClaimsIdentity.Object, new AuthenticationProperties(), new AuthenticationDescription()));
        }

        protected virtual void Dispose(bool disposing){
            if (disposing)
            {
                if (Controller != null)
                {
                    Controller.Dispose();
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
