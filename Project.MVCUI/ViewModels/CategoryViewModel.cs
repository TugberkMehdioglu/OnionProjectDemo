using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Project.MVCUI.ViewModels
{
    public class CategoryViewModel
    {
        public int? ID { get; set; }

        [Display(Name = "Kategori Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0} {2} ile {1} karakter arası olmalıdır")]
        public string Name { get; set; } = null!;
        public string? FormerName { get; set; }

        [Display(Name = "Açıklama")]
        [MaxLength(400, ErrorMessage ="{0} alanı en fazla {1} karakter olabilir")]
        public string? Description { get; set; }
    }
}
