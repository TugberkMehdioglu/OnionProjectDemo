using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.BLL.ManagerServices.Abstracts;
using Project.MVCUI.ViewModels;
using Project.MVCUI.ViewModels.WrapperClasses;

namespace Project.MVCUI.Controllers
{
    [Route("[Controller]/[Action]")]
    public class ShoppingController : Controller
    {
        private readonly IOrderManager _orderManager;
        private readonly IProductManager _productManager;
        private readonly ICategoryManager _categoryManager;
        private readonly IOrderDetailManager _orderDetailManager;
        public ShoppingController(IOrderManager orderManager, IProductManager productManager, ICategoryManager categoryManager, IOrderDetailManager orderDetailManager)
        {
            _orderManager = orderManager;
            _productManager = productManager;
            _categoryManager = categoryManager;
            _orderDetailManager = orderDetailManager;
        }

        [HttpGet("{categoryID?}/{pageNumber?}/{pageSize?}")]
        public async Task<IActionResult> ShoppingList(int? categoryID, int pageNumber = 1, int pageSize = 9)
        {
            int totalItemsCount;
            if (categoryID != null)
            {
                ViewBag.CategoryID = categoryID;
                totalItemsCount = await _productManager.GetActiveQueryableProducts().Where(x => x.CategoryID == categoryID).CountAsync();
            }
            else totalItemsCount = await _productManager.GetActiveQueryableProducts().CountAsync();


            ShoppingListWrapper wrapper = new ShoppingListWrapper()
            {
                Products = categoryID == null ? _productManager.GetActiveQueryableProducts().Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => new ProductViewModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    ImagePath = x.ImagePath,
                    Price = x.Price,
                    Stock = x.Stock,
                    CategoryName = x.Category.Name,
                    CategoryID = x.CategoryID
                }).ToList()

                : _productManager.GetActiveQueryableProducts().Where(x => x.CategoryID == categoryID).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => new ProductViewModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    ImagePath = x.ImagePath,
                    Price = x.Price,
                    Stock = x.Stock,
                    CategoryName = x.Category.Name,
                    CategoryID = x.CategoryID
                }).ToList()
            };

            
            ViewBag.totalPagesCount = (int)Math.Ceiling((double)totalItemsCount / pageSize);
            ViewBag.pageNumber = pageNumber;

            return View(wrapper);
        }
    }
}
