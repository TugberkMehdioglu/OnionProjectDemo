using Project.DAL.ContextClasses;
using Project.DAL.Repositories.Abstracts;
using Project.ENTITIES.Enums;
using Project.ENTITIES.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Repositories.Concretes
{
    public class BaseRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly MyContext _context;

        public BaseRepository(MyContext context)
        {
            _context = context;
        }

        protected void Save()
        {
            _context.SaveChanges();
        }

        public virtual void Add(T entity)
        {
            _context.Set<T>().Add(entity);
            Save();
        }

        public virtual void AddRange(ICollection<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            Save();
        }

        public virtual bool Any(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Any(expression);
        }

        public virtual void Delete(T entity)
        {
            entity.Status = DataStatus.Deleted;
            entity.DeletedDate = DateTime.Now;
            Save();
        }

        public virtual void DeleteRange(ICollection<T> entities)
        {
            foreach (T item in entities)
            {
                Delete(item);
            }
        }

        public virtual void Destroy(T entity)
        {
            _context.Set<T>().Remove(entity);
            Save();
        }

        public virtual void DestroyRange(ICollection<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            Save();
        }

        public virtual T? Find(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public virtual T? FindByString(string id)
        {
            return _context.Set<T>().Find(id);
        }

        public virtual T? FindFirstData()
        {
            return _context.Set<T>().OrderBy(x => x.CreatedDate).FirstOrDefault();
        }

        public virtual T? FindLastData()
        {
            return _context.Set<T>().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        }

        public virtual T? FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().FirstOrDefault(expression);
        }

        public virtual IEnumerable<T> GetActives()
        {
            return Where(x => x.Status != DataStatus.Deleted);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public virtual IEnumerable<T> GetModifieds()
        {
            return Where(x => x.Status == DataStatus.Updated);
        }

        public virtual IEnumerable<T> GetPassives()
        {
            return Where(x => x.Status == DataStatus.Deleted);
        }

        public virtual object Select(Expression<Func<T, object>> expression)
        {
            return _context.Set<T>().Select(expression);
        }

        public virtual X? Select<X>(Expression<Func<T, X>> expression) where X : class
        {
            return _context.Set<T>().Select(expression).FirstOrDefault();
        }

        public virtual void Update(T entity)
        {
            entity.Status = DataStatus.Updated;
            entity.ModifiedDate = DateTime.Now;
            T toBeUpdated = Find(entity.ID)!;
            _context.Entry(toBeUpdated).CurrentValues.SetValues(entity);
            Save();
        }

        public virtual void UpdateRange(ICollection<T> entities)
        {
            foreach (T item in entities)
            {
                Update(item);
            }
        }

        public virtual IEnumerable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }
    }
}
