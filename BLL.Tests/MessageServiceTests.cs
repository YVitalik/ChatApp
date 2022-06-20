using AutoFixture;
using AutoMapper;
using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Services;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Interfaces;
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
        public MessageServiceTests()
        {
            sut = new MessageService(moqIUnitOfWork.Object, moqIMapper.Object, moqIUserManagementService.Object);

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddMessage_PassCreateMessageDtoObjectToMethod_ShouldCallAddMessageMethodOfUnitOfWork()
        {
            var newMessageDto = fixture.Create<CreateMessageDto>();

            moqIUnitOfWork.Setup(x => x.Message.AddMessage(It.IsAny<Message>()));

            await sut.AddMessage(newMessageDto);

            moqIUnitOfWork.Verify(x => x.Message.AddMessage(It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task AddMessage_PassCreateMessageDtoObjectToMethod_ShouldCreateAndReturnNewMessage()
        {
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

            var actual = await sut.AddMessage(newMessageDto);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task DeleteMessage_PassDeleteMessageDtoObjectToMethod_ShouldDeleteMessageFromDatabaseAndReturnDeletedMessageId()
        {
            var deleteMessageDto = fixture.Create<DeleteMessageDto>();
            
            moqIUnitOfWork.Setup(x => x.Message.DeleteMessage(It.IsAny<int>()));
            moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(fixture.Build<Message>()
                                                                                                 .With(x => x.SenderId, deleteMessageDto.UserId)
                                                                                                 .Create());

            var actual = await sut.DeleteMessage(deleteMessageDto);

            Assert.Equal(actual, deleteMessageDto.MessageId);
            moqIUnitOfWork.Verify(x => x.Message.DeleteMessage(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_PassDeleteMessageDtoObjectToMethod_ShouldThrowInvalidUserExceptionWhenUserTriesToDeleteNotHisOwnMessage()
        {
            var deleteMessageDto = fixture.Create<DeleteMessageDto>();

            moqIUnitOfWork.Setup(x => x.Message.DeleteMessage(It.IsAny<int>()));
            moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(fixture.Create<Message>());

            await Assert.ThrowsAsync<InvalidUserException>(async () => await sut.DeleteMessage(deleteMessageDto));
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObjectToMethod_ShouldThrowArgumentNullExceptionWhenMessageWithSuchIdDoesntExist()
        {
            var updateMessageDto = fixture.Create<UpdateMessageDto>();

            moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(() => null);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.UpdateMessage(updateMessageDto));
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObjectToMethod_ShouldThrowInvalidUserExceptionWhenMessageFromDbSenderIdIsNotEqualToUpdateMessageDtoSenderId()
        {
            var updateMessageDto = fixture.Create<UpdateMessageDto>();

            moqIUnitOfWork.Setup(x => x.Message.GetMessage(It.IsAny<int>())).ReturnsAsync(fixture.Create<Message>());

            await Assert.ThrowsAsync<InvalidUserException>(async () => await sut.UpdateMessage(updateMessageDto));
        }

        [Fact]
        public async Task UpdateMessage_PassUpdateMessageDtoObjectToMethod_ShouldUpdateAndReturnUpdatedMessage()
        {
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

            var actual = await sut.UpdateMessage(updateMessageDto);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task ReplyMessage_ReplyMessageDtoObjectToMethod_ShouldThrowArgumentNullExceptionWhenChatWithSuchIdDoesntExist()
        {
            var replyMessageDto = fixture.Create<ReplyMessageDto>();

            moqIUnitOfWork.Setup(x => x.Room.GetChatByName(It.IsAny<string>())).ReturnsAsync(() => null);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.ReplyMessage(replyMessageDto));
        }

        [Fact]
        public async Task ReplyMessage_ReplyMessageDtoObjectToMethod_ShouldReplyMessageAndReturnRepliedMessage()
        {
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

            var actual = await sut.ReplyMessage(replyMessageDto);

            Assert.NotNull(actual);

            moqIUnitOfWork.Verify(x => x.Room.GetChatByName(It.IsAny<string>()), Times.Once);
            moqIUnitOfWork.Verify(x => x.Message.GetMessage(replyMessageDto.MessageId), Times.Once);
            moqIUnitOfWork.Verify(x => x.Message.AddMessage(It.IsAny<Message>()), Times.Once);
            moqIUserManagementService.Verify(x => x.GetUserByIdAsync(messageFromDb.SenderId), Times.Once);
        }
    }
}
