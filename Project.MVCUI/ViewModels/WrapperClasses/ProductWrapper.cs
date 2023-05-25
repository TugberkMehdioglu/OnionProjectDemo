namespace Project.MVCUI.ViewModels.WrapperClasses
{
    public class ProductWrapper
    {
        public ICollection<ProductViewModel>? Products { get; set; }
        public ProductViewModel? Product { get; set; }
        public ICollection<CategoryViewModel>? Categories { get; set; }

    }
}
