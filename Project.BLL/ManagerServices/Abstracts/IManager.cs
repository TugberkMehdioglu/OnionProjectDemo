using Project.ENTITIES.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.ManagerServices.Abstracts
{
    public interface IManager<T> where T : class, IEntity
    {
        //List Commands
        IEnumerable<T> GetAll();
        IEnumerable<T> GetActives();
        IEnumerable<T> GetModifieds();
        IEnumerable<T> GetPassives();

        //Modify Commands
        string Add(T entity);
        string AddRange(ICollection<T> entities);
        string Update(T entity);
        string UpdateRange(ICollection<T> entities);
        string Delete(T entity);
        string DeleteRange(ICollection<T> entities);
        string Destroy(T entity);
        string DestroyRange(ICollection<T> entities);

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
