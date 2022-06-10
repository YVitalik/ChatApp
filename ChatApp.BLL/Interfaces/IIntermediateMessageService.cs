using ChatApp.BLL.DTOs;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces
{
    public interface IIntermediateMessageService
    {
        Task<HttpResponseMessage> UpdateMessage(UpdateMessageDto updateMessageDto);
        Task<ServerResponseWithMessages> GetChatMessages(ReadChatMessagesDto readChatMessagesDto);
        Task<Message?> CreateMessage(CreateMessageDto messageDto);
        Task<HttpResponseMessage> ReplyMessage(ReplyMessageDto replyMessageDto);
        Task<HttpResponseMessage> DeleteMessage(int messageId);
    }
}
