﻿using Microsoft.AspNetCore.Identity;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.ManagerServices.Abstracts
{
    public interface IAppUserManager : IManager<AppUser>
    {
        public Task<(bool, IEnumerable<IdentityError>?, string?)> AddUserByIdentityAsync(AppUser user);
        public Task<(bool, string?, AppUser?)> GetUserWithProfileAsync(string userName);
        public Task<(bool, string?, IEnumerable<IdentityError>?)> EditUserWithOutPictureAsync(AppUser appUser);
    }
}
