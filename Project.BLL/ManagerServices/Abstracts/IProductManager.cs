using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.ManagerServices.Abstracts
{
    public interface IProductManager : IManager<Product>
    {
        public (bool, string?, List<Product>?) GetActivesWithCategories();
        public (bool, string?, List<Product>?) GetProductsWithCategories(Expression<Func<Product, bool>> whereExpression);
        public (bool, string?, Product?) GetActiveProductWithCategory(int id);
        public IQueryable<Product> GetActiveQueryableProducts();
    }
}
