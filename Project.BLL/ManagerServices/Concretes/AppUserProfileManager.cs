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
    public class AppUserProfileManager : BaseManager<AppUserProfile>, IAppUserProfileManager
    {
        private readonly IAppUserProfileRepository _appUserProfileRepository;
        public AppUserProfileManager(IRepository<AppUserProfile> repository, IAppUserProfileRepository appUserProfileRepository) : base(repository)
        {
            _appUserProfileRepository = appUserProfileRepository;
        }

        //Repository'deki override edilmiş Update method'u için bunu yaptık, yoksa BaseRepository'deki Update method'unu kullanmaya devam ediyordu.
        public override (bool, string?) Update(AppUserProfile entity)
        {
            if (entity == null || entity.Status == DataStatus.Deleted) return (false, "Lütfen zorunlu alanları doldurunuz");

            try
            {
                _appUserProfileRepository.Update(entity);
            }
            catch (Exception exception)
            {
                return (false, $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}");
            }
            return (true, null);

        }
    }
}
