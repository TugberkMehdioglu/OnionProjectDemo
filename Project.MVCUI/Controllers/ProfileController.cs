using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Project.BLL.ManagerServices.Abstracts;
using Project.COMMON.Extensions;
using Project.COMMON.Tools;
using Project.ENTITIES.Models;
using Project.MVCUI.Extensions;
using Project.MVCUI.ViewModels;
using Project.MVCUI.ViewModels.WrapperClasses;

namespace Project.MVCUI.Controllers
{
    [Authorize]
    [Route("[Controller]/[Action]")]
    public class ProfileController : Controller
    {
        private readonly IAppUserManager _appUserManager;
        private readonly IAppUserProfileManager _appUserProfileManager;
        private readonly IMapper _mapper;
        private readonly IFileProvider _fileProvider;
        private readonly UserManager<AppUser> _userManager;
        public ProfileController(IAppUserManager appUserManager, IAppUserProfileManager appUserProfileManager, IMapper mapper, IFileProvider fileProvider, UserManager<AppUser> userManager)
        {
            _appUserManager = appUserManager;
            _appUserProfileManager = appUserProfileManager;
            _mapper = mapper;
            _fileProvider = fileProvider;
            _userManager = userManager;
        }

        // GET: ProfileController/Details/5
        public async Task<IActionResult> Details()
        {
            string userName = User.Identity!.Name!;

            var (isSuccess, error, appUser) = await _appUserManager.GetUserWithProfileAsync(userName);
            if (!isSuccess)
            {
                TempData["fail"] = error;
                return RedirectToAction("Index", "Home");
            }

            AppUserViewModel viewModel = _mapper.Map<AppUserViewModel>(appUser);

            return View(viewModel);
        }

        
        public async Task<IActionResult> Edit()
        {
            var (isSuccess, error, appUser) = await _appUserManager.GetUserWithProfileAsync(User.Identity!.Name!);
            if(!isSuccess)
            {
                TempData["fail"] = error;
                return RedirectToAction("Index", "Home");
            }

            ProfileEditWrapper wrapper = new()
            {
                AppUser = _mapper.Map<AppUserViewModel>(appUser)
            };
            wrapper.AppUser!.Id = appUser!.Id;
            wrapper.AppUser!.AppUserProfile!.ID = appUser!.AppUserProfile!.ID;

            IEnumerable<IdentityError>? identityErrors = HttpContext.Session.GetSession<IEnumerable<IdentityError>>("identityErrors");
            if (identityErrors != null)
            {
                ModelState.AddModelErrorListWithOutKey(identityErrors);
                HttpContext.Session.Remove("identityErrors");
            }
            else if (TempData["error"] != null) ModelState.AddModelErrorWithOutKey(TempData["error"]!.ToString()!);
            else if (TempData["success"] != null) ModelState.AddModelErrorWithOutKey(TempData["success"]!.ToString()!);
            //else if (TempData["success"])

            return View(wrapper);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditWrapper request)
        {
            //Şifre Değiştir form tag'i başka bir Action'a yönleneceği için bu validation'ları pas geçtik
            //Bu validation'lar AppUserViewModel'da olduğundan alıyoruz, bunları başka bir action'da kullandığımızdan silmememiz lazım.
            ModelState.Remove("AppUser.PasswordHash");
            ModelState.Remove("AppUser.PasswordConfirm");

            if (!ModelState.IsValid) return View(request);

            AppUserProfile appUserProfile = _mapper.Map<AppUserProfile>(request.AppUser!.AppUserProfile);
            AppUser appUser = _mapper.Map<AppUser>(request.AppUser);

            if(request.AppUser!.Image != null && request.AppUser.Image.Length > 0)
            {
                string? result = ImageUploader.UploadImageToUser(request.AppUser.Image, _fileProvider, out string? entityImagePath);
                if(result != null)
                {
                    ModelState.AddModelErrorWithOutKey(result);
                    return View(request);
                }

                appUser.AppUserProfile!.ImagePath = entityImagePath;
            }

            var (isSuccess, error, errors) = await _appUserManager.EditUserWithOutPictureAsync(appUser);
            if (!isSuccess && errors != null)
            {
                ModelState.AddModelErrorListWithOutKey(errors);
                return View(request);
            }
            else if (!isSuccess && errors == null)
            {
                ModelState.AddModelErrorWithOutKey(error!);
                return View(request);
            }

            var (success, errorAlert) = _appUserProfileManager.Update(appUserProfile);
            if(!success)
            {
                ModelState.AddModelErrorWithOutKey(errorAlert!);
                return View(request);
            }

            TempData["success"] = "Profil başarıyla güncellendi";
            return RedirectToAction(nameof(Details), "Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ProfileEditWrapper request)
        {

            AppUser user = await _userManager.FindByNameAsync(User.Identity!.Name);

            if (!ModelState.IsValid) return View();


            if (!await _userManager.CheckPasswordAsync(user, request.PasswordChange!.FormerPassword))
            {
                TempData["error"] = "Eski şifreniz yanlış";
                return RedirectToAction(nameof(Edit), "Profile");
            }

            var (isSuccess, error, errors) = await _appUserManager.ChangePasswordAsync(user, request.PasswordChange!.FormerPassword, request.PasswordChange!.NewPassword);

            if (!isSuccess && errors != null)
            {
                HttpContext.Session.SetSession("identityErrors", errors);

                return RedirectToAction(nameof(Edit), "Profile");
            }
            else if(!isSuccess && error != null)
            {
                TempData["error"] = error;
                return RedirectToAction(nameof(Edit), "Profile", new { userName = user.UserName });
            }

            TempData["success"] = "Şifreniz başarıyla değiştirildi!";
            return RedirectToAction(nameof(Details), "Profile");
        }

    }
}
