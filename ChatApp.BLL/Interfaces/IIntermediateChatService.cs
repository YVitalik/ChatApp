using ChatApp.BLL.DTOs;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces
{
    public interface IIntermediateChatService
    {
        Task<HttpResponseMessage> UpdateMessage(UpdateMessageDto updateMessageDto);
        Task<ServerResponse> CreatePublicChat(string chatName);
        Task<ServerResponseWithChats> GetAllPublicChats();
        Task<ServerResponseWithChats> GetUserPublicChats();
        Task<ServerResponse> JoinRoom(int chatId);
        Task<ServerResponseWithMessages> GetChatMessages(ReadChatMessagesDto readChatMessagesDto);
        Task<Message?> CreateMessage(CreateMessageDto messageDto);
        Task<HttpResponseMessage> ReplyMessage(ReplyMessageDto replyMessageDto);
        Task<ServerResponseWithUsers> GetApplicationUsers();
        Task<ServerResponse> CreatePrivateChat(string targetId);
        Task<ServerResponseWithChats> GetUserPrivateChats();
        Task<HttpResponseMessage> DeleteMessage(int messageId);
    }
}
