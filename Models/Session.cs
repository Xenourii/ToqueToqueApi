using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ToqueToqueApi.Infrastructures;
using ToqueToqueApi.Models.Meals;

namespace ToqueToqueApi.Models
{
    public class Session
    {
        public int Id { get; set; }

        [Sortable]
        [FiltrableString]
        public string Title { get; set; }

        [FiltrableString]
        public string Description { get; set; }

        public string Address { get; set; }

        [Required]
        [Range(0, double.PositiveInfinity)]
        [Sortable]
        [FiltrableDecimal]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int AvailableTickets { get; set; }

        [FiltrableInteger]
        public int Creator { get; set; }
        public Profile CreatorProfile { get; set; }

        public List<Meal> Meals { get; set; }
        public List<int> MealsId { get; set; }

        [Sortable(Default = true)]
        [FiltrableDateTime]
        public DateTime EventStarting { get; set; }

        public DateTime RegisteringStop { get; set; }
        public DateTime Created { get; set; }

        [Required]
        public Geolocation Geolocation { get; set; }

        public double? DistanceInMeters { get; set; }

        public int? BookingState { get; set; }
    }
}
