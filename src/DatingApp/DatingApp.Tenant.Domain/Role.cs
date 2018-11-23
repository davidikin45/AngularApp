using System.ComponentModel.DataAnnotations;

namespace DatingApp.Tenant.Domain
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
