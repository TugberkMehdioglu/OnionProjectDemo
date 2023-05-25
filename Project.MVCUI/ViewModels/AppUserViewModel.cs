using Project.ENTITIES.Models;

namespace Project.MVCUI.ViewModels
{
    public class AppUserViewModel
    {
        public string? ID { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
