using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Project.BLL.ManagerServices.Abstracts;
using Project.ENTITIES.Enums;
using Project.ENTITIES.Models;
using Project.MVCUI.Areas.Admin.AdminViewModels;
using System.Data;

namespace Project.MVCUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[Area]/[Controller]/[Action]")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUserManager _appUserManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, IAppUserManager appUserManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _appUserManager = appUserManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> UserList()
        {
            string userName = User.Identity!.Name!;

            List<UserViewModel> users = await _userManager.Users.Where(x => x.Status != DataStatus.Deleted).Select(x => new UserViewModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            }).ToListAsync();

            foreach (UserViewModel item in users)
            {
                item.Roles = await _userManager.GetRolesAsync(new() { Id = item.Id, UserName = item.UserName });
            }

            return View(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            AppUser? appUser = await _userManager.FindByIdAsync(id);
            List<IdentityRole>? roles = await _roleManager.Roles.ToListAsync();
            IList<string> userRoles = await _userManager.GetRolesAsync(appUser);

            if (appUser == null)
            {
                TempData["fail"] = "Kullanıcı bulunamadı";
                return RedirectToAction(nameof(UserList));
            }
            TempData["userId"] = appUser.Id;

            List<AssignRoleToUserViewModel> userRolesViewModel= new List<AssignRoleToUserViewModel>();
            foreach (IdentityRole role in roles)
            {
                AssignRoleToUserViewModel roleViewModel = new AssignRoleToUserViewModel
                {
                    Id = role.Id,
                    Name = role.Name,
                };
                if (userRoles.Contains(role.Name)) roleViewModel.Exist = true;

                userRolesViewModel.Add(roleViewModel);
            }

            return View(userRolesViewModel);
        }

        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRoleToUser(List<AssignRoleToUserViewModel> requestList)
        {
            if (!ModelState.IsValid) return View();

            var userId = TempData["userId"];
            if(userId == null)
            {
                TempData["fail"] = "Atama yapılacak kullanıcı bulunamadı";
                return RedirectToAction(nameof(UserList));
            }

            AppUser? appUser = await _userManager.FindByIdAsync(userId.ToString());
            if (userId == null)
            {
                TempData["fail"] = "Atama yapılacak kullanıcı bulunamadı";
                return RedirectToAction(nameof(UserList));
            }

            foreach (AssignRoleToUserViewModel item in requestList)
            {
                if (item.Exist) await _userManager.AddToRoleAsync(appUser, item.Name);
                else await _userManager.RemoveFromRoleAsync(appUser, item.Name);
            }

            TempData["success"] = "Rol atama başarıyla gerçekleşti";
            return RedirectToAction(nameof(UserList));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            AppUser? appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null) return StatusCode(500, new { message = "Kullanıcı bulunamadı" });

            var (isSuccess, error) = _appUserManager.Delete(appUser);
            if (!isSuccess) return StatusCode(500, new { message = error });

            return Ok(new { message = "Kullanıcı başarıyla silindi" });
        }
    }
}
