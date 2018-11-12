using System.ComponentModel.DataAnnotations;

namespace AspNetCore.ApiBase.Users
{
    public class AuthenticateDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
