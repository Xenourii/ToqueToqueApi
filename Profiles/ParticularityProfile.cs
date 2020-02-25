using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models;
using Profile = AutoMapper.Profile;

namespace ToqueToqueApi.Profiles
{
    public class ParticularityProfile : Profile
    {
        public ParticularityProfile()
        {
            CreateMap<Particularity, ParticularityDb>();
            CreateMap<ParticularityDb, Particularity>();
        }
    }
}