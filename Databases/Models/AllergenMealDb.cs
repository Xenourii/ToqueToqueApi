namespace ToqueToqueApi.Databases.Models
{
    public class AllergenMealDb
    {
        public int AllergenId { get; set; }
        public AllergenDb Allergen { get; set; }
        public int MealId { get; set; }
        public MealDb Meal { get; set; }
    }
}
