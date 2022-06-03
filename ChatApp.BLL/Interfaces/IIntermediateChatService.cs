using ChatApp.BLL.DTOs;

namespace ChatApp.BLL.Interfaces
{
    public interface IIntermediateChatService
    {
        Task<ServerResponse> CreatePublicChat(string chatName);
        Task<ServerResponseWithChats> GetAllPublicChats();
        Task<ServerResponseWithChats> GetUserPublicChats();
        Task<ServerResponse> JoinRoom(int chatId);
        Task<ServerResponseWithMessages> GetChatMessages(int chatId);
    }
}
