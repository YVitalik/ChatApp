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

        public async Task AddMessage(Message newMessage)
        {
            await _context.AddAsync(newMessage);
        }

        public async Task<int> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            
            if (message == null)
                throw new ArgumentNullException("Message with such id doesn't exist!");

            _context.Messages.Remove(message);

            return message.Id;
        }

        public async Task<IEnumerable<Message>> GetAllMessages() => await _context.Messages.ToListAsync();

        public async Task<int> UpdateMessage(int id, string text)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
                throw new ArgumentNullException("Message with such id doesn't exist!");

            message.Text = text;

            return message.Id;
        }
    }
}
