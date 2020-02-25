
using System.Collections.Generic;

namespace ToqueToqueApi.Databases.Models
{
    /// <summary>
    /// Etat des r�servations (en attente, accept�, refus�...)
    /// </summary>
    public class BookingStateDb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BookingStateSessionUserDb> BookingStateSessionUser { get; set; }
    }
}
