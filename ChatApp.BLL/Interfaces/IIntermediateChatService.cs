using ChatApp.BLL.DTOs;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces
{
    public interface IIntermediateChatService
    {
        Task<Message?> UpdateMessage(UpdateMessageDto updateMessageDto);
        Task<ServerResponse> CreatePublicChat(string chatName);
        Task<ServerResponseWithChats> GetAllPublicChats();
        Task<ServerResponseWithChats> GetUserPublicChats();
        Task<ServerResponse> JoinRoom(int chatId);
        Task<ServerResponseWithMessages> GetChatMessages(int chatId);
        Task<Message?> CreateMessage(CreateMessageDto messageDto);
        Task<int?> DeleteMessage(int messageId);
        Task<ServerResponseWithUsers> GetApplicationUsers();
        Task<ServerResponse> CreatePrivateChat(string targetId);
        Task<ServerResponseWithChats> GetUserPrivateChats();
    }
}
