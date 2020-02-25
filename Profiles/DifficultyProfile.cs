using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models;
using Profile = AutoMapper.Profile;

namespace ToqueToqueApi.Profiles
{
    public class DifficultyProfile : Profile
    {
        public DifficultyProfile()
        {
            CreateMap<Difficulty, DifficultyDb>();
            CreateMap<DifficultyDb, Difficulty>();
        }
    }
}