using Project.ENTITIES.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Project.MVCUI.ViewModels
{
    public class AppUserViewModel
    {
        public string? ID { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [EmailAddress(ErrorMessage = "{0} formatı yanlıştır")]
        public string Email { get; set; } = null!;

        [Display(Name = "Telefon")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [MinLength(6, ErrorMessage = "Şifre en az {1} karakter olabilir")]
        public string PasswordHash { get; set; } = null!;

        [Display(Name = "Şifre Tekrar")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [Compare(nameof(PasswordHash), ErrorMessage = "Girmiş olduğunuz şifreler uyuşmuyor")]
        [MinLength(6, ErrorMessage = "Şifre en az {1} karakter olabilir")]
        public string PasswordConfirm { get; set; } = null!;
    }
}
