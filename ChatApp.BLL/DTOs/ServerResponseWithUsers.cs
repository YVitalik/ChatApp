using ChatApp.DAL.Entities;

namespace ChatApp.BLL.DTOs
{
    public class ServerResponseWithUsers : ServerResponse
    {
        public List<User>? Users { get; set; }
    }
}
