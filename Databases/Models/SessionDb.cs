using System;
using System.Collections.Generic;

namespace ToqueToqueApi.Databases.Models
{
    public class SessionDb
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public int AvailableTickets { get; set; }
        public int Creator { get; set; }
        public DateTime EventStarting { get; set; }
        public DateTime RegisteringStop { get; set; }
        public List<SessionMealDb> SessionMeals { get; set; }
        public DateTime Created { get; set; }
        public List<BookingStateSessionUserDb> BookingStateSessionUser { get; set; }
        public GeolocationDb Geolocation { get; set; }
    }
}