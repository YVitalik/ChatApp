using ChatApp.BLL.DTOs.ChatDTOs;

namespace ChatApp.BLL.DTOs
{
    public class ServerResponseWithMessages : ServerResponse
    {
        public List<ReadMessageDto>? Messages { get; set; }
    }
}
