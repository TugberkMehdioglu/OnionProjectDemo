namespace Project.MVCUI.ViewModels
{
    public class OrderViewModel
    {
        public int? ID { get; set; }
        public string ShippedAddress { get; set; } = null!;
        public decimal TotalPrice { get; set; }
    }
}
