using System.ComponentModel.DataAnnotations;

namespace ToqueToqueApi.Models
{
    public sealed class DistanceOptions
    {
        [Range(10, 1000000, ErrorMessage = "MaxDistanceInMeter must be between 0 and 30.000")]
        public double? MaxDistanceInMeter { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double? Longitude { get; set; }

        public bool IsDistanceOptionsSet => MaxDistanceInMeter != null && IsGeolocationOptionSet;
        public bool IsGeolocationOptionSet => Latitude != null && Longitude != null;

        public double MaxDistanceInKm
        {
            get
            {
                if (MaxDistanceInMeter != null)
                    return (double) (MaxDistanceInMeter / 1000);

                return 0;
            }
        }
    }
}