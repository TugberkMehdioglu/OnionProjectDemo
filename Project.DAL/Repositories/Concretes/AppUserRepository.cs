using Microsoft.AspNetCore.Identity;
using Project.DAL.ContextClasses;
using Project.DAL.Repositories.Abstracts;
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
    }
}
