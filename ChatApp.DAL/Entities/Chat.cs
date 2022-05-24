using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.DAL.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ChatType Type { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<ChatUser>? Users { get; set; }
    }
}
