using Microsoft.EntityFrameworkCore;
using Project.DAL.ContextClasses;
using Project.DAL.Repositories.Abstracts;
using Project.ENTITIES.Enums;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Repositories.Concretes
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(MyContext context) : base(context)
        {
        }

        public List<Product> GetActivesWithCategories() => _context.Products!.Where(x => x.Status != ENTITIES.Enums.DataStatus.Deleted).Include(x => x.Category).ToList();

        public List<Product> GetProductsWithCategories(Expression<Func<Product, bool>> whereExpression) => _context.Products!.Where(whereExpression).Include(x => x.Category).ToList();

        public Product? GetActiveProductWithCategory(int id) => _context.Products!.Where(x => x.ID == id && x.Status != DataStatus.Deleted).Include(x => x.Category).FirstOrDefault();

        public IQueryable<Product> GetActiveQueryableProducts() => _context.Products!.Where(x => x.Status != DataStatus.Deleted);
    }
}
