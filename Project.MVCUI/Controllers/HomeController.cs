using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.BLL.ManagerServices.Abstracts;
using Project.COMMON.Extensions;
using Project.COMMON.Tools;
using Project.ENTITIES.Models;
using Project.MVCUI.Extensions;
using Project.MVCUI.ViewModels;
using System.Diagnostics;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Project.MVCUI.Controllers
{
    [Route("[Controller]/[Action]")]
    public class HomeController : Controller
    {
        private readonly IAppUserManager _appUserManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAppUserProfileManager _appUserProfileManager;

        public HomeController(IAppUserManager appUserManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IAppUserProfileManager appUserProfileManager)
        {
            _appUserManager = appUserManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _appUserProfileManager = appUserProfileManager;
        }

        [Route("/")]
        [Route("/Home")]
        [Route("/Home/Index")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel request, string? returnUrl = null)
        {
            returnUrl ??= Url.Action(nameof(Index), "Home");

            AppUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                ModelState.AddModelErrorWithOutKey("Email veya şifre yanlış");
                return View();
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(user, request.PasswordHash, request.RememberMe, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelErrorWithOutKey("3 dakika boyunca giriş yapamazsınız");
                return View();
            }
            else if (!result.Succeeded)
            {
                ModelState.AddModelErrorWithOutKey("Email veya şifre yanlış", $"Başarısız giriş sayısı => {await _userManager.GetAccessFailedCountAsync(user)}");
                return View();
            }

            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelErrorWithOutKey("Lütfen hesabınızı aktif hale getiriniz, mail'lerinizi kontrol ediniz");
                await _signInManager.SignOutAsync();//So that the cookie still does not remain in the browser
                return View();
            }

            AppUserProfile? userProfile = _appUserProfileManager.FindByString(user.Id);
            if (userProfile != null)
            {
                SessionViewModel session = new SessionViewModel() { ImagePath = userProfile.ImagePath };
                HttpContext.Session.SetSession("sessionVM", session);
            }


            return Redirect(returnUrl!);
        }

        public IActionResult Terms()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Remove("sessionVM");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            if (TempData["resetPasswordAlert"] != null) ModelState.AddModelErrorWithOutKey(TempData["resetPasswordAlert"]!.ToString()!);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
        {
            ModelState.Remove("NewPassword");
            ModelState.Remove("NewPasswordConfirm");
            if (!ModelState.IsValid) return View();

            AppUser? appUser = await _userManager.FindByEmailAsync(request.Email);
            if(appUser == null)
            {
                ModelState.AddModelErrorWithOutKey("Bu email adresine sahip kullanıcı bulunamadı");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(appUser!);

            string passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = appUser!.Id, token = passwordResetToken }, HttpContext.Request.Scheme)!;

            string emailBody = $"Şifre sıfırlama talebiniz alınmıştır. Şifrenizi sıfırlamak için <a href='{passwordResetLink}'>buraya tıklayabilirsiniz.</a>";

            MailService.SendMailAsync(request.Email, emailBody, "Şifre Sıfırlama | Onion Project");
            TempData["success"] = "Şifre sıfırlama linki email adresinize gönderilmiştir";
            return View();
        }

        public IActionResult ResetPassword(string? userId, string? token)
        {
            if (userId == null || token == null)
            {
                TempData["resetPasswordAlert"] = "Geçersiz kullanıcı yada zaman aşımı gerçekleşti!";
                return RedirectToAction(nameof(ForgetPassword));
            }

            TempData["userId"] = userId;
            TempData["token"] = token;

            IEnumerable<IdentityError>? errors = HttpContext.Session.GetSession<IEnumerable<IdentityError>>("resetPasswordAlerts");
            if (errors != null)
            {
                ModelState.AddModelErrorListWithOutKey(errors);
                HttpContext.Session.Remove("resetPasswordAlerts");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ForgetPasswordViewModel request)
        {
            ModelState.Remove("Email");
            if (!ModelState.IsValid) return View();

            var userId = TempData["userId"];
            var token = TempData["token"];

            if(userId == null || token == null)
            {
                TempData["resetPasswordAlert"] = "Geçersiz kullanıcı yada zaman aşımı gerçekleşti!";
                return RedirectToAction(nameof(ForgetPassword));
            }

            AppUser? appUser = await _userManager.FindByIdAsync(userId.ToString());
            if(appUser == null)
            {
                TempData["resetPasswordAlert"] = "Kullanıcı bulunamadı";
                return RedirectToAction(nameof(ForgetPassword));
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(appUser, token.ToString(), request.NewPassword);
            if(!result.Succeeded)
            {
                HttpContext.Session.SetSession("resetPasswordAlerts", result.Errors);
                return RedirectToAction(nameof(ResetPassword), "Home", new { userId = userId!, token = token! });
            }

            TempData["success"] = "Şifreniz başarıyla yenilendi!";
            return RedirectToAction(nameof(Login));
        }
    }
}