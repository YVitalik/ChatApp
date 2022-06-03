using ChatApp.DAL.Entities;

namespace ChatApp.BLL.DTOs
{
    public class ServerResponseWithMessages : ServerResponse
    {
        public IEnumerable<Message>? Messages { get; set; }
    }
}
