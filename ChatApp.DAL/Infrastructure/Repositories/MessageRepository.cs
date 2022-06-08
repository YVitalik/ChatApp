using ChatApp.DAL.Entities;
using ChatApp.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DAL.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;
        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Message> AddMessage(Message message)
        {
            await _context.Messages.AddAsync(message);
            return message;
        }

        public async Task<int> DeleteMessage(int messageId)
        {
            var messageToDelete = await _context.Messages.FindAsync(messageId);
            _context.Remove(messageToDelete);

            return messageToDelete.Id;
        }

        public async Task<IEnumerable<Message>> GetChatMessages(int chatId, int amountOfMessagesToTake, DateTime? timeOfSending = null)
        {
            if (timeOfSending == null)
            {
                var messages = await _context.Messages.Where(x => x.ChatId == chatId).ToListAsync();
                return messages.TakeLast(amountOfMessagesToTake);
            }
            else 
            {
                var messages = await _context.Messages.Where(x => (x.ChatId == chatId && x.CreatedAt < timeOfSending)).ToListAsync();
                return messages.TakeLast(amountOfMessagesToTake);
            }
        }

        public async Task<Message> GetMessage(int messageId)
        {
            return await _context.Messages.FindAsync(messageId);
        }

        public Message UpdateMessage(Message message)
        {
            _context.Update(message);
            return message;
        }
    }
}
