using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.BLL.ManagerServices.Concretes;
using Project.COMMON.Tools;
using Project.ENTITIES.Models;
using Project.MVCUI.Extensions;
using Project.MVCUI.ViewModels.WrapperClasses;

namespace Project.MVCUI.Controllers
{
    [Route("[Controller]/[Action]")]
    public class RegisterController : Controller
    {
        private readonly AppUserManager _appUserManager;
        private readonly AppUserProfileManager _appUserProfileManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public RegisterController(IMapper mapper, AppUserManager appUserManager, UserManager<AppUser> userManager, AppUserProfileManager appUserProfileManager)
        {
            _mapper = mapper;
            _appUserManager = appUserManager;
            _userManager = userManager;
            _appUserProfileManager = appUserProfileManager;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(AppUserWrapper request)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = _mapper.Map<AppUser>(request.AppUser);
            AppUserProfile profile = _mapper.Map<AppUserProfile>(request.AppUserProfile);

            var (isSuccess, errors, error) = await _appUserManager.AddUserByIdentityAsync(user);

            if(!isSuccess && errors != null)
            {
                ModelState.AddModelErrorListWithOutKey(errors);
                return View();
            }
            else if(!isSuccess && errors == null)
            {
                ModelState.AddModelErrorWithOutKey(error!);
                return View();
            }

            string emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string message = $"Tebrikler, hesabınız oluşturulmuştur. Hesabınızı aktive etmek için https://localhost:7117/Register/Activation/{emailToken} linkine tıklayabilirsiniz.";
            MailService.SendMailAsync(request.AppUser!.Email, message, "Hesap aktivasyon | Onion Project");

            string result = _appUserProfileManager.Add(profile);
            

            return View();
        }
    }
}
