using ChatApp.DAL.Entities;

namespace ChatApp.BLL.DTOs
{
    public class ServerResponseWithMessages : ServerResponse
    {
        public List<Message>? Messages { get; set; }
    }
}
