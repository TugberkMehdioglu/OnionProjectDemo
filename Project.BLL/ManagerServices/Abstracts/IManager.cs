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
        (bool, string?) Add(T entity);
        (bool, string?) AddRange(ICollection<T> entities);
        (bool, string?) Update(T entity);
        (bool, string?) UpdateRange(ICollection<T> entities);
        (bool, string?) Delete(T entity);
        (bool, string?) DeleteRange(ICollection<T> entities);
        (bool, string?) Destroy(T entity);
        (bool, string?) DestroyRange(ICollection<T> entities);

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
