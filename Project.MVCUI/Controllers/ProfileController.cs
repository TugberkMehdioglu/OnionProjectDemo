using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.BLL.ManagerServices.Abstracts;
using Project.ENTITIES.Models;
using Project.MVCUI.ViewModels;

namespace Project.MVCUI.Controllers
{
    [Authorize]
    [Route("[Controller]/[Action]")]
    public class ProfileController : Controller
    {
        private readonly IAppUserManager _appUserManager;
        private readonly IAppUserProfileManager _appUserProfileManager;
        private readonly IMapper _mapper;
        public ProfileController(IAppUserManager appUserManager, IAppUserProfileManager appUserProfileManager, IMapper mapper)
        {
            _appUserManager = appUserManager;
            _appUserProfileManager = appUserProfileManager;
            _mapper = mapper;
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

        // GET: ProfileController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProfileController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
