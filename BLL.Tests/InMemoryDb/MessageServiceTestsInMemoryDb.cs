using AutoFixture;
using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.DAL.Entities;
using FluentAssertions;
using Moq;

namespace BLL.Tests.InMemoryDb
{
    public class MessageServiceTestsInMemoryDb : BaseTestsClassInMemoryDb
    {
        public MessageServiceTestsInMemoryDb() : base()
        {

        }

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
            actual.Text.Should().Be(createMessageDto.MessageText);
            actual.ChatId.Should().Be(createMessageDto.ChatId);
            actual.SenderId.Should().Be(createMessageDto.SenderId);
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
            var allMessages = _context.Messages;
            var check = allMessages.FirstOrDefault(x => x.Id == deleteMessageDto.MessageId);

            //Assert
            actual.Should().Be(deleteMessageDto.MessageId);
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
            actual.Text.Should().Be(updateMessageDto.Text);
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
    }
}
