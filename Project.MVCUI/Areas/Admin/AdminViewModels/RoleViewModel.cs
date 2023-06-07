using System.ComponentModel.DataAnnotations;

namespace Project.MVCUI.Areas.Admin.AdminViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; } = null!;

        [Display(Name ="Rol Adı")]
        [Required(ErrorMessage ="{0} alanı boş bırakılamaz")]
        public string Name { get; set; } = null!;
    }
}
