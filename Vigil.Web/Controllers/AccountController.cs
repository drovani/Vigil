using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Vigil.Data.Core.System;
using Vigil.Identity.Model;
using Vigil.Web.Controllers.Results;
using Vigil.Web.Models;

namespace Vigil.Web.Controllers
{
    [Authorize]
    [ContractVerification(false)]
    public class AccountController : Controller
    {
        private VigilSignInManager _signInManager;
        private VigilUserManager _userManager;
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                Contract.Assume(HttpContext != null);
                IOwinContext context = HttpContext.GetOwinContext();
                Contract.Assume(context != null);
                
                return context.Authentication;
            }
        }

        public VigilSignInManager SignInManager
        {
            get
            {
                Contract.Ensures(Contract.Result<VigilSignInManager>() != null);
                return _signInManager ?? GetVigilSignInManager();
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
                Contract.Ensures(Contract.Result<VigilUserManager>() != null);
                return _userManager ?? GetVigilUserManager();
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                _userManager = value;
            }
        }

        public AccountController()
        {
        }
        public AccountController(VigilUserManager userManager, VigilSignInManager signInManager)
        {
            Contract.Requires<ArgumentNullException>(userManager != null);
            Contract.Requires<ArgumentNullException>(signInManager != null);

            UserManager = userManager;
            SignInManager = signInManager;
        }

        /// <summary>GET: /Account/Login
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);
            ViewResult view = View();
            Contract.Assume(view != null);

            ViewBag.ReturnUrl = returnUrl;
            return view;
        }

        /// <summary>POST: /Account/Login
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        /// <summary>GET: /Account/VerifyCode
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>POST: /Account/VerifyCode
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        /// <summary>GET: /Account/Register
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);

            return View();
        }

        /// <summary>POST: /Account/Register
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (ModelState.IsValid)
            {
                var user = new VigilUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>GET: /Account/ConfirmEmail
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(Guid userId, string code)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>GET: /Account/ForgotPassword
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);

            return View();
        }

        /// <summary>POST: /Account/ForgotPassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>GET: /Account/ForgotPasswordConfirmation
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);

            return View();
        }

        /// <summary>GET: /Account/ResetPassword
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);
            ViewResult view = code == null ? View("Error") : View();
            Contract.Assume(view != null);

            return view;
        }

        /// <summary>POST: /Account/ResetPassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        /// <summary>GET: /Account/ResetPasswordConfirmation
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);
            Contract.Assume(View() != null);

            return View();
        }

        /// <summary>POST: /Account/ExternalLogin
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ChallengeResult ExternalLogin(string provider, string returnUrl)
        {
            Contract.Requires<ArgumentNullException>(Url != null);
            Contract.Ensures(Contract.Result<ActionResult>() != null);

            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        /// <summary>GET: /Account/SendCode
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>POST: /Account/SendCode
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        /// <summary>GET: /Account/ExternalLoginCallback
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        /// <summary>POST: /Account/ExternalLoginConfirmation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            Contract.Ensures(Contract.Result<Task<ActionResult>>() != null);

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new VigilUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        /// <summary>POST: /Account/LogOff
        /// </summary>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);

            Contract.Assume(this.AuthenticationManager != null);

            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>GET: /Account/ExternalLoginFailure
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ViewResult ExternalLoginFailure()
        {
            EnsuresActionResultIsNotNull();

            ViewResult view = View();
            Contract.Assume(view != null);

            return view;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        private void AddErrors(IdentityResult result)
        {
            Contract.Requires<ArgumentNullException>(result != null);
            Contract.Requires<ArgumentNullException>(result.Errors != null);

            Contract.Assume(ModelState != null);

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private VigilUserManager GetVigilUserManager()
        {
            Contract.Ensures(Contract.Result<VigilUserManager>() != null);

            IOwinContext owinContext = HttpContext.GetOwinContext();
            Contract.Assume(owinContext != null);
            VigilUserManager manager = owinContext.Get<VigilUserManager>();
            Contract.Assume(manager != null);

            return manager;
        }
        private VigilSignInManager GetVigilSignInManager()
        {
            Contract.Ensures(Contract.Result<VigilSignInManager>() != null);

            IOwinContext owinContext = HttpContext.GetOwinContext();
            Contract.Assume(owinContext != null);
            VigilSignInManager manager = owinContext.Get<VigilSignInManager>();
            Contract.Assume(manager != null);
            return manager;
        }

        [ContractAbbreviator]
        private void EnsuresActionResultIsNotNull()
        {
            Contract.Ensures(Contract.Result<ActionResult>() != null);
            Contract.Assume(View() != null);
        }
    }
}