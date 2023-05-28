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
                Category = _categoryManager.Where(x => x.ID == id && x.Status != DataStatus.Deleted).Select(x => new CategoryViewModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                }).FirstOrDefault()
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
            var (isSuccess, error) = _categoryManager.Add(_mapper.Map<Category>(request));
            if(error != null)
            {
                ModelState.AddModelErrorWithOutKey(error);
                return View();
            }

            TempData["success"] = "Kategori başarıyla eklendi";
            return RedirectToAction(nameof(Index), "Category", new { Area = "Admin" });
        }

        public IActionResult UpdateCategory(int id)
        {
            Category? category=_categoryManager.Find(id);
            if(category == null) return RedirectToAction(nameof(Index), "Category", new { Area = "Admin" });

            return View(_mapper.Map<CategoryViewModel>(category));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCategory(CategoryViewModel request)
        {
            var (isSuccess, error) = _categoryManager.Update(_mapper.Map<Category>(request));
            if(error != null )
            {
                ModelState.AddModelErrorWithOutKey(error);
                return View();
            }

            TempData["success"] = "Kategori başarıyla güncellendi";
            return RedirectToAction(nameof(Index), "Category", new { Area = "Admin" });
        }

        public IActionResult DeleteCategory(int id)
        {
            Category? category = _categoryManager.Find(id);
            if (category == null) return RedirectToAction(nameof(Index), "Category", new { Area = "Admin" });

            _categoryManager.Delete(category);

            TempData["success"] = "Kategori başarıyla silindi";
            return RedirectToAction(nameof(Index), "Category", new { Area = "Admin" });
        }
    }
}
