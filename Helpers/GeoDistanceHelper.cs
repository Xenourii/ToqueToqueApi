using System;
using ToqueToqueApi.Models;

namespace ToqueToqueApi.Helpers
{
    //Code from https://www.geodatasource.com/developers/c-sharp
    public static class GeoDistanceHelper
    {
        private const double RadiusOfEarthInMeters = 6378.137;
        private const double OneMeterInDegree = 1 / (2 * Math.PI / 360 * RadiusOfEarthInMeters) / 1000;

        public static double DistanceBetween(Geolocation geolocationOne, Geolocation geolocationTwo)
        {
            if (geolocationOne == geolocationTwo)
                return 0;

            var theta = geolocationOne.Longitude - geolocationTwo.Longitude;
            var dist = Math.Sin(ConvertDecimalDegreesToRadians(geolocationOne.Latitude))
                       * Math.Sin(ConvertDecimalDegreesToRadians(geolocationTwo.Latitude))
                       + Math.Cos(ConvertDecimalDegreesToRadians(geolocationOne.Latitude))
                       * Math.Cos(ConvertDecimalDegreesToRadians(geolocationTwo.Latitude))
                       * Math.Cos(ConvertDecimalDegreesToRadians(theta));
            dist = ConvertRadiansToDecimalDegrees(Math.Acos(dist));

            return dist * 60 * 1.1515 * 1.609344; //convert to Km
        }

        public static Geolocation FakeGeolocation(Geolocation originalGeolocation, int latitudeMeters, int longitudeMeters) =>
            new Geolocation
            {
                Latitude = originalGeolocation.Latitude + (latitudeMeters * OneMeterInDegree),
                Longitude = originalGeolocation.Longitude + (longitudeMeters * OneMeterInDegree) / Math.Cos(originalGeolocation.Latitude * (Math.PI / 180))
            };

        private static double ConvertDecimalDegreesToRadians(double deg) => (deg * Math.PI / 180.0);
        private static double ConvertRadiansToDecimalDegrees(double rad) => (rad / Math.PI * 180.0);
    }
}