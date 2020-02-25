using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ToqueToqueApi.ValidationAttributes;

namespace ToqueToqueApi.Models.Users
{
    public class UpdateUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [BirthdayDateTime]
        public DateTime BirthDate { get; set; }

        public string ProfilePicture { get; set; }
        public string Password { get; set; }
        public List<Allergen> Allergens { get; set; }
    }
}