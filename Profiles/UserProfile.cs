using AutoMapper;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models.Users;

namespace ToqueToqueApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterUser, UserDb>();
            CreateMap<UserDb, User>();
        }
    }
}