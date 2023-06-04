using AutoMapper;
using Project.ENTITIES.Models;
using Project.MVCUI.ViewModels;

namespace Project.MVCUI.Mapping
{
    public class ViewModelMapping : Profile
    {
        public ViewModelMapping()
        {
            CreateMap<AppUser, AppUserViewModel>().ReverseMap().ForMember(x => x.ID, opt => opt.Ignore());
            CreateMap<AppUserProfile, AppUserProfileViewModel>().ReverseMap();
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            CreateMap<Product, ProductViewModel>().ReverseMap();
        }
    }
}
