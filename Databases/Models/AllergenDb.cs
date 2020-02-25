using System.Collections.Generic;

namespace ToqueToqueApi.Databases.Models
{
    public class AllergenDb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AllergenMealDb> AllergenMeal { get; set; }
        public List<AllergenUserDb> AllergenUser { get; set; }
    }
}
