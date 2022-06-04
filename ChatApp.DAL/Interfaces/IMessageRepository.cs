using ChatApp.DAL.Entities;

namespace ChatApp.DAL.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> AddMessage(Message message);
        Task<IEnumerable<Message>> GetChatMessages(int chatId); 
    }
}
