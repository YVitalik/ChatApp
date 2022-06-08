using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces
{
    public interface IChatService
    {
        Task<int> DeleteMessage(DeleteMessageDto deleteMessageDto);
        Task CreatePublicRoom(string name, string userId);
        Task<IEnumerable<ReadChatDto>> GetAllPublicChats(string userId);
        Task<IEnumerable<ReadChatDto>> GetUserPublicChats(string userId);
        Task CreatePrivateRoom(string rootId, string targetId);
        Task<IEnumerable<Chat>> GetPrivateChats(string userId);
        Task JoinRoom(int chatId, string userId);
        Task<Message> AddMessage(CreateMessageDto message);
        Task<IEnumerable<Message>> GetChatMessages(int chatId, int amountOfMessagesToTake, DateTime? timeOfSending = null);
        Task<Message> UpdateMessage(UpdateMessageDto updateMessageDto);
        Task<List<User>> GetAllUsers(string currentUserId);
        Task<Message> ReplyMessage(ReplyMessageDto replyMessageDto);
    }
}
