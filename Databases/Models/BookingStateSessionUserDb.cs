namespace ToqueToqueApi.Databases.Models
{
    /// <summary>
    /// Lien entre les états de réservation, les sessions et les utilisateurs
    /// </summary>
    public class BookingStateSessionUserDb
    {
        public int BookingStateId { get; set; }
        public BookingStateDb BookingState { get; set; }

        public int SessionId { get; set; }
        public SessionDb Session { get; set; }

        public int UserId { get; set; }
        public UserDb User { get; set; }
    }
}