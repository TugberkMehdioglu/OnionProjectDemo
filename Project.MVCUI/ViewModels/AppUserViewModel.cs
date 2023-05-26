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
        [StringLength(40, MinimumLength = 2, ErrorMessage = "{0} {2} ile {1} karakter arası olmalıdır")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [EmailAddress(ErrorMessage = "{0} formatı yanlıştır")]
        [MaxLength(80, ErrorMessage ="{0} alanı en fazla {1} karakter olabilir")]
        [DataType(DataType.Text)]
        public string Email { get; set; } = null!;

        [Display(Name = "Telefon")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [RegularExpression(@"^[0-9]{11}", ErrorMessage ="Cep numaranızı başında 0 olarak ve 11 haneli yazınız")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [MinLength(6, ErrorMessage = "Şifre en az {1} karakter olabilir")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; } = null!;

        [Display(Name = "Şifre Tekrar")]
        [Required(ErrorMessage = "{0} boş bırakılamaz")]
        [Compare(nameof(PasswordHash), ErrorMessage = "Girmiş olduğunuz şifreler uyuşmuyor")]
        [MinLength(6, ErrorMessage = "Şifre en az {1} karakter olabilir")]
        [StringLength(40, MinimumLength =6, ErrorMessage ="{0} {2} ile {1} karakter arası olmalıdır")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; } = null!;
    }
}
