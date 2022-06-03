using ChatApp.DAL.Entities;

namespace ChatApp.DAL.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessage(Message message);
        Task<IEnumerable<Message>> GetChatMessages(int chatId); 
    }
}
