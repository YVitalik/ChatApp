using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.BLL.DTOs.ChatDTOs
{
    public class DeleteMessageDto
    {
        public string UserId { get; set; }
        public int MessageId { get; set; }
    }
}
