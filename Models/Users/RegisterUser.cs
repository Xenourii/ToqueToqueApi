using System;
using System.ComponentModel.DataAnnotations;
using ToqueToqueApi.ValidationAttributes;

namespace ToqueToqueApi.Models.Users
{
    public class RegisterUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [BirthdayDateTime]
        public DateTime BirthDate { get; set; }

        public string ProfilePicture { get; set; }

        [Required]
        public string Password { get; set; }
    }
}