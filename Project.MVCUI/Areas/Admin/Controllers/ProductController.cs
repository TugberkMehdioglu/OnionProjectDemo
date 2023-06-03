using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Project.BLL.ManagerServices.Abstracts;
using Project.BLL.ManagerServices.Concretes;
using Project.COMMON.Tools;
using Project.ENTITIES.Enums;
using Project.ENTITIES.Models;
using Project.MVCUI.Extensions;
using Project.MVCUI.ViewModels;
using Project.MVCUI.ViewModels.WrapperClasses;
using System.Data;

namespace Project.MVCUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[Area]/[Controller]/[Action]")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductManager _productManager;
        private readonly IMapper _mapper;
        private readonly ICategoryManager _categoryManager;
        private readonly IFileProvider _fileProvider;

        public ProductController(IProductManager productManager, IMapper mapper, ICategoryManager categoryManager, IFileProvider fileProvider)
        {
            _productManager = productManager;
            _mapper = mapper;
            _categoryManager = categoryManager;
            _fileProvider = fileProvider;
        }

        [HttpGet("{id?}")]
        public IActionResult Index(int? id)
        {
            if(id != null)
            {
                var (Success, error1, products1) = _productManager.GetProductsWithCategories(x => x.CategoryID == id);
                if (Success == false)
                {
                    ModelState.AddModelErrorWithOutKey(error1!);
                    return View();
                }

                HashSet<ProductViewModel> viewModels = _mapper.Map<HashSet<ProductViewModel>>(products1);
                return View(viewModels);
            }

            var (isSuccess, error, products) = _productManager.GetActivesWithCategories();
            if(isSuccess == false)
            {
                ModelState.AddModelErrorWithOutKey(error!);
                return View();
            }

            HashSet<ProductViewModel> productsViewModel = _mapper.Map<HashSet<ProductViewModel>>(products);
            return View(productsViewModel);
        }

        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var (isSuccess, error, product) = _productManager.GetActiveProductWithCategory(id);
            if (!isSuccess)
            {
                TempData["fail"] = error;
                return RedirectToAction(nameof(Index), "Product", new { Area = "Admin" });
            }

            ProductViewModel viewModel = _mapper.Map<ProductViewModel>(product);

            return View(viewModel);
        }

        // GET: ProductController/Create
        public IActionResult Create()
        {
            HashSet<CategoryViewModel>? categories = _categoryManager.GetActives().Select(x => new CategoryViewModel()
            {
                ID = x.ID,
                Name = x.Name
            }).ToHashSet();

            TempData["categoriesSelectList"] = new SelectList(categories, nameof(CategoryViewModel.ID), nameof(CategoryViewModel.Name));

            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel request)
        {
            if (!ModelState.IsValid)
            {
                AddCategoriesForProduct();

                return View();
            }
            else if(_productManager.Any(x => x.Name.ToLower() == request.Name.ToLower() && x.Status != DataStatus.Deleted))
            {
                AddCategoriesForProduct();
                ModelState.AddModelErrorWithOutKey("Oluşturmaya çalıştığınız ürün zaten bulunmaktadır!");
                return View();
            }

            Product product = _mapper.Map<Product>(request);

            if (request.Image != null && request.Image.Length > 0)
            {
                string? result = ImageUploader.UploadImageToProduct(request.Image!, _fileProvider, out string? entityImagePath);
                if (result != null)
                {
                    ModelState.AddModelErrorWithOutKey(result);
                    AddCategoriesForProduct();
                    return View();
                }
                product.ImagePath = entityImagePath;
            }

            var (isSuccess, error) = _productManager.Add(product);
            if (isSuccess == false)
            {
                ModelState.AddModelErrorWithOutKey(error!);
                return View(request);
            }

            TempData["success"] = "Ürün başarıyla eklendi";
            return RedirectToAction(nameof(Index), "Product", new { Area = "Admin" });
        }

        [HttpGet("{id}")]
        public IActionResult Edit(int id)
        {
            Product? product = _productManager.Find(id);

            if (product == null)
            {
                ModelState.AddModelErrorWithOutKey("Ürün bulunamadı");
                return RedirectToAction(nameof(Index), "Product", new { Area = "Admin" });
            }
            ProductViewModel productViewModel = _mapper.Map<ProductViewModel>(product);
            productViewModel.FormerName = product.Name;

            HashSet<CategoryViewModel> categories = _categoryManager.GetActives().Select(x => new CategoryViewModel()
            {
                ID = x.ID,
                Name = x.Name
            }).ToHashSet();

            TempData["categoriesSelectList"] = new SelectList(categories, nameof(CategoryViewModel.ID), nameof(CategoryViewModel.Name), nameof(productViewModel.CategoryID));

            return View(productViewModel);
        }

        
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductViewModel request)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Edit), "Product", new { Area = "Admin" });

            if (request.FormerName != request.Name && _productManager.Any(x => x.Name == request.Name && x.Status != DataStatus.Deleted))
            {
                ModelState.AddModelErrorWithOutKey("Güncellemek istediğiniz ürün adı zaten mevcut!");
                AddCategoriesForProduct();
                return View(request);
            }

            if (request.Image != null && request.Image.Length > 0)
            {
                string? result = ImageUploader.UploadImageToProduct(request.Image!, _fileProvider, out string? entityImagePath);
                if (result != null)
                {
                    ModelState.AddModelErrorWithOutKey(result);
                    AddCategoriesForProduct();
                    return View();
                }
                request.ImagePath = entityImagePath;
            }

            var (isSuccess, error) = _productManager.Update(_mapper.Map<Product>(request));
            if (isSuccess == false)
            {
                ModelState.AddModelErrorWithOutKey(error!);
                return View();
            }

            TempData["success"] = "Ürün başarıyla güncellendi";
            return RedirectToAction(nameof(Index), "Product", new { Area = "Admin" });
        }

        [HttpGet("{id}")]
        public IActionResult Delete(int id)
        {
            Product? product = _productManager.Find(id);
            if (product == null)
            {
                ModelState.AddModelErrorWithOutKey("Ürün bulunamadı");
                return RedirectToAction(nameof(Index), "Product", new { Area = "Admin" });
            }

            var (isSuccess, error) = _productManager.Delete(product);
            if (!isSuccess)
            {
                ModelState.AddModelErrorWithOutKey(error!);
                return RedirectToAction(nameof(Index), "Product", new { Area = "Admin" });
            }

            TempData["success"] = "Ürün başarıyla silindi";
            return RedirectToAction(nameof(Index), "Product", new { Area = "Admin" });
        }


        public void AddCategoriesForProduct()
        {
            HashSet<CategoryViewModel>? categories = _categoryManager.GetActives().Select(x => new CategoryViewModel()
            {
                ID = x.ID,
                Name = x.Name
            }).ToHashSet();

            TempData["categoriesSelectList"] = new SelectList(categories, nameof(CategoryViewModel.ID), nameof(CategoryViewModel.Name));
        }


        //For search product on Product.Index action method but never used
        [HttpGet("{value}")]
        public IActionResult GetProducts(string value)
        {
            var (success, error, products) = _productManager.GetProductsWithCategories(x => x.Name.ToLower() == value.ToLower() && x.Status != DataStatus.Deleted);

            if (!success)
            {
                ModelState.AddModelErrorWithOutKey("Ürün bulunamadı");
                return RedirectToAction(nameof(Index), "Product", new { Area = "Admin" });
            }

            HashSet<ProductViewModel> viewModels = _mapper.Map<HashSet<ProductViewModel>>(products);

            return View("Index", viewModels);
        }
    }
}
