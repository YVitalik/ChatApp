using ChatApp.BLL.DTOs.ChatDTOs;

namespace ChatApp.BLL.DTOs
{
    public class ServerResponseWithChats : ServerResponse
    {
        public IEnumerable<ReadChatDto>? Chats { get; set; }
    }
}
