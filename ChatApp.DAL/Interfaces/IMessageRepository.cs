using ChatApp.DAL.Entities;

namespace ChatApp.DAL.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessage(Message newMessage);
        Task<int> DeleteMessage(int id);
        Task<int> UpdateMessage(int id, string text);
        Task<IEnumerable<Message>> GetAllMessages();
    }
}
