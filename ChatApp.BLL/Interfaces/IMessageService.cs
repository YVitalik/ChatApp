using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces
{
    public interface IMessageService
    {
        Task<Message> AddMessage(CreateMessageDto messageDto);
        Task<int> DeleteMessage(DeleteMessageDto deleteMessageDto);
        Task<IEnumerable<Message>> GetChatMessages(int chatId, int amountOfMessagesToTake, DateTime? timeOfSending);
        Task<Message> UpdateMessage(UpdateMessageDto updateMessageDto);
        Task<Message> ReplyMessage(ReplyMessageDto replyMessageDto);

    }
}
