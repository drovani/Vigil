using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Moq;
using Moq.Protected;
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
    public class ManageControllerTests
    {
        private readonly Mock<IPrincipal> MockUser;
        private readonly Mock<VigilUserManager> MockUserManager;
        private readonly Mock<IVigilSignInManager> MockSignInManager;
        private readonly Mock<IAuthenticationManager> MockAuthenticationManager = new Mock<IAuthenticationManager>();

        public ManageControllerTests()
        {
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.SetupGet(mi => mi.Name).Returns("TestUser");
            mockIdentity.Setup(mi => mi.FindFirst(It.Is<string>(s => s == ClaimsIdentity.DefaultNameClaimType))).Returns(new Claim(ClaimsIdentity.DefaultNameClaimType, "TestUser"));
            mockIdentity.Setup(mi => mi.FindFirst(It.Is<string>(s => s == ClaimTypes.NameIdentifier))).Returns(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));

            MockUser = new Mock<IPrincipal>();
            MockUser.SetupGet(mu => mu.Identity).Returns(mockIdentity.Object);
            MockUserManager = new Mock<VigilUserManager>(Mock.Of<IUserStore<VigilUser, Guid>>());
            MockAuthenticationManager = new Mock<IAuthenticationManager>();
            MockSignInManager = new Mock<IVigilSignInManager>();
            MockSignInManager.SetupGet(msim => msim.AuthenticationManager).Returns(MockAuthenticationManager.Object);
            MockSignInManager.SetupGet(msim => msim.UserManager).Returns(MockUserManager.Object);
        }

        [Fact]
        public void ManageController_Default_Constructor()
        {
            var ctrl = new ManageController();
            Assert.NotNull(ctrl);
        }

        [Fact]
        public void ManageController_Explicit_Constructor()
        {
            var ctrl = GetManageController();

            Assert.NotNull(ctrl);
            Assert.Same(MockSignInManager.Object, ctrl.SignInManager);
            Assert.Same(MockUserManager.Object, ctrl.UserManager);
        }

        [Fact]
        public async Task Index_Returns_View_With_IndexViewModel_Model()
        {
            var ctrl = GetManageController();
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new VigilUser() { Id = Guid.Parse(MockUser.Object.Identity.GetUserId()) });
            MockUserManager.Setup(mum => mum.GetPhoneNumberAsync(It.IsAny<Guid>())).ReturnsAsync("3125550123");
            MockUserManager.Setup(mum => mum.GetTwoFactorEnabledAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            MockUserManager.Setup(mum => mum.GetLoginsAsync(It.IsAny<Guid>())).ReturnsAsync(new List<UserLoginInfo>());
            MockAuthenticationManager.Setup(mam => mam.AuthenticateAsync(It.Is<string>(s => s == DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie)))
                                     .ReturnsAsync(new AuthenticateResult(null, new AuthenticationProperties(), new AuthenticationDescription()));

            ViewResult result = await ctrl.Index(null) as ViewResult;
            IndexViewModel model = result.Model as IndexViewModel;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(false, model.HasPassword);
            Assert.Equal("3125550123", model.PhoneNumber);
            Assert.Equal(false, model.TwoFactor);
            Assert.Equal(0, model.Logins.Count);
            Assert.Equal(false, model.BrowserRemembered);
        }

        [Fact]
        public async Task RemoveLogin_POST_When_RemoveLoginAsync_Fails_Returns_RedirectToRouteResult_With_Error_Message()
        {
            var ctrl = GetManageController();
            MockUserManager.Setup(mum => mum.RemoveLoginAsync(It.IsAny<Guid>(), It.IsAny<UserLoginInfo>()))
                           .ReturnsAsync(IdentityResult.Failed("Failed"));

            RedirectToRouteResult result = await ctrl.RemoveLogin(String.Empty, String.Empty) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("ManageLogins", result.RouteValues["Action"]);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact]
        public async Task RemoveLogin_POST_When_RemoveLoginAsync_Succeeds_Returns_RedirectToRouteResult_With_Success_Message()
        {
            var ctrl = GetManageController();
            MockUserManager.Setup(mum => mum.RemoveLoginAsync(It.IsAny<Guid>(), It.IsAny<UserLoginInfo>()))
                           .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(new VigilUser() { Id = Guid.Parse(MockUser.Object.Identity.GetUserId()) });
            RedirectToRouteResult result = await ctrl.RemoveLogin("TestProvider", "TestKey") as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("ManageLogins", result.RouteValues["Action"]);
            Assert.Equal(ManageController.ManageMessageId.RemoveLoginSuccess, result.RouteValues["Message"]);
        }

        [Fact]
        public void AddPhoneNumber_Returns_View()
        {
            var ctrl = new ManageController();

            ViewResult result = ctrl.AddPhoneNumber() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddPhoneNumber_POST_With_Invalid_Model_Returns_View_With_Same_Model()
        {
            var ctrl = new ManageController();
            AddPhoneNumberViewModel model = new AddPhoneNumberViewModel();
            bool isModelStateValid = ctrl.BindModel(model);

            ViewResult result = await ctrl.AddPhoneNumber(model) as ViewResult;

            Assert.False(isModelStateValid);
            Assert.False(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task AddPhoneNumber_POST_With_Valid_Model_Sends_Code_Returns_RedirectToAction_VerifyPhoneNumber()
        {
            var ctrl = GetManageController();
            MockUserManager.Setup(mum => mum.GenerateChangePhoneNumberTokenAsync(It.IsAny<Guid>(), It.Is<string>(s => s == "3125550123")))
                           .ReturnsAsync("TestToken");
            var mockSmsService = new Mock<IIdentityMessageService>();
            mockSmsService.Setup(mss => mss.SendAsync(It.IsAny<IdentityMessage>()))
                           .Returns(Task.FromResult(IdentityResult.Success))
                           .Verifiable();
            MockUserManager.Object.SmsService = mockSmsService.Object;

            AddPhoneNumberViewModel model = new AddPhoneNumberViewModel()
            {
                Number = "3125550123"
            };
            bool isModelStateValid = ctrl.BindModel(model);

            RedirectToRouteResult result = await ctrl.AddPhoneNumber(model) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.True(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("VerifyPhoneNumber", result.RouteValues["Action"]);
            Assert.Equal("3125550123", result.RouteValues["PhoneNumber"]);
            mockSmsService.Verify();
        }

        [Fact]
        public async Task EnableTwoFactorAuthentication_POST_When_User_Is_Null_Returns_RedirectToAction_Index()
        {
            MockUserManager.Setup(mum => mum.SetTwoFactorEnabledAsync(It.IsAny<Guid>(), It.Is<bool>(b => b == true)))
                           .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(null);
            MockSignInManager.Setup(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()))
                             .Verifiable();
            var ctrl = GetManageController();

            RedirectToRouteResult result = await ctrl.EnableTwoFactorAuthentication() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["Action"]);
            Assert.Equal("Manage", result.RouteValues["Controller"]);
            MockSignInManager.Verify(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public async Task EnableTwoFactorAuthentication_POST_When_User_Exists_Calls_SignInAsync_And_Returns_RedirectToAction_Index()
        {
            MockUserManager.Setup(mum => mum.SetTwoFactorEnabledAsync(It.IsAny<Guid>(), It.Is<bool>(b => b == true)))
                           .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(new VigilUser());
            MockSignInManager.Setup(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()))
                             .Returns(Task.FromResult(IdentityResult.Success))
                             .Verifiable();
            var ctrl = GetManageController();

            RedirectToRouteResult result = await ctrl.EnableTwoFactorAuthentication() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["Action"]);
            Assert.Equal("Manage", result.RouteValues["Controller"]);
            MockSignInManager.Verify(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async Task DisableTwoFactorAuthentication_POST_When_User_Is_Null_Returns_RedirectToAction_Index()
        {
            MockUserManager.Setup(mum => mum.SetTwoFactorEnabledAsync(It.IsAny<Guid>(), It.Is<bool>(b => b == false)))
                           .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(null);
            MockSignInManager.Setup(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()))
                             .Verifiable();
            var ctrl = GetManageController();

            RedirectToRouteResult result = await ctrl.DisableTwoFactorAuthentication() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["Action"]);
            Assert.Equal("Manage", result.RouteValues["Controller"]);
            MockSignInManager.Verify(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public async Task DisableTwoFactorAuthentication_POST_When_User_Exists_Calls_SignInAsync_And_Returns_RedirectToAction_Index()
        {
            MockUserManager.Setup(mum => mum.SetTwoFactorEnabledAsync(It.IsAny<Guid>(), It.Is<bool>(b => b == false)))
                           .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(new VigilUser());
            MockSignInManager.Setup(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()))
                             .Returns(Task.FromResult(IdentityResult.Success))
                             .Verifiable();
            var ctrl = GetManageController();

            RedirectToRouteResult result = await ctrl.DisableTwoFactorAuthentication() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["Action"]);
            Assert.Equal("Manage", result.RouteValues["Controller"]);
            MockSignInManager.Verify(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async Task VerifyPhoneNumber_Without_PhoneNumber_Returns_Error_View()
        {
            MockUserManager.Setup(mum => mum.GenerateChangePhoneNumberTokenAsync(It.IsAny<Guid>(), It.Is<string>(s => s == null)))
                           .ReturnsAsync("TestCode");
            var ctrl = GetManageController();

            ViewResult result = await ctrl.VerifyPhoneNumber(phoneNumber: null) as ViewResult;
            VerifyPhoneNumberViewModel model = result.Model as VerifyPhoneNumberViewModel;

            Assert.NotNull(result);
            Assert.Null(model);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task VerifyPhoneNumber_With_PhoneNumber_Returns_View_With_VerifyPhoneNumberViewModel()
        {
            MockUserManager.Setup(mum => mum.GenerateChangePhoneNumberTokenAsync(It.IsAny<Guid>(), It.Is<string>(s => s == "3125550123")))
                           .ReturnsAsync("TestCode");
            var ctrl = GetManageController();

            ViewResult result = await ctrl.VerifyPhoneNumber(phoneNumber: "3125550123") as ViewResult;
            VerifyPhoneNumberViewModel model = result.Model as VerifyPhoneNumberViewModel;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal("3125550123", model.PhoneNumber);
            Assert.NotEqual("Error", result.ViewName);
        }

        [Fact]
        public async Task VerifyPhoneNumber_POST_With_Invalid_Model_Returns_View_With_Same_Model()
        {
            VerifyPhoneNumberViewModel model = new VerifyPhoneNumberViewModel();
            var ctrl = new ManageController();
            bool isModelStateValid = ctrl.BindModel(model);

            ViewResult result = await ctrl.VerifyPhoneNumber(model) as ViewResult;

            Assert.False(isModelStateValid);
            Assert.False(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task VerifyPhoneNumber_POST_With_Failed_ChangePhoneNumberAsync_Invalidates_ModelState_And_Resturns_View_With_Same_Model()
        {
            MockUserManager.Setup(mum => mum.ChangePhoneNumberAsync(It.IsAny<Guid>(), It.Is<string>(s => s == "3125550123"), It.Is<string>(s => s == "TestCode")))
                           .ReturnsAsync(IdentityResult.Failed("Invalid Code"));
            VerifyPhoneNumberViewModel model = new VerifyPhoneNumberViewModel()
            {
                PhoneNumber = "3125550123",
                Code = "TestCode"
            };
            var ctrl = new ManageController();
            bool isModelStateValid = ctrl.BindModel(model);

            ViewResult result = await ctrl.VerifyPhoneNumber(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.False(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task VerifyPhoneNumber_POST_With_Successful_ChangePhoneNumberAsync_Returns_RedirectToAction_Index()
        {
            MockUserManager.Setup(mum => mum.ChangePhoneNumberAsync(It.IsAny<Guid>(), It.Is<string>(s => s == "3125550123"), It.Is<string>(s => s == "TestCode")))
                           .ReturnsAsync(IdentityResult.Success);
            VerifyPhoneNumberViewModel model = new VerifyPhoneNumberViewModel()
            {
                PhoneNumber = "3125550123",
                Code = "TestCode"
            };
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(new VigilUser());
            MockSignInManager.Setup(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.Is<bool>(b => b == false), It.Is<bool>(b => b == false)))
                             .Returns(Task.FromResult(IdentityResult.Success))
                             .Verifiable();
            var ctrl = GetManageController();
            bool isModelStateValid = ctrl.BindModel(model);

            RedirectToRouteResult result = await ctrl.VerifyPhoneNumber(model) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.True(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["Action"]);
            Assert.Equal(ManageController.ManageMessageId.AddPhoneSuccess, result.RouteValues["Message"]);
            MockSignInManager.Verify(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.Is<bool>(b => b == false), It.Is<bool>(b => b == false)), Times.Once());
        }

        [Fact]
        public async Task RemovePhoneNumber_When_SetPhoneNumberAsync_Failed_Returns_RedirectToAction_Index_With_Error_Message()
        {
            MockUserManager.Setup(mum => mum.SetPhoneNumberAsync(It.IsAny<Guid>(), It.Is<string>(s => s == null)))
                           .ReturnsAsync(IdentityResult.Failed("Failure."));
            var ctrl = GetManageController();

            RedirectToRouteResult result = await ctrl.RemovePhoneNumber() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["Action"]);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact]
        public async Task RemovePhoneNumber_When_Successful_Returns_RedirectToAction_Index_With_Success_Message()
        {
            MockUserManager.Setup(mum => mum.SetPhoneNumberAsync(It.IsAny<Guid>(), It.Is<string>(s => s == null)))
                           .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(new VigilUser());
            MockSignInManager.Setup(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.Is<bool>(b => b == false), It.Is<bool>(b => b == false)))
                             .Returns(Task.FromResult(IdentityResult.Success))
                             .Verifiable();
            var ctrl = GetManageController();

            RedirectToRouteResult result = await ctrl.RemovePhoneNumber() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["Action"]);
            Assert.Equal(ManageController.ManageMessageId.RemovePhoneSuccess, result.RouteValues["Message"]);
            MockSignInManager.Verify(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.Is<bool>(b => b == false), It.Is<bool>(b => b == false)), Times.Once());
        }

        [Fact]
        public void ChangePassword_Returns_View()
        {
            var ctrl = new ManageController();

            ViewResult result = ctrl.ChangePassword() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ChangePassword_POST_With_Invalid_Model_Returns_View_With_Same_Model()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            var ctrl = GetManageController();
            bool isModelStateValid = ctrl.BindModel(model);

            ViewResult result = await ctrl.ChangePassword(model) as ViewResult;

            Assert.False(isModelStateValid);
            Assert.False(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task ChangePassword_POST_With_Failed_ChangePasswordAsync_Invalidate_ModelState_And_Returns_View_With_Same_Model()
        {
            MockUserManager.Setup(mum => mum.ChangePasswordAsync(It.IsAny<Guid>(), It.Is<string>(s => s == "oldPassword.01"), It.Is<string>(s => s == "newPassword.01")))
                           .ReturnsAsync(IdentityResult.Failed("Failed"));
            ChangePasswordViewModel model = new ChangePasswordViewModel()
            {
                OldPassword = "oldPassword.01",
                NewPassword = "newPassword.01",
                ConfirmPassword = "newPassword.01"
            };
            var ctrl = GetManageController();
            bool isModelStateValid = ctrl.BindModel(model);

            ViewResult result = await ctrl.ChangePassword(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.False(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task ChangePassword_POST_With_Successfull_ChangePasswordAsync_Calls_SignInAsync_And_Returns_RedirectToAction_Index()
        {
            MockUserManager.Setup(mum => mum.ChangePasswordAsync(It.IsAny<Guid>(), It.Is<string>(s => s == "oldPassword.01"), It.Is<string>(s => s == "newPassword.01")))
                           .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(new VigilUser());
            MockSignInManager.Setup(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()))
                             .Verifiable();
            ChangePasswordViewModel model = new ChangePasswordViewModel()
            {
                OldPassword = "oldPassword.01",
                NewPassword = "newPassword.01",
                ConfirmPassword = "newPassword.01"
            };
            var ctrl = GetManageController();
            bool isModelStateValid = ctrl.BindModel(model);

            RedirectToRouteResult result = await ctrl.ChangePassword(model) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.True(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["Action"]);
            Assert.Equal(ManageController.ManageMessageId.ChangePasswordSuccess, result.RouteValues["Message"]);
            MockSignInManager.Verify(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()));
        }

        [Fact]
        public void SetPassword_Returns_View()
        {
            var ctrl = new ManageController();

            ViewResult result = ctrl.SetPassword() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task SetPassword_POST_With_Invalid_Model_Returns_View_With_Same_Model()
        {
            SetPasswordViewModel model = new SetPasswordViewModel();
            var ctrl = new ManageController();
            bool isModelStateValid = ctrl.BindModel(model);

            ViewResult result = await ctrl.SetPassword(model) as ViewResult;

            Assert.False(isModelStateValid);
            Assert.False(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task SetPassword_POST_With_Valid_Model_And_Failed_AddPasswordAsync_Invalidates_ModelState_And_Returns_View_With_Same_Model()
        {
            MockUserManager.Setup(mum => mum.AddPasswordAsync(It.IsAny<Guid>(), It.Is<string>(s => s == "newPassword.01")))
                           .ReturnsAsync(IdentityResult.Failed("Failed"));
            SetPasswordViewModel model = new SetPasswordViewModel()
            {
                NewPassword = "newPassword.01",
                ConfirmPassword = "newPassword.01"
            };
            var ctrl = GetManageController();
            bool isModelStateValid = ctrl.BindModel(model);

            ViewResult result = await ctrl.SetPassword(model) as ViewResult;

            Assert.True(isModelStateValid);
            Assert.False(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Same(model, result.Model);
        }

        [Fact]
        public async Task SetPassword_POST_With_Valid_Model_And_Successfull_AddPasswordAsync_Returns_RedirectToAction_Index_With_Success_Message()
        {
            MockUserManager.Setup(mum => mum.AddPasswordAsync(It.IsAny<Guid>(), It.Is<string>(s => s == "newPassword.01")))
                           .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(new VigilUser());
            MockSignInManager.Setup(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()))
                             .Verifiable();
            SetPasswordViewModel model = new SetPasswordViewModel()
            {
                NewPassword = "newPassword.01",
                ConfirmPassword = "newPassword.01"
            };
            var ctrl = GetManageController();
            bool isModelStateValid = ctrl.BindModel(model);

            RedirectToRouteResult result = await ctrl.SetPassword(model) as RedirectToRouteResult;

            Assert.True(isModelStateValid);
            Assert.False(ctrl.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["Action"]);
            Assert.Equal(ManageController.ManageMessageId.SetPasswordSuccess, result.RouteValues["Message"]);
            MockSignInManager.Verify(msim => msim.SignInAsync(It.IsAny<VigilUser>(), It.IsAny<bool>(), It.IsAny<bool>()));
        }

        [Fact]
        public async Task ManageLogins_When_User_Not_Found_Returns_Error_View()
        {
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(null);
            var ctrl = GetManageController();

            ViewResult result = await ctrl.ManageLogins(null) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ManageLogins_When_User_Found_Returns_View_With_ManageLoginsViewModel_Model()
        {
            MockUserManager.Setup(mum => mum.FindByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(new VigilUser());
            MockUserManager.Setup(mum => mum.GetLoginsAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(new List<UserLoginInfo>());
            MockAuthenticationManager.Setup(mam => mam.GetAuthenticationTypes(It.IsAny<Func<AuthenticationDescription, bool>>()))
                           .Returns(new List<AuthenticationDescription>());
            var ctrl = GetManageController();

            ViewResult result = await ctrl.ManageLogins(null) as ViewResult;
            ManageLoginsViewModel model = result.Model as ManageLoginsViewModel;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(0, model.CurrentLogins.Count);
            Assert.Equal(0, model.OtherLogins.Count);
        }

        [Fact]
        public void LinkLogin_Returns_ChallengeResult_With_Provider_RedirectUri_And_UserId()
        {
            var ctrl = GetManageController();
            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(muh => muh.Action(It.Is<string>(s => s == "LinkLoginCallback"), It.Is<string>(s => s == "Manage")))
                         .Returns("/Manage/LinkLoginCallback");

            ChallengeResult result = ctrl.LinkLogin("TestProvider") as ChallengeResult;

            Assert.NotNull(result);
            Assert.Equal("TestProvider", result.LoginProvider);
            Assert.Equal("/Manage/LinkLoginCallback", result.RedirectUri);
            Assert.Equal(MockUser.Object.Identity.GetUserId(), result.UserId.ToString());
        }

        [Fact]
        public async Task LinkLoginCallback_When_GetExternalLoginInfoAsync_Fails_Returns_RedirectToAction_ManageLogins_With_Error_Message()
        {
            // AuthenticationManager.GetExternalLoginInfoAsync(string, string) -> returns null
            MockAuthenticationManager.Setup(mam => mam.AuthenticateAsync(It.Is<string>(s => s == DefaultAuthenticationTypes.ExternalCookie)))
                                     .ReturnsAsync(new AuthenticateResult(null, new AuthenticationProperties(), new AuthenticationDescription()));
            MockUserManager.Setup(mum => mum.AddLoginAsync(It.IsAny<Guid>(), It.IsAny<UserLoginInfo>()))
                           .ReturnsAsync(IdentityResult.Failed("Failed"));
            var ctrl = GetManageController();

            RedirectToRouteResult result = await ctrl.LinkLoginCallback() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("ManageLogins", result.RouteValues["Action"]);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact]
        public async Task LinkLoginCallback_When_GetExternalLoginInfoAsync_Succeeds_But_AddLoginAsync_Fails_Returns_RedirectToAction_ManageLogins_With_Error_Message()
        {
            var ctrl = GetManageController();
            // AuthenticationManager.GetExternalLoginInfoAsync(string, string) -> returns null
            MockAuthenticationManager.Setup(mam => mam.AuthenticateAsync(It.Is<string>(s => s == DefaultAuthenticationTypes.ExternalCookie)))
                                     .ReturnsAsync(new AuthenticateResult(MockUser.Object.Identity, new AuthenticationProperties(
                                     new Dictionary<string, string>()
                                         {
                                             { ChallengeResult.XsrfKey, ctrl.UserId.ToString() }
                                         }
                                     ), new AuthenticationDescription()));
            MockUserManager.Setup(mum => mum.AddLoginAsync(It.IsAny<Guid>(), It.IsAny<UserLoginInfo>()))
                           .ReturnsAsync(IdentityResult.Failed("Failed"));

            RedirectToRouteResult result = await ctrl.LinkLoginCallback() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("ManageLogins", result.RouteValues["Action"]);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact]
        public void Dispose_When_Disposing_Disposes_UserManager_And_SignInManager()
        {
            MockUserManager.Protected().Setup("Dispose", ItExpr.IsAny<bool>())
                .Verifiable();

            using (ManageController ctrl = GetManageController()) { }

            MockUserManager.Protected().Verify("Dispose", Times.AtLeastOnce(), ItExpr.IsAny<bool>());
            MockSignInManager.As<IDisposable>().Verify(msim => msim.Dispose());
        }

        [Fact]
        public void VigilUserManager_Property_Gets_Manager_From_OwinContext_When_Not_Explicitly_Specified()
        {
            // HttpContext.GetOwinContext() is an extension method that calls context.Items[OwinEnvironmentKey] which passes that to a new OwinContext(environment);
            // owinContext.Get<VigilUserManager>() is an extension method that calls IOwinContext.Get<VigilUserManager>(string)
            // Thus, adding the dictionary for the Owin Environment, and adding the VigilUserManager to that dictionary results in everything mocked up just properly.

            var ctrl = new ManageController();
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

        [Fact]
        public void IVigilSignInManager_Property_Gets_Manager_From_OwinContext_When_Not_Explicitly_Specified()
        {
            // HttpContext.GetOwinContext() is an extension method that calls context.Items[OwinEnvironmentKey] which passes that to a new OwinContext(environment);
            // owinContext.Get<IVigilSignInManager>() is an extension method that calls IOwinContext.Get<IVigilSignInManager>(string)
            // Thus, adding the dictionary for the Owin Environment, and adding the IVigilSignInManager to that dictionary results in everything mocked up just properly.

            var ctrl = new ManageController();
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

        private ManageController GetManageController()
        {
            var ctrl = new ManageController(MockUserManager.Object, MockSignInManager.Object, MockAuthenticationManager.Object);

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.SetupGet(ctx => ctx.User).Returns(MockUser.Object);
            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.SetupGet(con => con.HttpContext).Returns(mockHttpContext.Object);

            ctrl.ControllerContext = mockControllerContext.Object;
            return ctrl;
        }
    }
}
