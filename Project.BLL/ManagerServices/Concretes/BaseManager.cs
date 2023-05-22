using Project.BLL.ManagerServices.Abstracts;
using Project.DAL.Repositories.Abstracts;
using Project.ENTITIES.Enums;
using Project.ENTITIES.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.ManagerServices.Concretes
{
    public class BaseManager<T> : IManager<T> where T : class, IEntity
    {
        protected readonly IRepository<T> _repository;

        public BaseManager(IRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual string Add(T entity)
        {
            if (entity == null || entity.Status == DataStatus.Deleted) return "Lütfen zorunlu alanları doldurunuz";

            try
            {
                _repository.Add(entity);
            }
            catch (Exception exception)
            {
                return $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}";
            }
            return "İşlem başarılı";
        }

        public virtual string AddRange(ICollection<T> entities)
        {
            if (entities == null || entities.Count < 1) return "Lütfen zorunlu alanları doldurunuz";

            try
            {
                _repository.AddRange(entities);
            }
            catch (Exception exception)
            {
                return $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}";
            }
            return "İşlem başarılı";
        }

        public virtual bool Any(Expression<Func<T, bool>> expression)
        {
            return _repository.Any(expression);
        }

        public virtual string Delete(T entity)
        {
            if (entity == null || entity.Status == DataStatus.Deleted) return "Lütfen zorunlu alanları doldurunuz";

            try
            {
                _repository.Delete(entity);
            }
            catch (Exception exception)
            {
                return $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}";
            }
            return "İşlem başarılı";
        }

        public virtual string DeleteRange(ICollection<T> entities)
        {
            if (entities == null || entities.Count < 1) return "Lütfen zorunlu alanları doldurunuz";

            try
            {
                _repository.DeleteRange(entities);
            }
            catch (Exception exception)
            {
                return $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}";
            }
            return "İşlem başarılı";
        }

        public virtual string Destroy(T entity)
        {
            if (entity == null) return "Lütfen zorunlu alanları doldurunuz";

            try
            {
                _repository.Destroy(entity);
            }
            catch (Exception exception)
            {
                return $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}";
            }
            return "İşlem başarılı";
        }

        public virtual string DestroyRange(ICollection<T> entities)
        {
            if (entities == null || entities.Count < 1) return "Lütfen zorunlu alanları doldurunuz";

            try
            {
                _repository.DestroyRange(entities);
            }
            catch (Exception exception)
            {
                return $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}";
            }
            return "İşlem başarılı";
        }

        public virtual T? Find(int id)
        {
            return _repository.Find(id);
        }

        public virtual T? FindByString(string id)
        {
            return _repository.FindByString(id);
        }

        public virtual T? FindFirstData()
        {
            return _repository.FindFirstData();
        }

        public virtual T? FindLastData()
        {
            return _repository.FindLastData();
        }

        public virtual T? FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return _repository.FirstOrDefault(expression);
        }

        public virtual IEnumerable<T> GetActives()
        {
            return _repository.GetActives();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _repository.GetAll();
        }

        public virtual IEnumerable<T> GetModifieds()
        {
            return _repository.GetModifieds();
        }

        public virtual IEnumerable<T> GetPassives()
        {
            return _repository.GetPassives();
        }

        public virtual object Select(Expression<Func<T, object>> expression)
        {
            return _repository.Select(expression);
        }

        public virtual X? Select<X>(Expression<Func<T, X>> expression) where X : class
        {
            return _repository.Select<X>(expression);
        }

        public virtual string Update(T entity)
        {
            if (entity == null || entity.Status == DataStatus.Deleted) return "Lütfen zorunlu alanları doldurunuz";

            try
            {
                _repository.Update(entity);
            }
            catch (Exception exception)
            {
                return $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}";
            }
            return "İşlem başarılı";
        }

        public virtual string UpdateRange(ICollection<T> entities)
        {
            if (entities == null || entities.Count < 1) return "Lütfen zorunlu alanları doldurunuz";

            try
            {
                _repository.UpdateRange(entities);
            }
            catch (Exception exception)
            {
                return $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}";
            }
            return "İşlem başarılı";
        }

        public virtual IEnumerable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _repository.Where(expression);
        }
    }
}
