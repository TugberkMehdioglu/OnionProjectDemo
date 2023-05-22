using Project.ENTITIES.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Repositories.Abstracts
{
    public interface IRepository<T> where T : IEntity
    {
        //List Commands
        IEnumerable<T> GetAll();
        IEnumerable<T> GetActives();
        IEnumerable<T> GetModifieds();
        IEnumerable<T> GetPassives();

        //Modify Commands
        void Add(T entity);
        void AddRange(ICollection<T> entities);
        void Update(T entity);
        void UpdateRange(ICollection<T> entities);
        void Delete(T entity);
        void DeleteRange(ICollection<T> entities);
        void Destroy(T entity);
        void DestroyRange(ICollection<T> entities);

        //Expression Commands
        IEnumerable<T> Where(Expression<Func<T, bool>> expression);
        bool Any(Expression<Func<T, bool>> expression);
        T? FirstOrDefault(Expression<Func<T, bool>> expression);
        object Select(Expression<Func<T, object>> expression);
        X? Select<X>(Expression<Func<T, X>> expression) where X : class; //For DTO classes without unboxing

        //Find Commands
        T? Find(int id);
        T? FindByString(string id);
        T? FindFirstData();
        T? FindLastData();
    }
}
