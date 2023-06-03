using Project.BLL.ManagerServices.Abstracts;
using Project.DAL.Repositories.Abstracts;
using Project.ENTITIES.Interfaces;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.ManagerServices.Concretes
{
    public class ProductManager : BaseManager<Product>, IProductManager
    {
        private readonly IProductRepository _productRepository;
        public ProductManager(IRepository<Product> repository, IProductRepository productRepository) : base(repository)
        {
            _productRepository = productRepository;
        }

        public (bool, string?, List<Product>?) GetActivesWithCategories() 
        {
            List<Product> products;

            try
            {
                products = _productRepository.GetActivesWithCategories();
            }
            catch (Exception exception)
            {
                return (false, $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}", null);
            }
            return (true, null, products);
        }

        public (bool, string?, List<Product>?) GetProductsWithCategories(Expression<Func<Product, bool>> whereExpression)
        {
            List<Product> products;

            try
            {
                products = _productRepository.GetProductsWithCategories(whereExpression);
            }
            catch (Exception exception)
            {
                return (false, $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}", null);
            }
            return (true, null, products);
        }

        public(bool, string?, Product?) GetActiveProductWithCategory(int id)
        {
            Product? product;
            try
            {
                product = _productRepository.GetActiveProductWithCategory(id);
            }
            catch (Exception exception)
            {
                return (false, $"Veritabanı işlemi sırasında hata oluştu, alınan hata => {exception.Message}. İçeriği => {exception.InnerException}", null);
            }

            if (product == null) return (false, "Belirtilen ürün bulunamadı", null);

            return (true, null, product);
        }
    }
}
