using System.ComponentModel.DataAnnotations;

namespace AspNetCore.ApiBase.Users
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
