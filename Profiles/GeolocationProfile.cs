using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models;
using Profile = AutoMapper.Profile;

namespace ToqueToqueApi.Profiles
{
    public class GeolocationProfile : Profile
    {
        public GeolocationProfile()
        {
            CreateMap<Geolocation, GeolocationDb>();
            CreateMap<GeolocationDb, Geolocation>();
            CreateMap<DistanceOptions, Geolocation>();
        }
    }
}