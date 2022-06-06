using ChatApp.DAL.Entities;

namespace ChatApp.DAL.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> AddMessage(Message message);
        Task<IEnumerable<Message>> GetChatMessages(int chatId);
        Task<Message> GetMessage(int messageId);
        Message UpdateMessage(Message message);
        Task<int> DeleteMessage(int messageId);
    }
}
