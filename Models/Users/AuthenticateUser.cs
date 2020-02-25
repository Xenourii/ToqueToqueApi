using System.ComponentModel.DataAnnotations;

namespace ToqueToqueApi.Models.Users
{
    public class AuthenticateUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}