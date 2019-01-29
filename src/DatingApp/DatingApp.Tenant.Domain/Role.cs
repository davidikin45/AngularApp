using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DatingApp.Tenant.Domain
{
    public enum Role
    {
        //[EnumMember(Value = "anonymous")]
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
