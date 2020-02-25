namespace ToqueToqueApi.Databases.Models
{
    public class GeolocationDb
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public SessionDb Session { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}