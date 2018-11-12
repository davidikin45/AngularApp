using Microsoft.AspNetCore.Identity;

namespace DatingApp.Domain
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}
