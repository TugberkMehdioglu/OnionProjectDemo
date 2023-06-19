using Project.DTO.Models;
using Project.ENTITIES.Models;

namespace Project.MVCUI.ViewModels.WrapperClasses
{
    public class OrderWrapper
    {
        public PaymentDTO PaymentDTO { get; set; } = null!;
        public AppUserProfileViewModel? AppUserProfile { get; set; }
        public AppUserViewModel? AppUser { get; set; }

    }
}
