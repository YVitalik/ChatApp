using Microsoft.AspNetCore.Identity;

namespace ChatApp.DAL.Entities
{
    public class User : IdentityUser
    {
        public virtual ICollection<ChatUser>? Chats { get; set; }
    }
}
