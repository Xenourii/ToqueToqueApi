namespace ToqueToqueApi.Databases.Models
{
    public class AllergenUserDb
    {
        public int AllergenId { get; set; }
        public AllergenDb Allergen { get; set; }
        public int UserId { get; set; }
        public UserDb User { get; set; }
    }
}