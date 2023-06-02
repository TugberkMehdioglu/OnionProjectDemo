using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.BLL.ManagerServices.Abstracts;
using Project.ENTITIES.Enums;
using Project.ENTITIES.Models;
using Project.MVCUI.Extensions;
using Project.MVCUI.ViewModels;
using Project.MVCUI.ViewModels.WrapperClasses;

namespace Project.MVCUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[Area]/[Controller]/[Action]")]
    [Authorize(Roles ="Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryManager _categoryManager;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryManager categoryManager, IMapper mapper)
        {
            _categoryManager = categoryManager;
            _mapper = mapper;
        }

        [HttpGet("{id?}")]
        public IActionResult Index(int? id)
        {
            CategoryWrapper categoryWrapper = id == null ? new CategoryWrapper()
            {
                Categories = _categoryManager.GetActives().Select(x => new CategoryViewModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                }).ToHashSet()
            }
            : new CategoryWrapper()
            {
                Categories = _categoryManager.Where(x => x.ID == id && x.Status != DataStatus.Deleted).Select(x => new CategoryViewModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                }).ToHashSet()
            };

            return View(categoryWrapper);
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(CategoryViewModel request)
        {
            if (!ModelState.IsValid) return View(request);

            if (_categoryManager.Any(x => x.Name == request.Name && x.Status != DataStatus.Deleted)) 
            {
                ModelState.AddModelErrorWithOutKey("Oluşturmaya çalıştığınız kategori zaten bulunmaktadır!");
                return View();
            }

            var (isSuccess, error) = _categoryManager.Add(_mapper.Map<Category>(request));
            if(error != null)
            {
                ModelState.AddModelErrorWithOutKey(error);
                return View();
            }

            TempData["success"] = "Kategori başarıyla eklendi";
            return RedirectToAction(nameof(Index), "Category", new { Area = "Admin" });
        }

        [HttpGet("{id}")]
        public IActionResult UpdateCategory(int id)
        {
            Category? category=_categoryManager.Find(id);
            if(category == null) return RedirectToAction(nameof(Index), "Category", new { Area = "Admin" });

            CategoryViewModel viewModel = _mapper.Map<CategoryViewModel>(category);
            viewModel.FormerName = category.Name;

            return View(viewModel);
        }

        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCategory(CategoryViewModel request)
        {
            if(!ModelState.IsValid) return View(request);

            if (request.FormerName != request.Name && _categoryManager.Any(x => x.Name == request.Name && x.Status != DataStatus.Deleted))
            {
                ModelState.AddModelErrorWithOutKey("Güncellemek istediğiniz kategori adı zaten mevcut!");
                return View();
            }

            var (isSuccess, error) = _categoryManager.Update(_mapper.Map<Category>(request));
            if(error != null )
            {
                ModelState.AddModelErrorWithOutKey(error);
                return View();
            }

            TempData["success"] = "Kategori başarıyla güncellendi";
            return RedirectToAction(nameof(Index), "Category", new { Area = "Admin" });
        }

        [HttpGet("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            Category? category = _categoryManager.Find(id);
            if (category == null) return Json(new { message = "Kategori bulunamadı!" });

            _categoryManager.Delete(category);

            return Json(new { message = "Kategori başarıyla silindi" });
        }
    }
}
