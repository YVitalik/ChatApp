using ChatApp.DAL.Entities;

namespace ChatApp.DAL.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessage(int chatId, string messageText, string userId);
        Task<IEnumerable<Message>> GetChatMessages(int chatId); 
    }
}
