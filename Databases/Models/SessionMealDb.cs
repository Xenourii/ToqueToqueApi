namespace ToqueToqueApi.Databases.Models
{
    public class SessionMealDb
    {
        public int SessionId { get; set; }
        public SessionDb Session { get; set; }
        public int MealId { get; set; }
        public MealDb Meal { get; set; }
    }
}