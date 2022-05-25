using Microsoft.AspNetCore.Identity;

namespace ChatApp.DAL.Entities
{
    public class User : IdentityUser
    {
        public User() : base()
        {
            Chats = new List<ChatUser>();
        }
        public virtual ICollection<ChatUser> Chats { get; set; }
    }
}
