using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.BLL.ManagerServices.Abstracts;
using Project.COMMON.Extensions;
using Project.ENTITIES.Enums;
using Project.ENTITIES.Models;
using Project.MVCUI.Models.ShoppingTools;
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

        [Route("/")]
        [Route("/Home")]
        [Route("/Shopping/ShoppingList")]
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

            wrapper.Categories = _categoryManager.GetActiveQueryableCategories().Select(x => new CategoryViewModel()
            {
                ID = x.ID,
                Name = x.Name
            }).ToHashSet();

            ViewBag.totalPagesCount = (int)Math.Ceiling((double)totalItemsCount / pageSize);
            ViewBag.pageNumber = pageNumber;

            return View(wrapper);
        }

        public IActionResult CartPage()
        {
            //Todo:Create view of this action
            Cart? basket = HttpContext.Session.GetSession<Cart>("cart");
            if(basket == null)
            {
                TempData["fail"] = "Sepetinizde ürün bulunmamaktadır!";
                return RedirectToAction(nameof(ShoppingList));
            }

            CardPageViewModel cardPageViewModel = new CardPageViewModel()
            {
                Cart = basket as Cart
            };

            return View(cardPageViewModel);
        }

        [HttpGet("{id}/{categoryID?}/{pageNumber?}/{from?}")]
        public IActionResult AddToCart(int id, int? categoryID, int? pageNumber, string? from)
        {
            Cart? basket = HttpContext.Session.GetSession<Cart>("cart");
            if (basket == null) basket = new Cart();

            Product toBeAdded = _productManager.Find(id)!;

            CartItem cartItem = new()
            {
                ID = toBeAdded.ID,
                Name = toBeAdded.Name,
                Price = toBeAdded.Price,
                ImagePath = toBeAdded.ImagePath
            };
            basket.AddToBasket(cartItem);
            HttpContext.Session.SetSession("cart", basket);


            TempData["success"] = "Ürün sepete eklendi";
            if (from != null && from == "cart") return RedirectToAction(nameof(CartPage));
            return RedirectToAction(nameof(ShoppingList), new { categoryID = categoryID, pageNumber = pageNumber });
        }

        [HttpGet("{id}")]
        public IActionResult DeleteFromCart(int id)
        {
            Cart? basket = HttpContext.Session.GetSession<Cart>("cart");

            if (basket == null) return RedirectToAction(nameof(ShoppingList));
            if (!basket.Basket.Any(x => x.ID == id)) return RedirectToAction(nameof(ShoppingList));

            basket.RemoveFromBasket(id);

            if (!basket.Basket.Any())
            {
                HttpContext.Session.Remove("cart");
                TempData["fail"] = "Sepetinizde ürün bulunmamaktadır!";
                return RedirectToAction(nameof(ShoppingList));
            }

            HttpContext.Session.SetSession("cart", basket);

            TempData["success"] = "Ürün sepetten silindi";
            return RedirectToAction(nameof(CartPage));
        }

        [HttpGet("{id}")]
        public IActionResult DeleteProductWithAllAmountFromCart(int id)
        {
            Cart? basket = HttpContext.Session.GetSession<Cart>("cart");

            if (basket == null) return RedirectToAction(nameof(ShoppingList));
            if (!basket.Basket.Any(x => x.ID == id)) return RedirectToAction(nameof(ShoppingList));

            basket.RemoveItemWithAllAmountFromBasket(id);

            if (!basket.Basket.Any())
            {
                HttpContext.Session.Remove("cart");
                TempData["fail"] = "Sepetinizde ürün bulunmamaktadır!";
                return RedirectToAction(nameof(ShoppingList));
            }

            HttpContext.Session.SetSession("cart", basket);

            TempData["success"] = "Ürün sepetten silindi";
            return RedirectToAction(nameof(CartPage));
        }
    }
}
