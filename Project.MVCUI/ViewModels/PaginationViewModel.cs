using PagedList;

namespace Project.MVCUI.ViewModels
{
    //Similar to ProductViewModel but this class does pagination process
    public class PaginationViewModel
    {
        public ProductViewModel? Product { get; set; }
        public ICollection<ProductViewModel>? Products { get; set; }
        public IPagedList<ProductViewModel>? PagedProducts { get; set; } //pageable products

    }
}
