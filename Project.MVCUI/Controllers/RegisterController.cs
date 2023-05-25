using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.BLL.ManagerServices.Abstracts;
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
        private readonly IAppUserManager _appUserManager;
        private readonly IAppUserProfileManager _appUserProfileManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public RegisterController(IMapper mapper, IAppUserManager appUserManager, IAppUserProfileManager appUserProfileManager, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _appUserManager = appUserManager;
            _appUserProfileManager = appUserProfileManager;
            _userManager = userManager;
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

            user.Id = Guid.NewGuid().ToString(); //https://stackoverflow.com/questions/59134406/unable-to-track-an-entity-of-type-because-primary-key-property-id-is-null
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

            string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string confirmationLink = Url.Action("Activation", "Register", new { userId = user.Id, token = confirmationToken }, HttpContext.Request.Scheme)!;
            string message = $"Tebrikler, hesabınız oluşturulmuştur. Hesabınızı aktive etmek için {confirmationLink} linkine tıklayabilirsiniz.";

            MailService.SendMailAsync(request.AppUser!.Email, message, "Hesap aktivasyon | Onion Project");

            profile.ID = user.Id;
            var (success, alert) = _appUserProfileManager.Add(profile);
            if (!success)
            {
                ModelState.AddModelErrorWithOutKey(alert!);
                return View();
            }

            ViewBag.message = "Üyeliğiniz oluşturulmuştur, üyelik işlemlerini tamamlamak için mail adresinize yolladığımız talimatları izleyiniz";
            return View();
        }

        public async Task<IActionResult> Activation(string userId, string token)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ModelState.AddModelErrorWithOutKey("Kullanıcı bulunamadı");
                return View();
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorListWithOutKey(result.Errors);
                return View();
            }
            
            ViewBag.success = "Email onaylama başarılı";
            return View();
        }
    }
}
