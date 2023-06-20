using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using Project.BLL.ManagerServices.Abstracts;
using Project.COMMON.Extensions;
using Project.COMMON.Tools;
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
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUserManager _appUserManager;
        private readonly IAppUserProfileManager _appUserProfileManager;
        private readonly HttpClient _httpClient;
        public ShoppingController(IOrderManager orderManager, IProductManager productManager, ICategoryManager categoryManager, IOrderDetailManager orderDetailManager, IMapper mapper, UserManager<AppUser> userManager, IAppUserManager appUserManager, IAppUserProfileManager appUserProfileManager, HttpClient httpClient)
        {
            _orderManager = orderManager;
            _productManager = productManager;
            _categoryManager = categoryManager;
            _orderDetailManager = orderDetailManager;
            _mapper = mapper;
            _userManager = userManager;
            _appUserManager = appUserManager;
            _appUserProfileManager = appUserProfileManager;
            _httpClient = httpClient;
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

        
        public IActionResult ConfirmOrder()
        {
            if(User.Identity?.Name == null)
            {
                TempData["success"] = "Bu işlemi gerçekleştirebilmek için giriş yapmanız gerekmektedir.";
                return Challenge();
            }

            AppUser appUser = _appUserManager.Where(x => x.UserName == User.Identity.Name).Select(x => new AppUser() { Id = x.Id, PhoneNumber = x.PhoneNumber, Email = x.Email }).FirstOrDefault()!;
            AppUserProfile appUserProfile = _appUserProfileManager.FindByString(appUser.Id)!;

            OrderWrapper wrapper = new()
            {
                AppUser = _mapper.Map<AppUserViewModel>(appUser),
                AppUserProfile = _mapper.Map<AppUserProfileViewModel>(appUserProfile)
            };
            wrapper.AppUser.Id = appUser.Id;

            return View(wrapper);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ConfirmOrder(OrderWrapper request)
        {
            ModelState.Remove("AppUser.PasswordHash");
            ModelState.Remove("AppUser.PhoneNumber");
            ModelState.Remove("AppUser.PasswordConfirm");
            ModelState.Remove("AppUser.UserName");
            if (!ModelState.IsValid) return View();

            Cart? cart = HttpContext.Session.GetSession<Cart>("cart");
            if (cart == null)
            {
                TempData["fail"] = "Sepetinizde ürün bulunmamaktadır!";
                return RedirectToAction(nameof(ShoppingList));
            }

            Order order = new();
            order.TotalPrice = request.PaymentDTO.ShoppingPrice = cart.TotalPrice;

            //Middleware'de builder.Services.AddHttpClient(); koymayı unutma.
            string apiUrl = "https://localhost:7189/api/Payment/ReceivePayment";
            HttpResponseMessage postResponse;

            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(300); //API cevabını 300 saniye bekler, cevap gelmezse catch'e düşer.
                postResponse = await _httpClient.PostAsJsonAsync(apiUrl, request.PaymentDTO);
            }
            catch (Exception exception)
            {
                TempData["error"] = $"Banka bağlantıyı reddetti! Alınan hata => {exception.Message}. Hatanın içeriği => {exception.InnerException}";
                return RedirectToAction(nameof(ConfirmOrder));
            }

            if (postResponse.IsSuccessStatusCode)
            {
                //ViewBag.responseBody = await postResponse.Content.ReadAsStringAsync(); => geriye tip döndürürse, JsonConvert ile çevirebilirsin.

                order.AppUserID = request.AppUser!.Id!;
                order.ShippedAddress = request.AppUserProfile!.Address;
                _orderManager.Add(order);

                foreach (CartItem item in cart.Basket)
                {
                    OrderDetail orderDetail = new()
                    {
                        OrderID = order.ID,
                        ProductID = item.ID,
                        TotalPrice = item.SubTotal,
                        Quantity = item.Amount
                    };
                    _orderDetailManager.Add(orderDetail);

                    Product product = _productManager.Find(item.ID)!;
                    product.Stock -= item.Amount;
                    _productManager.Update(product);
                }

                MailService.SendMailAsync(request.AppUser!.Email, $"Siparişiniz başarıyla alındı, Toplam Tutar => {order.TotalPrice.ToString("C2")}", "OnionProject | Sipariş");
                TempData["success"] = "Siparişiniz bize ulaşmıştır, teşekkür ederiz";
                return RedirectToAction(nameof(ShoppingList));
            }
            else
            {
                //ViewBag.error = "Ödeme ile ilgili bir sorun oluştu, lütfen bankanızla iletişime geçin.";
                string error = await postResponse.Content.ReadAsStringAsync();
                TempData["error"] = $"Ödeme ile ilgili bir sorun oluştu, lütfen bankanız ile iletişime geçiniz. Alınan hata => {error}";
            }

            return RedirectToAction(nameof(ConfirmOrder));
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
            else if (from != null && from == "ProductDetail") return RedirectToAction(nameof(ProductDetail), new { id });
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

        [HttpGet("{id}")]
        public IActionResult ProductDetail(int id)
        {
            var (isSuccess, error, product) = _productManager.GetActiveProductWithCategory(id);
            if (!isSuccess)
            {
                TempData["fail"] = error;
                return RedirectToAction(nameof(ShoppingList));
            }

            ProductViewModel viewModel = _mapper.Map<ProductViewModel>(product);

            return View(viewModel);
        }
    }
}
