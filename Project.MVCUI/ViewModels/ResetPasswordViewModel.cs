using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Project.MVCUI.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "Eski Şifre")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "{0} {2} ile {1} karakter arası olmalıdır")]
        [DataType(DataType.Password)]
        public string FormerPassword { get; set; } = null!;

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "{0} {2} ile {1} karakter arası olmalıdır")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Display(Name = "Şifre Tekrar")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [Compare(nameof(NewPassword), ErrorMessage = "Girmiş olduğunuz şifreler uyuşmuyor")]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "{0} {2} ile {1} karakter arası olmalıdır")]
        [DataType(DataType.Password)]
        public string NewPasswordConfirm { get; set; } = null!;
    }
}
