using Microsoft.AspNetCore.Identity;

namespace _14_15_AIChat.Entities
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; } = "";
        public List<Conversation> Conversations { get; set; }
    }
}
