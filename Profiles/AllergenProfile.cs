using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models;
using Profile = AutoMapper.Profile;

namespace ToqueToqueApi.Profiles
{
    public class AllergenProfile : Profile
    {
        public AllergenProfile()
        {
            CreateMap<Allergen, AllergenDb>();
            CreateMap<AllergenDb, Allergen>();
        }
    }
}