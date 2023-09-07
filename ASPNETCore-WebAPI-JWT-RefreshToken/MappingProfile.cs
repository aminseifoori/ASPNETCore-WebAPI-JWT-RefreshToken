using ASPNETCore_WebAPI_JWT_RefreshToken.Models;
using AutoMapper;
using Shared.Dtos.Users;

namespace StandardWebApiTemplate
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<User, CreateUserDto>().ReverseMap();
        }

    }
}
