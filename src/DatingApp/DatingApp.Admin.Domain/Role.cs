using System.ComponentModel.DataAnnotations;

namespace DatingApp.Admin.Domain
{
    public enum Role
    {
        [Display(Name = "Anonymous")]
        anonymous,
        [Display(Name = "Authenticated")]
        authenticated,
        [Display(Name = "Api")]
        api,
        [Display(Name = "Administrator")]
        administrator
    }
}
