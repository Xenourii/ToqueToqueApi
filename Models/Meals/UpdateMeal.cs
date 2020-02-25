using System.Collections.Generic;

namespace ToqueToqueApi.Models.Meals
{
    public class UpdateMeal
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Allergen> Allergens { get; set; }
        public Difficulty Difficulty { get; set; }
        public Particularity Particularity { get; set; }
        public List<string> Pictures { get; set; }
        public double RealizationTime { get; set; }
        public string LinkToFullMeal { get; set; }
    }
}