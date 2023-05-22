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
    public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly MyContext _context;

        protected BaseRepository(MyContext context)
        {
            _context = context;
        }

        protected void Save()
        {
            _context.SaveChanges();
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
            Save();
        }

        public void AddRange(ICollection<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            Save();
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Any(expression);
        }

        public void Delete(T entity)
        {
            entity.Status = DataStatus.Deleted;
            entity.DeletedDate = DateTime.Now;
            Save();
        }

        public void DeleteRange(ICollection<T> entities)
        {
            foreach (T item in entities)
            {
                Delete(item);
            }
        }

        public void Destroy(T entity)
        {
            _context.Set<T>().Remove(entity);
            Save();
        }

        public void DestroyRange(ICollection<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            Save();
        }

        public T? Find(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public T? FindByString(string id)
        {
            return _context.Set<T>(id).Find(id);
        }

        public T? FindFirstData()
        {
            return _context.Set<T>().OrderBy(x => x.CreatedDate).FirstOrDefault();
        }

        public T? FindLastData()
        {
            return _context.Set<T>().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        }

        public T? FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().FirstOrDefault(expression);
        }

        public IEnumerable<T> GetActives()
        {
            return Where(x => x.Status != DataStatus.Deleted);
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public IEnumerable<T> GetModifieds()
        {
            return Where(x => x.Status == DataStatus.Updated);
        }

        public IEnumerable<T> GetPassives()
        {
            return Where(x => x.Status == DataStatus.Deleted);
        }

        public object Select(Expression<Func<T, object>> expression)
        {
            return _context.Set<T>().Select(expression);
        }

        public X? Select<X>(Expression<Func<T, X>> expression) where X : class
        {
            return _context.Set<T>().Select(expression).FirstOrDefault();
        }

        public void Update(T entity)
        {
            entity.Status = DataStatus.Updated;
            entity.ModifiedDate = DateTime.Now;
            T toBeUpdated = Find(entity.ID)!;
            _context.Entry(toBeUpdated).CurrentValues.SetValues(entity);
            Save();
        }

        public void UpdateRange(ICollection<T> entities)
        {
            foreach (T item in entities)
            {
                Update(item);
            }
        }

        public IEnumerable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }
    }
}
