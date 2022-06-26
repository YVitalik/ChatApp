using AutoFixture;
using AutoMapper;
using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Services;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Infrastructure;
using ChatApp.DAL.Infrastructure.Repositories;
using ChatApp.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace BLL.Tests.InMemoryDb
{
    public class BaseTestsClassInMemoryDb
    {
        protected readonly MessageService _sutMessageService;
        protected readonly ChatService _sutChatService;
        protected readonly Fixture _fixture = new Fixture();
        protected readonly AppDbContext _context;
        protected readonly IRoomRepository _roomRepository;
        protected readonly IMessageRepository _messageRepository;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly Mock<IMapper> _mapper = new Mock<IMapper>();
        protected readonly Mock<IUserManagementService> _userManagementService = new Mock<IUserManagementService>();
        protected BaseTestsClassInMemoryDb()
        {
            var _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                                .UseInMemoryDatabase("MessageServiceTest")
                                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                                .Options;

            _context = new AppDbContext(_contextOptions);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var testChats = GetTestChats();
            var testMessages = GetTestMessages();
            var testChatUsers = GetTestChatUsers();

            _context.Messages.AddRangeAsync(testMessages);
            _context.Chats.AddRangeAsync(testChats);
            _context.ChatUsers.AddRangeAsync(testChatUsers);

            _context.SaveChanges();

            _roomRepository = new RoomRepository(_context);
            _messageRepository = new MessageRepository(_context);

            _unitOfWork = new UnitOfWork(_context, _roomRepository, _messageRepository);

            _sutMessageService = new MessageService(_unitOfWork, _mapper.Object, _userManagementService.Object);
            _sutChatService = new ChatService(_unitOfWork, _mapper.Object, _userManagementService.Object);

            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        }

        private List<ChatUser> GetTestChatUsers()
        {
            var result = new List<ChatUser>();
            
            result.Add(new ChatUser
            {
                ChatId = 3,
                UserId = "testUser"
            });

            result.Add(new ChatUser
            {
                ChatId = 25,
                UserId = "testUser"
            });

            return result;
        }

        private List<Chat> GetTestChats()
        {
            var result = new List<Chat>();
            var id = 1;

            for (int i = 0; i < 10; i++)
            {
                var chat = new Chat
                {
                    Id = id,
                    Name = "Chat " + id,
                    Type = ChatType.Room
                };

                result.Add(chat);

                id++;
            }

            result.Add(new Chat
            {
                Id = 25,
                Name = "user1 and user2 private chat",
                Type = ChatType.Private
            });

            return result;
        }

        private List<Message> GetTestMessages()
        {
            var result = new List<Message>();
            var chatId = 1;
            var counterForChatId = 25;
            int id = 1;

            for (int i = 0; i < 100; i++)
            {
                if (result.Count >= counterForChatId)
                {
                    chatId++;
                    counterForChatId += 25;
                }

                var message = new Message
                {
                    Id = id,
                    Name = "Hello",
                    CreatedAt = _fixture.Create<DateTime>(),
                    SenderId = "user1",
                    ChatId = chatId,
                    Text = "Hello everyone this is test message!"
                };

                result.Add(message);

                id++;

            }

            return result;
        }
    }
}
