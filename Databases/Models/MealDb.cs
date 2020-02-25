using System;
using System.Collections.Generic;

namespace ToqueToqueApi.Databases.Models
{
    public class MealDb
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DifficultyDb Difficulty { get; set; }
        public ParticularityDb Particularity { get; set; }
        public List<string> Pictures { get; set; }
        public TimeSpan RealizationTime { get; set; }
        public string LinkToFullMeal { get; set; }
        public int OwnerId { get; set; }
        public List<AllergenMealDb> AllergenMeals { get; set; }
        public List<SessionMealDb> SessionMeals { get; set; }
    }
}
