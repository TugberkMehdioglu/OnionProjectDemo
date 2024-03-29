﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.DAL.ContextClasses;
using Project.DAL.Repositories.Abstracts;
using Project.ENTITIES.Enums;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Repositories.Concretes
{
    public class AppUserRepository : BaseRepository<AppUser>, IAppUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AppUserRepository(MyContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> AddUserAsync(AppUser appUser)
        {
            string password = appUser.PasswordHash;
            IdentityResult result = await _userManager.CreateAsync(appUser, appUser.PasswordHash);

            if (result.Succeeded)
            {
                await _signInManager.PasswordSignInAsync(appUser, password, true, true);
                return (true, null);
            }
            return (false, result.Errors);
        }

        public async Task<AppUser?> GetUserWithProfileAsync(string userName) => await _context.AppUsers!.Where(x => x.UserName == userName && x.Status != DataStatus.Deleted).Include(x => x.AppUserProfile).FirstOrDefaultAsync();

        public async Task<(bool, IEnumerable<IdentityError>?)> EditUserWithOutPictureAsync(AppUser appUser)
        {
            AppUser newUser = await _userManager.FindByIdAsync(appUser.Id);
            newUser.Email = appUser.Email;
            newUser.UserName = appUser.UserName;
            newUser.PhoneNumber = appUser.PhoneNumber;
            newUser.ModifiedDate = DateTime.Now;
            newUser.Status = DataStatus.Updated;

            IdentityResult result = await _userManager.UpdateAsync(newUser);
            if (!result.Succeeded) return (false, result.Errors);

            IdentityResult securtiyStampResult = await _userManager.UpdateSecurityStampAsync(newUser);
            if (!securtiyStampResult.Succeeded) return (false, securtiyStampResult.Errors);

            await _signInManager.SignOutAsync();//Because SecurityStamp updated
            await _signInManager.SignInAsync(newUser, true);
            return (true, null);
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(AppUser user, string oldPassword, string newPassword)
        {
            IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded) return (false, result.Errors);

            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(user, newPassword, true, true);

            return (true, null);
        }
    }
}
