using AutoMapper;
using ToqueToqueApi.Databases.Models;

namespace ToqueToqueApi.Profiles
{
    public class ProfileProfile : Profile
    {
        public ProfileProfile()
        {
            CreateMap<UserDb, Models.Profile>();
        }
    }
}