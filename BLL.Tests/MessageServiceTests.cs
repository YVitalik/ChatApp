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
        private readonly Fixture fixture = new Fixture();
        private readonly MessageService sut;
        private readonly Mock<IUnitOfWork> moqIUnitOfWork = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> moqIMapper = new Mock<IMapper>();
        private readonly Mock<IUserManagementService> moqIUserManagementService = new Mock<IUserManagementService>();
        private readonly DateTime? dateTime = null;
        public MessageServiceTests()
        {
            sut = new MessageService(moqIUnitOfWork.Object, moqIMapper.Object, moqIUserManagementService.Object);

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddMessage_PassCreateMessageDtoObjectToMethod_ShouldCallAddMessageMethodOfUnitOfWork()
        {
            //Arrange
            var newMessageDto = fixture.Create<CreateMessageDto>();

            moqIUnitOfWork.Setup(x => x.Message.AddMessage(It.IsAny<Message>()));

            //Act
            await sut.AddMessage(newMessageDto);

            //Assert
            moqIUnitOfWork.Verify(x => x.Message.AddMessage(It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task AddMessage_PassCreateMessageDtoObjectToMethod_ShouldCreateAndReturnNewMessage()
        {
            //Arrange
            var newMessage = fixture.Create<Message>();

            var newMessageDto = new CreateMessageDto
            {
                ChatId = newMessage.ChatId,
                MessageText = newMessage.Text,
                Name = newMessage.Name,
                SenderId = newMessage.SenderId
            };

            moqIUnitOfWork.Setup(x => x.Message.AddMessage(It.IsAny<Message>())).ReturnsAsync(newMessage);
            moqIMapper.Setup(x => x.Map<ReadMessageDto>(It.IsAny<Message>())).Returns((Message source) =>
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
            var actual = await sut.AddMessage(newMessageDto);

            //Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteMessage_PassDeleteMessageDtoObjectToMethod_ShouldDeleteMessageFromDatabaseAndReturnDeletedMessageId()
        {
            //Arrange
            var deleteMessageDto = fixture.Create<DeleteMessageDto>();

            moqIUnitOfWork.Setup(x => x.Message.DeleteMessage(It.IsAny<int>()));
            moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(fixture.Build<Message>()
                                                                                                 .With(x => x.SenderId, deleteMessageDto.UserId)
                                                                                                 .Create());
            //Act
            var actual = await sut.DeleteMessage(deleteMessageDto);

            //Assert
            actual.Should().Be(deleteMessageDto.MessageId);
            moqIUnitOfWork.Verify(x => x.Message.DeleteMessage(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_PassDeleteMessageDtoObjectToMethod_ShouldThrowInvalidUserExceptionWhenUserTriesToDeleteNotHisOwnMessage()
        {
            //Arrange
            var deleteMessageDto = fixture.Create<DeleteMessageDto>();

            moqIUnitOfWork.Setup(x => x.Message.DeleteMessage(It.IsAny<int>()));
            moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(fixture.Create<Message>());

            //Assert
            await sut.Invoking(x => x.DeleteMessage(deleteMessageDto)).Should().ThrowAsync<InvalidUserException>();
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObjectToMethod_ShouldThrowArgumentNullExceptionWhenMessageWithSuchIdDoesntExist()
        {
            //Arrange
            var updateMessageDto = fixture.Create<UpdateMessageDto>();

            moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(() => null);

            //Assert
            await sut.Invoking(x => x.UpdateMessage(updateMessageDto)).Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObjectToMethod_ShouldThrowInvalidUserExceptionWhenMessageFromDbSenderIdIsNotEqualToUpdateMessageDtoSenderId()
        {
            //Arrange
            var updateMessageDto = fixture.Create<UpdateMessageDto>();

            moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(fixture.Create<Message>());

            //Assert
            await sut.Invoking(x => x.UpdateMessage(updateMessageDto)).Should().ThrowAsync<InvalidUserException>();
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObjectToMethod_ShouldUpdateAndReturnUpdatedMessage()
        {
            //Arrange
            var updateMessageDto = fixture.Create<UpdateMessageDto>();
            var updatedMessage = fixture.Create<Message>();

            moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(fixture.Build<Message>()
                                                                                                 .With(x => x.SenderId, updateMessageDto.SenderId)
                                                                                                 .Create());
            moqIUnitOfWork.Setup(x => x.Message.UpdateMessage(It.IsAny<Message>())).Returns(updatedMessage);
            moqIMapper.Setup(x => x.Map<ReadMessageDto>(It.IsAny<Message>())).Returns((Message source) =>
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
            var actual = await sut.UpdateMessage(updateMessageDto);

            //Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task ReplyMessage_ReplyMessageDtoObjectToMethod_ShouldThrowArgumentNullExceptionWhenChatWithSuchIdDoesntExist()
        {
            //Arrange
            var replyMessageDto = fixture.Create<ReplyMessageDto>();

            moqIUnitOfWork.Setup(x => x.Room.GetChatByName(It.IsAny<string>())).ReturnsAsync(() => null);

            //Assert
            await sut.Invoking(x => x.ReplyMessage(replyMessageDto)).Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ReplyMessage_ReplyMessageDtoObjectToMethod_ShouldReplyMessageAndReturnRepliedMessage()
        {
            //Arrange
            var replyMessageDto = fixture.Create<ReplyMessageDto>();
            var chatFromDb = fixture.Create<Chat>();
            var messageFromDb = fixture.Create<Message>();
            var userFromDb = fixture.Create<User>();
            var expected = new Message
            {
                ChatId = chatFromDb.Id,
                Text = messageFromDb.Text,
                Name = "Replied from: " + messageFromDb.Name,
                SenderId = messageFromDb.SenderId,
                CreatedAt = DateTime.Now
            };

            moqIUnitOfWork.Setup(x => x.Room.GetChatByName(It.IsAny<string>())).ReturnsAsync(chatFromDb);
            moqIUnitOfWork.Setup(x => x.Message.GetMessage(replyMessageDto.MessageId)).ReturnsAsync(messageFromDb);
            moqIUnitOfWork.Setup(x => x.Message.AddMessage(It.IsAny<Message>())).ReturnsAsync(expected);
            moqIUserManagementService.Setup(x => x.GetUserByIdAsync(messageFromDb.SenderId)).ReturnsAsync(userFromDb);
            moqIMapper.Setup(x => x.Map<ReadMessageDto>(It.IsAny<Message>())).Returns((Message source) =>
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
            var actual = await sut.ReplyMessage(replyMessageDto);

            //Assert
            actual.Should().NotBeNull();

            moqIUnitOfWork.Verify(x => x.Room.GetChatByName(It.IsAny<string>()), Times.Once);
            moqIUnitOfWork.Verify(x => x.Message.GetMessage(replyMessageDto.MessageId), Times.Once);
            moqIUnitOfWork.Verify(x => x.Message.AddMessage(It.IsAny<Message>()), Times.Once);
            moqIUserManagementService.Verify(x => x.GetUserByIdAsync(messageFromDb.SenderId), Times.Once);
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

            moqIUnitOfWork.Setup(x => x.Message.GetChatMessages(1, messagesToTake, timeOfSending))
                .ReturnsAsync(GetTestMessages(messagesToTake, timeOfSending));
            moqIMapper.Setup(x => x.Map<IEnumerable<ReadMessageDto>>(It.IsAny<IEnumerable<Message>>())).Returns((IEnumerable<Message> source) =>
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
            var actual = await sut.GetChatMessages(1, messagesToTake, timeOfSending);

            //Assert
            actual.Count().Should().BeLessThanOrEqualTo(messagesToTake);
            actual.Should().BeOfType<List<ReadMessageDto>>();
            moqIUnitOfWork.Verify(x => x.Message.GetChatMessages(1, messagesToTake, timeOfSending));
        }

        private IEnumerable<Message> GetTestMessages(int messagesToTake, DateTime timeOfSending)
        {
            var messages = new List<Message>();

            for (int i = 0; i < 100; i++)
            {
                var message = new Message()
                {
                    Id = fixture.Create<int>(),
                    Name = fixture.Create<string>(),
                    SenderId = fixture.Create<string>(),
                    Text = fixture.Create<string>(),
                    CreatedAt = DateTime.Now,
                    ChatId = fixture.Create<int>()
                };
                messages.Add(message);
            }

            var filteredMessages = messages.Where(x => x.CreatedAt > timeOfSending).OrderBy(x => x.CreatedAt).ToList();

            var result = filteredMessages.TakeLast(messagesToTake);

            return result;
        }
    }
}
