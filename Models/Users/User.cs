using System;
using System.Collections.Generic;

namespace ToqueToqueApi.Models.Users
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string ProfilePicture { get; set; }
        public string Password { get; set; }
        public List<Allergen> Allergens { get; set; }
    }
}
