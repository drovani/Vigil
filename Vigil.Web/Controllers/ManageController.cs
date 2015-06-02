using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Vigil.Identity.Model;
using Vigil.Web.Controllers.Results;
using Vigil.Web.Models;

namespace Vigil.Web.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private VigilSignInManager _signInManager;
        private VigilUserManager _userManager;

        public Guid UserId { get { return Guid.Parse(User.Identity.GetUserId()); } }

        public ManageController()
        {
        }

        public ManageController(VigilUserManager userManager, VigilSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public VigilSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<VigilSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public VigilUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<VigilUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>GET: /Manage/Index
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(UserId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(UserId),
                Logins = await UserManager.GetLoginsAsync(UserId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(UserId.ToString())
            };
            return View(model);
        }

        /// <summary>POST: /Manage/RemoveLogin
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(UserId, new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(UserId);
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        /// <summary>GET: /Manage/AddPhoneNumber
        /// </summary>
        /// <returns></returns>
        [Pure]
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        /// <summary>POST: /Manage/AddPhoneNumber
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(UserId, model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        /// <summary>POST: /Manage/EnableTwoFactorAuthentication
        /// </summary>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            await UserManager.SetTwoFactorEnabledAsync(UserId, true);
            var user = await UserManager.FindByIdAsync(UserId);
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        /// <summary>POST: /Manage/DisableTwoFactorAuthentication
        /// </summary>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            await UserManager.SetTwoFactorEnabledAsync(UserId, false);
            var user = await UserManager.FindByIdAsync(UserId);
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        /// <summary>GET: /Manage/VerifyPhoneNumber
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(UserId, phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        /// <summary>POST: /Manage/VerifyPhoneNumber
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(UserId, model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(UserId);
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        /// <summary>GET: /Manage/RemovePhoneNumber
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RemovePhoneNumber()
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            var result = await UserManager.SetPhoneNumberAsync(UserId, null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(UserId);
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        /// <summary>GET: /Manage/ChangePassword
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);

            return View();
        }

        /// <summary>POST: /Manage/ChangePassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(UserId, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(UserId);
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        /// <summary>GET: /Manage/SetPassword
        /// </summary>
        /// <returns></returns>
        public ActionResult SetPassword()
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);

            return View();
        }

        /// <summary>POST: /Manage/SetPassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(UserId, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(UserId);
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>GET: /Manage/ManageLogins
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(UserId);
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        /// <summary>POST: /Manage/LinkLogin
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);

            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), UserId);
        }

        /// <summary>GET: /Manage/LinkLoginCallback
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> LinkLoginCallback()
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(ChallengeResult.XsrfKey, UserId.ToString());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(UserId, loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            Contract.Requires<ArgumentNullException>(result != null);
            Contract.Requires<ArgumentNullException>(result.Errors != null);

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(UserId);
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(UserId);
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
    }
}