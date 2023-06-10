namespace Project.MVCUI.ViewModels.WrapperClasses
{
    public class ShoppingListWrapper
    {
        public ICollection<ProductViewModel>? Products { get; set; }
        public ICollection<CategoryViewModel>? Categories { get; set; }
    }
}
