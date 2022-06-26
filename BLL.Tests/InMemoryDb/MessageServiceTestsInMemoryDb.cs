using AutoFixture;
using AutoMapper;
using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Services;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Infrastructure;
using ChatApp.DAL.Infrastructure.Repositories;
using ChatApp.DAL.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace BLL.Tests.InMemoryDb
{
    public class MessageServiceTestsInMemoryDb : BaseTestsClassInMemoryDb
    {
        public MessageServiceTestsInMemoryDb() : base()
        {
                
        }
        //private readonly MessageService _sut;
        //private readonly Fixture _fixture = new Fixture();
        //private readonly AppDbContext _context;
        //private readonly IRoomRepository _roomRepository;
        //private readonly IMessageRepository _messageRepository;
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly Mock<IMapper> _mapper = new Mock<IMapper>();
        //private readonly Mock<IUserManagementService> _userManagementService = new Mock<IUserManagementService>();
        //public MessageServiceTestsInMemoryDb()
        //{
        //    var _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
        //                        .UseInMemoryDatabase("MessageServiceTest")
        //                        .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        //                        .Options;

        //    _context = new AppDbContext(_contextOptions);

        //    _context.Database.EnsureDeleted();
        //    _context.Database.EnsureCreated();

        //    var testChats = GetTestChats();
        //    var testMessages = GetTestMessages();

        //    _context.Messages.AddRangeAsync(testMessages);
        //    _context.Chats.AddRangeAsync(testChats);

        //    _context.SaveChanges();

        //    _roomRepository = new RoomRepository(_context);
        //    _messageRepository = new MessageRepository(_context);

        //    _unitOfWork = new UnitOfWork(_context, _roomRepository, _messageRepository);

        //    _sut = new MessageService(_unitOfWork, _mapper.Object, _userManagementService.Object);

        //    _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        //    _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        //}

        [Fact]
        public async Task AddMessage_PassCreateMessageDtoObjectToMethod_ShouldAddAndReturnNewMessage()
        {
            //Arrange
            _mapper.Setup(x => x.Map<ReadMessageDto>(It.IsAny<Message>())).Returns((Message source) =>
            {
                return new ReadMessageDto
                {
                    SenderId = source.SenderId,
                    CreatedAt = source.CreatedAt,
                    Id = source.Id,
                    Name = source.Name,
                    ChatId = source.ChatId,
                    Text = source.Text
                };
            });

            var createMessageDto = new CreateMessageDto
            {
                ChatId = 4,
                MessageText = "I am new message!",
                Name = "Hello",
                SenderId = "user2"
            };

            //Act
            var actual = await _sutMessageService.AddMessage(createMessageDto);

            //Assert
            actual.Should().NotBeNull();
            Assert.Equal(createMessageDto.MessageText, actual.Text);
            Assert.Equal(createMessageDto.ChatId, actual.ChatId);
            Assert.Equal(createMessageDto.SenderId, actual.SenderId);
        }

        [Fact]
        public async Task DeleteMessage_PassDeleteMessageDtoObject_ShouldDeleteMessageFromDbAndReturnDeletedMessageId()
        {
            //Arrange
            var deleteMessageDto = new DeleteMessageDto
            {
                MessageId = 1,
                UserId = "user1"
            };

            //Act
            var actual = await _sutMessageService.DeleteMessage(deleteMessageDto);
            var allMessages = await _sutMessageService.GetChatMessages(1, 25, null);
            var check = allMessages.FirstOrDefault(x => x.Id == deleteMessageDto.MessageId);

            //Assert
            Assert.Equal(deleteMessageDto.MessageId, actual);
            check.Should().BeNull();
        }

        [Fact]
        public async Task DeleteMessage_PassDeleteMessageDtoObject_ShouldThrownExceptionWhenUserTriesToDeleteNotHisMessage()
        {
            //Arrange
            var deleteMessageDto = new DeleteMessageDto
            {
                MessageId = 1,
                UserId = "user"
            };

            //Assert
            await _sutMessageService.Invoking(x => x.DeleteMessage(deleteMessageDto)).Should().ThrowAsync<InvalidUserException>();
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObject_ShouldThrownExceptionWhenUserTriesToUpdateMessageWhichDoesntExist()
        {
            //Arrange
            var updateMessageDto = new UpdateMessageDto
            {
                Id = 96274,
                SenderId = "user1",
                Text = "Should not be updated"
            };

            //Assert
            await _sutMessageService.Invoking(x => x.UpdateMessage(updateMessageDto)).Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObject_ShouldThrownExceptionWhenUserTriesToUpdateNotHisMessage()
        {
            //Arrange
            var updateMessageDto = new UpdateMessageDto
            {
                Id = 1,
                SenderId = "user",
                Text = "Should not be updated"
            };

            //Assert
            await _sutMessageService.Invoking(x => x.UpdateMessage(updateMessageDto)).Should().ThrowAsync<InvalidUserException>();
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObject_ShouldSuccesfullyUpdateMessageAndReturnUpdatedMessage()
        {
            //Arrange
            var updateMessageDto = new UpdateMessageDto
            {
                Id = 2,
                SenderId = "user1",
                Text = "Should be updated"
            };

            _mapper.Setup(x => x.Map<ReadMessageDto>(It.IsAny<Message>())).Returns((Message source) =>
            {
                return new ReadMessageDto
                {
                    SenderId = source.SenderId,
                    CreatedAt = source.CreatedAt,
                    Id = source.Id,
                    Name = source.Name,
                    ChatId = source.ChatId,
                    Text = source.Text
                };
            });

            //Act
            var actual = await _sutMessageService.UpdateMessage(updateMessageDto);

            //Assert
            actual.Should().NotBeNull();
            Assert.Equal(updateMessageDto.Text, actual.Text);
        }

        [Fact]
        public async Task ReplyMessage_PassReplyMessageDtoObject_ShouldThrowExceptionWhenChatForReplyingDoesntExist()
        {
            //Arrange
            var replyMessageDto = new ReplyMessageDto
            {
                ChatNameToReply = "chat",
                MessageId = 5
            };

            //Assert
            await _sutMessageService.Invoking(x => x.ReplyMessage(replyMessageDto)).Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ReplyMessage_PassReplyMessageDtoObject_ShouldSuccesfullyReplyMessageAndReturnRepliedMessage()
        {
            //Arrange
            var replyMessageDto = new ReplyMessageDto
            {
                MessageId = 5,
                ChatNameToReply = "Chat 1"
            };
            _userManagementService.Setup(x => x.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(_fixture.Create<User>());
            _mapper.Setup(x => x.Map<ReadMessageDto>(It.IsAny<Message>())).Returns((Message source) =>
            {
                return new ReadMessageDto
                {
                    SenderId = source.SenderId,
                    CreatedAt = source.CreatedAt,
                    Id = source.Id,
                    Name = source.Name,
                    ChatId = source.ChatId,
                    Text = source.Text
                };
            });

            //Act
            var actual = await _sutMessageService.ReplyMessage(replyMessageDto);

            //Actual
            actual.Should().NotBeNull();
        }

        [Theory]
        [InlineData(2, 20)]
        [InlineData(1, 10)]
        [InlineData(3, 12)]
        public async Task GetChatMessages_MethodReceivesChatIdAndAmountOfMessagesToTake_ShouldReturnMessagesWithSameChatId(int chatId, int amountOfMessagesToTake)
        {
            //Arrange
            _mapper.Setup(x => x.Map<IEnumerable<ReadMessageDto>>(It.IsAny<IEnumerable<Message>>())).Returns((IEnumerable<Message> source) =>
            {
                var toReturn = new List<ReadMessageDto>();

                foreach (var message in source)
                {
                    var messageDto = new ReadMessageDto
                    {
                        SenderId = message.SenderId,
                        CreatedAt = message.CreatedAt,
                        Id = message.Id,
                        Name = message.Name,
                        ChatId = message.ChatId,
                        Text = message.Text
                    };

                    toReturn.Add(messageDto);
                }

                return toReturn;
            });

            //Act
            var actual = await _sutMessageService.GetChatMessages(chatId, amountOfMessagesToTake, null);

            //Assert
            actual.Should().BeOfType<List<ReadMessageDto>>();
            actual.Count().Should().BeLessThanOrEqualTo(amountOfMessagesToTake);
        }

        //private List<Chat> GetTestChats()
        //{
        //    var result = new List<Chat>();
        //    var id = 1;

        //    for (int i = 0; i < 10; i++)
        //    {
        //        var chat = new Chat
        //        {
        //            Id = id,
        //            Name = "Chat " + id,
        //            Type = ChatType.Room
        //        };

        //        result.Add(chat);

        //        id++;
        //    }

        //    return result;
        //}

        //private List<Message> GetTestMessages()
        //{
        //    var result = new List<Message>();
        //    var chatId = 1;
        //    var counterForChatId = 25;
        //    int id = 1;

        //    for (int i = 0; i < 100; i++)
        //    {
        //        if (result.Count >= counterForChatId)
        //        {
        //            chatId++;
        //            counterForChatId += 25;
        //        }

        //        var message = new Message
        //        {
        //            Id = id,
        //            Name = "Hello",
        //            CreatedAt = _fixture.Create<DateTime>(),
        //            SenderId = "user1",
        //            ChatId = chatId,
        //            Text = "Hello everyone this is test message!"
        //        };

        //        result.Add(message);

        //        id++;

        //    }

        //    return result;
        //}
    }
}
