using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Project.MVCUI.ViewModels
{
    public class AppUserProfileViewModel
    {
        public string? ID { get; set; }

        [Display(Name = "İsim")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Soyisim")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Adres")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        public string Address { get; set; } = null!;
    }
}
