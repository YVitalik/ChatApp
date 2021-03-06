using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces
{
    public interface IChatService
    {
        Task CreatePublicRoom(string name, string userId);
        Task<IEnumerable<ReadChatDto>> GetAllPublicChats(string userId);
        Task<IEnumerable<ReadChatDto>> GetUserPublicChats(string userId);
        Task<IEnumerable<ReadChatDto>> GetPrivateChats(string userId);
        Task JoinRoom(int chatId, string userId);
        Task CreatePrivateRoom(string rootId, string targetId);
    }
}
