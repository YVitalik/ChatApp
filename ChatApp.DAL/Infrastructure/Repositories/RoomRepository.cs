using ChatApp.DAL.Entities;
using ChatApp.DAL.Interfaces;

namespace ChatApp.DAL.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;
        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task CreatePublicRoom(string name, string userId)
        {
            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Room
            };

            chat.Users.Add(new ChatUser
            {
                UserId = userId
            });

            await _context.Chats.AddAsync(chat);
        }
    }
}
