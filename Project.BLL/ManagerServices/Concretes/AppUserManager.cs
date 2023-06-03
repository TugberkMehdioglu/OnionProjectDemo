using Microsoft.AspNetCore.Identity;
using Project.BLL.ManagerServices.Abstracts;
using Project.DAL.Repositories.Abstracts;
using Project.ENTITIES.Enums;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.ManagerServices.Concretes
{
    public class AppUserManager : BaseManager<AppUser>, IAppUserManager
    {
        private readonly IAppUserRepository _appUserRepository;
        public AppUserManager(IRepository<AppUser> repository, IAppUserRepository appUserRepository) : base(repository)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task<(bool, IEnumerable<IdentityError>?, string?)> AddUserByIdentityAsync(AppUser user)
        {
            if (user == null || user.Status == DataStatus.Deleted || user.PasswordHash == null || user.Email == null || user.PhoneNumber == null || user.UserName == null) return (false, null, "Lütfen zorunlu alanları doldurunuz");

            var (isSuccess, errors) = await _appUserRepository.AddUserAsync(user);
            
            if (isSuccess) return (true, null, null);
            return (false, errors, null);
        }

        public async Task<(bool, string?, AppUser?)> GetUserWithProfileAsync(string userName)
        {
            AppUser? user;
            try
            {
                user = await _appUserRepository.GetUserWithProfileAsync(userName);
            }
            catch (Exception exception)
            {
                return (false, $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}", null);
            }

            if (user == null) return (false, "Kullanıcı bulunamadı", null);

            return (true, null, user);
        }
    }
}
