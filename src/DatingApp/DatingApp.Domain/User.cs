using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DatingApp.Domain
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

        public static User CreateUser(IPasswordHasher<User> passwordHasher, string id, string name, string email, string password)
        {
            var user = new User();
            user.Id = id;
            user.Name = name;
            user.UserName = email;
            user.Email = email;
            user.EmailConfirmed = true;
            user.LockoutEnabled = true;
            user.SecurityStamp = "InitialSecurityStamp";
            user.NormalizedUserName = user.UserName.ToUpper();
            user.NormalizedEmail = user.Email.ToUpper();
            user.PasswordHash = passwordHasher.HashPassword(user, password);
            return user;
        }
    }
}
