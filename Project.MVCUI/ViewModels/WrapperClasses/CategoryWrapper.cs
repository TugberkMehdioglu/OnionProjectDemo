using Project.ENTITIES.Models;

namespace Project.MVCUI.ViewModels.WrapperClasses
{
    public class CategoryWrapper
    {
        public CategoryViewModel? Category { get; set; }
        public ICollection<CategoryViewModel>? Categories { get; set; }
    }
}
