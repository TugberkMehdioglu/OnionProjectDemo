namespace Project.MVCUI.ViewModels
{
    public class ProductViewModel
    {
        public int? ID { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public short Stock { get; set; }
        public string? ImagePath { get; set; }
    }
}
