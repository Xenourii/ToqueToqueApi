using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToqueToqueApi.Models.Meals
{
    public class Meal
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        public List<int> Allergens { get; set; }
        public int Difficulty { get; set; }
        public int Particularity { get; set; }
        public List<string> Pictures { get; set; }
        public int RealizationTimeInMinutes { get; set; }
        public string LinkToFullMeal { get; set; }
    }
}
