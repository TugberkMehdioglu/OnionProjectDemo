using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Project.MVCUI.ViewModels
{
    public class AppUserProfileViewModel
    {
        public string? ID { get; set; }

        [Display(Name = "İsim")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [StringLength(100, MinimumLength =2, ErrorMessage ="{0} alanı {2} ile {1} karakter arasında olmalıdır")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Soyisim")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} alanı {2} ile {1} karakter arasında olmalıdır")]
        public string LastName { get; set; } = null!;

        public string FullName { get { return $"{FirstName} {LastName}"; } }

        [Display(Name = "Adres")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "{0} alanı {2} ile {1} karakter arasında olmalıdır")]
        public string Address { get; set; } = null!;

        public string? ImagePath { get; set; }

        [ValidateNever]
        public AppUserViewModel? AppUser { get; set; }
    }
}
