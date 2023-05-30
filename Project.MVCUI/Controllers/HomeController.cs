using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.BLL.ManagerServices.Abstracts;
using Project.COMMON.Extensions;
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

            //AppUserProfile? userProfile = _appUserProfileManager.FindByString(user.Id);
            //if (userProfile != null)
            //{
            //    SessionViewModel session = new SessionViewModel() { ImagePath = userProfile.ImagePath };
            //    HttpContext.Session.SetSession("sessionVM", session);
            //    SessionViewModel? model = HttpContext.Session.GetSession<SessionViewModel>("sessionVM");
            //}


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
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}