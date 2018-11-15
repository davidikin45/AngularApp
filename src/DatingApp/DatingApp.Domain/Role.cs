using System.ComponentModel.DataAnnotations;

namespace DatingApp.Domain
{
    public enum Role
    {
        [Display(Name = "Anonymous")]
        anonymous,
        [Display(Name = "Authenticated")]
        authenticated,
        [Display(Name = "Administrator")]
        administrator
    }
}
