using ChatApp.DAL.Entities;

namespace ChatApp.DAL.Interfaces
{
    public interface IRoomRepository
    {
        Task CreatePublicRoom(string name, string userId);
        Task<IEnumerable<Chat>> GetAllPublicChats(string userId);
        Task<IEnumerable<Chat>> GetUserPublicChats(string userId);
        Task CreatePrivateRoom(string rootId, string targetId);
        Task<IEnumerable<Chat>> GetPrivateChats(string userId);
        Task JoinRoom(int chatId, string userId);
    }
}
