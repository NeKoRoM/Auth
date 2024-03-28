using Microsoft.AspNetCore.Identity;

namespace Auth.Models
{
    public class UserEntity : IdentityUser
    {
        public string? Address { get; set; }
    }
}