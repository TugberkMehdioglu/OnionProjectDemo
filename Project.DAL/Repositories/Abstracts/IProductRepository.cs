using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Repositories.Abstracts
{
    public interface IProductRepository : IRepository<Product>
    {
        public List<Product> GetActivesWithCategories();
        public List<Product> GetProductsWithCategories(Expression<Func<Product, bool>> whereExpression);
        public Product? GetActiveProductWithCategory(int id);
    }
}
