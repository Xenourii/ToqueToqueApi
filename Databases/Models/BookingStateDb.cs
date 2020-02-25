
using System.Collections.Generic;

namespace ToqueToqueApi.Databases.Models
{
    /// <summary>
    /// Etat des réservations (en attente, accepté, refusé...)
    /// </summary>
    public class BookingStateDb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BookingStateSessionUserDb> BookingStateSessionUser { get; set; }
    }
}
