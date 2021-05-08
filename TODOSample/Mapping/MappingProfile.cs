using AutoMapper;
using DAL;
using Services;

namespace TODOSample.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Item, ItemModel>();
            CreateMap<ItemModel, Item>();

            CreateMap<UserModel, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));
        }
    }
}
