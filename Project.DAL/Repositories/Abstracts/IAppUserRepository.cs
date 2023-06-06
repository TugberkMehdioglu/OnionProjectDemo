using Microsoft.AspNetCore.Identity;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Repositories.Abstracts
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        public Task<(bool, IEnumerable<IdentityError>?)> AddUserAsync(AppUser appUser);
        public Task<AppUser?> GetUserWithProfileAsync(string userName);
        public Task<(bool, IEnumerable<IdentityError>?)> EditUserWithOutPictureAsync(AppUser appUser);
        public Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(AppUser user, string oldPassword, string newPassword);
    }
}
