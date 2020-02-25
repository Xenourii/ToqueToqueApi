using System;
using System.Collections.Generic;

namespace ToqueToqueApi.Databases.Models
{
    public class UserDb
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string ProfilePicture { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public List<AllergenUserDb> AllergenUser { get; set; }
        public List<BookingStateSessionUserDb> BookingStateSessionUser { get; set; }
        public bool IsEnable { get; set; }
        public List<ConversationUserDb> ConversationUser { get; set; }
    }
}
