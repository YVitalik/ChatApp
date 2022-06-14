using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces
{
    public interface IMessageService
    {
        Task<ReadMessageDto> AddMessage(CreateMessageDto messageDto);
        Task<int> DeleteMessage(DeleteMessageDto deleteMessageDto);
        Task<IEnumerable<ReadMessageDto>> GetChatMessages(int chatId, int amountOfMessagesToTake, DateTime? timeOfSending);
        Task<ReadMessageDto> UpdateMessage(UpdateMessageDto updateMessageDto);
        Task<ReadMessageDto> ReplyMessage(ReplyMessageDto replyMessageDto);

    }
}
