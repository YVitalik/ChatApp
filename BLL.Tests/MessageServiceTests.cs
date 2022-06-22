using AutoFixture;
using AutoMapper;
using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Services;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Interfaces;
using FluentAssertions;
using Moq;

namespace BLL.Tests
{
    public class MessageServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly MessageService _sut;
        private readonly Mock<IUnitOfWork> _moqIUnitOfWork = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _moqIMapper = new Mock<IMapper>();
        private readonly Mock<IUserManagementService> _moqIUserManagementService = new Mock<IUserManagementService>();

        public MessageServiceTests()
        {
            _sut = new MessageService(_moqIUnitOfWork.Object, _moqIMapper.Object, _moqIUserManagementService.Object);

            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddMessage_PassCreateMessageDtoObjectToMethod_ShouldCallAddMessageMethodOfUnitOfWork()
        {
            //Arrange
            var newMessageDto = _fixture.Create<CreateMessageDto>();

            _moqIUnitOfWork.Setup(x => x.Message.AddMessage(It.IsAny<Message>()));

            //Act
            await _sut.AddMessage(newMessageDto);

            //Assert
            _moqIUnitOfWork.Verify(x => x.Message.AddMessage(It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task AddMessage_PassCreateMessageDtoObjectToMethod_ShouldCreateAndReturnNewMessage()
        {
            //Arrange
            var newMessage = _fixture.Create<Message>();

            var newMessageDto = new CreateMessageDto
            {
                ChatId = newMessage.ChatId,
                MessageText = newMessage.Text,
                Name = newMessage.Name,
                SenderId = newMessage.SenderId
            };

            _moqIUnitOfWork.Setup(x => x.Message.AddMessage(It.IsAny<Message>())).ReturnsAsync(newMessage);
            _moqIMapper.Setup(x => x.Map<ReadMessageDto>(It.IsAny<Message>())).Returns((Message source) =>
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
            var actual = await _sut.AddMessage(newMessageDto);

            //Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteMessage_PassDeleteMessageDtoObjectToMethod_ShouldDeleteMessageFromDatabaseAndReturnDeletedMessageId()
        {
            //Arrange
            var deleteMessageDto = _fixture.Create<DeleteMessageDto>();

            _moqIUnitOfWork.Setup(x => x.Message.DeleteMessage(It.IsAny<int>()));
            _moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(_fixture.Build<Message>()
                                                                                                 .With(x => x.SenderId, deleteMessageDto.UserId)
                                                                                                 .Create());
            //Act
            var actual = await _sut.DeleteMessage(deleteMessageDto);

            //Assert
            actual.Should().Be(deleteMessageDto.MessageId);
            _moqIUnitOfWork.Verify(x => x.Message.DeleteMessage(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_PassDeleteMessageDtoObjectToMethod_ShouldThrowInvalidUserExceptionWhenUserTriesToDeleteNotHisOwnMessage()
        {
            //Arrange
            var deleteMessageDto = _fixture.Create<DeleteMessageDto>();

            _moqIUnitOfWork.Setup(x => x.Message.DeleteMessage(It.IsAny<int>()));
            _moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(_fixture.Create<Message>());

            //Assert
            await _sut.Invoking(x => x.DeleteMessage(deleteMessageDto)).Should().ThrowAsync<InvalidUserException>();
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObjectToMethod_ShouldThrowArgumentNullExceptionWhenMessageWithSuchIdDoesntExist()
        {
            //Arrange
            var updateMessageDto = _fixture.Create<UpdateMessageDto>();

            _moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(() => null);

            //Assert
            await _sut.Invoking(x => x.UpdateMessage(updateMessageDto)).Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObjectToMethod_ShouldThrowInvalidUserExceptionWhenMessageFromDbSenderIdIsNotEqualToUpdateMessageDtoSenderId()
        {
            //Arrange
            var updateMessageDto = _fixture.Create<UpdateMessageDto>();

            _moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(_fixture.Create<Message>());

            //Assert
            await _sut.Invoking(x => x.UpdateMessage(updateMessageDto)).Should().ThrowAsync<InvalidUserException>();
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObjectToMethod_ShouldUpdateAndReturnUpdatedMessage()
        {
            //Arrange
            var updateMessageDto = _fixture.Create<UpdateMessageDto>();
            var updatedMessage = _fixture.Create<Message>();

            _moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(_fixture.Build<Message>()
                                                                                                 .With(x => x.SenderId, updateMessageDto.SenderId)
                                                                                                 .Create());
            _moqIUnitOfWork.Setup(x => x.Message.UpdateMessage(It.IsAny<Message>())).Returns(updatedMessage);
            _moqIMapper.Setup(x => x.Map<ReadMessageDto>(It.IsAny<Message>())).Returns((Message source) =>
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
            var actual = await _sut.UpdateMessage(updateMessageDto);

            //Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task ReplyMessage_ReplyMessageDtoObjectToMethod_ShouldThrowArgumentNullExceptionWhenChatWithSuchIdDoesntExist()
        {
            //Arrange
            var replyMessageDto = _fixture.Create<ReplyMessageDto>();

            _moqIUnitOfWork.Setup(x => x.Room.GetChatByName(It.IsAny<string>())).ReturnsAsync(() => null);

            //Assert
            await _sut.Invoking(x => x.ReplyMessage(replyMessageDto)).Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ReplyMessage_ReplyMessageDtoObjectToMethod_ShouldReplyMessageAndReturnRepliedMessage()
        {
            //Arrange
            var replyMessageDto = _fixture.Create<ReplyMessageDto>();
            var chatFromDb = _fixture.Create<Chat>();
            var messageFromDb = _fixture.Create<Message>();
            var userFromDb = _fixture.Create<User>();
            var expected = new Message
            {
                ChatId = chatFromDb.Id,
                Text = messageFromDb.Text,
                Name = "Replied from: " + messageFromDb.Name,
                SenderId = messageFromDb.SenderId,
                CreatedAt = DateTime.Now
            };

            _moqIUnitOfWork.Setup(x => x.Room.GetChatByName(It.IsAny<string>())).ReturnsAsync(chatFromDb);
            _moqIUnitOfWork.Setup(x => x.Message.GetMessage(replyMessageDto.MessageId)).ReturnsAsync(messageFromDb);
            _moqIUnitOfWork.Setup(x => x.Message.AddMessage(It.IsAny<Message>())).ReturnsAsync(expected);
            _moqIUserManagementService.Setup(x => x.GetUserByIdAsync(messageFromDb.SenderId)).ReturnsAsync(userFromDb);
            _moqIMapper.Setup(x => x.Map<ReadMessageDto>(It.IsAny<Message>())).Returns((Message source) =>
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
            var actual = await _sut.ReplyMessage(replyMessageDto);

            //Assert
            actual.Should().NotBeNull();

            _moqIUnitOfWork.Verify(x => x.Room.GetChatByName(It.IsAny<string>()), Times.Once);
            _moqIUnitOfWork.Verify(x => x.Message.GetMessage(replyMessageDto.MessageId), Times.Once);
            _moqIUnitOfWork.Verify(x => x.Message.AddMessage(It.IsAny<Message>()), Times.Once);
            _moqIUserManagementService.Verify(x => x.GetUserByIdAsync(messageFromDb.SenderId), Times.Once);
        }

        [Theory]
        [InlineData(10, "2021-06-21 21:47:13")]
        [InlineData(5, "2023-06-21 21:47:13")]
        [InlineData(5, "2022-06-21 21:47:13")]
        private async Task GetChatMessages_ShouldReturnMessagesWichSatisfyGivenParameters(int messagesToTake, string timeOfSendingInString)
        {
            //Arrange
            var timeOfSending = DateTime.ParseExact(timeOfSendingInString, "yyyy-MM-dd HH:mm:ss",
                                                    System.Globalization.CultureInfo.InvariantCulture);

            _moqIUnitOfWork.Setup(x => x.Message.GetChatMessages(1, messagesToTake, timeOfSending))
                .ReturnsAsync(GetTestMessages(messagesToTake, timeOfSending));
            _moqIMapper.Setup(x => x.Map<IEnumerable<ReadMessageDto>>(It.IsAny<IEnumerable<Message>>())).Returns((IEnumerable<Message> source) =>
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
            var actual = await _sut.GetChatMessages(1, messagesToTake, timeOfSending);

            //Assert
            actual.Count().Should().BeLessThanOrEqualTo(messagesToTake);
            actual.Should().BeOfType<List<ReadMessageDto>>();
            _moqIUnitOfWork.Verify(x => x.Message.GetChatMessages(1, messagesToTake, timeOfSending));
        }

        private IEnumerable<Message> GetTestMessages(int messagesToTake, DateTime timeOfSending)
        {
            var messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                var message = new Message()
                {
                    Id = _fixture.Create<int>(),
                    Name = _fixture.Create<string>(),
                    SenderId = _fixture.Create<string>(),
                    Text = _fixture.Create<string>(),
                    CreatedAt = DateTime.Now,
                    ChatId = _fixture.Create<int>()
                };
                messages.Add(message);
            }

            var filteredMessages = messages.Where(x => x.CreatedAt > timeOfSending).OrderBy(x => x.CreatedAt).ToList();

            var result = filteredMessages.TakeLast(messagesToTake);

            return result;
        }
    }
}
