using ChatApp.BLL.DTOs.ChatDTOs;

namespace ChatApp.BLL.Interfaces
{
    public interface IIntermediateChatService
    {
        Task<bool> CreatePublicChat(string chatName);
        Task<IEnumerable<ReadChatDto>> GetAllPublicChats();
        Task<IEnumerable<ReadChatDto>> GetUserPublicChats();
        Task<bool> JoinRoom(int chatId);
    }
}
