using ChatApp.DAL.Entities;
using ChatApp.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Chat>> GetUserPublicChats(string userId)
        {
            return await _context.Chats
                    .Where(x => x.Type == ChatType.Room &&
                        x.Users
                        .Any(y => y.UserId == userId))
                    .ToListAsync();
        }

        public async Task<IEnumerable<Chat>> GetAllPublicChats(string userId)
        {
            return await _context.Chats
                    .Where(x => x.Type == ChatType.Room && 
                        !x.Users
                        .Any(y => y.UserId == userId))
                    .ToListAsync();
        }

        public async Task CreatePrivateRoom(string rootId, string targetId, List<User> privateChatUsers)
        {
            var chat = new Chat
            {
                Name = privateChatUsers[0] + " and " + privateChatUsers[1] + " private chat",
                Type = ChatType.Private
            };

            chat.Users.Add(new ChatUser
            {
                UserId = targetId
            });

            chat.Users.Add(new ChatUser
            {
                UserId = rootId
            });

            await _context.Chats.AddAsync(chat);
        }

        public async Task<IEnumerable<Chat>> GetPrivateChats(string userId)
        {
            return await _context.Chats
                  .Where(x => x.Type == ChatType.Private
                      && x.Users
                          .Any(y => y.UserId == userId))
                  .ToListAsync();
        }

        public async Task JoinRoom(int chatId, string userId)
        {
            var chatUser = new ChatUser
            {
                ChatId = chatId,
                UserId = userId
            };

            await _context.ChatUsers.AddAsync(chatUser);
        }
    }
}
