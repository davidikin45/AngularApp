using System.ComponentModel.DataAnnotations;

namespace DatingApp.Admin.Domain
{
    public enum Role
    {
        [Display(Name = "Anonymous")]
        anonymous,
        [Display(Name = "Authenticated")]
        authenticated,
        [Display(Name = "Read-Only")]
        read_only,
        [Display(Name = "Administrator")]
        administrator
    }
}
