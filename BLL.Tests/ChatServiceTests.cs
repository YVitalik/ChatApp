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
    public class ChatServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly ChatService _sut;
        private readonly Mock<IUnitOfWork> _moqIUnitOfWork = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _moqIMapper = new Mock<IMapper>();
        private readonly Mock<IUserManagementService> _moqIUserManagementService = new Mock<IUserManagementService>();

        public ChatServiceTests()
        {
            _sut = new ChatService(_moqIUnitOfWork.Object, _moqIMapper.Object, _moqIUserManagementService.Object);

            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task CreatePrivateRoom_PassTargetIdAndRootId_ShouldThrowExceptionWhenPrivateChatWithUserAlreadyExist()
        {
            //Arrange
            var chatToReturn = _fixture.Create<Chat>();
            var chatUsers = new List<User>();
            chatUsers.Add(_fixture.Create<User>());
            chatUsers.Add(_fixture.Create<User>());

            _moqIUserManagementService.Setup(x => x.GetUsersForPrivateChats(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(chatUsers);
            _moqIUnitOfWork.Setup(x => x.Room.GetChatByName(It.IsAny<string>())).ReturnsAsync(chatToReturn);

            //Assert
            await _sut.Invoking(x => x.CreatePrivateRoom(_fixture.Create<string>(), _fixture.Create<string>())).Should().ThrowAsync<ItemWithSuchNameAlreadyExists>();
        }

        [Fact]
        public async Task CreatePrivateRoom_PassTargetIdAndRootId_ShouldSuccesfullyCreatePrivateRoom()
        {
            //Arrange
            var chatToReturn = _fixture.Create<Chat>();
            var chatUsers = new List<User>();
            chatUsers.Add(_fixture.Create<User>());
            chatUsers.Add(_fixture.Create<User>());

            _moqIUserManagementService.Setup(x => x.GetUsersForPrivateChats(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(chatUsers);
            _moqIUnitOfWork.Setup(x => x.Room.GetChatByName(It.IsAny<string>())).ReturnsAsync(() => null);

            //Act
            await _sut.CreatePrivateRoom(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            _moqIUnitOfWork.Verify(x => x.Room.CreatePrivateRoom(It.IsAny<string>(), It.IsAny<string>(), chatUsers), Times.Once);
            _moqIUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreatePublicRoom_PassRoomNameAndUserId_ShouldThrowExceptionWhenRoomWithSuchNameAlreadyExists()
        {
            //Arrange
            _moqIUnitOfWork.Setup(x => x.Room.GetChatByName(It.IsAny<string>())).ReturnsAsync(_fixture.Create<Chat>());

            //Assert
            await _sut.Invoking(x => x.CreatePublicRoom(_fixture.Create<string>(), _fixture.Create<string>())).Should().ThrowAsync<ItemWithSuchNameAlreadyExists>();
        }

        [Fact]
        public async Task CreatePublicRoom_PassRoomNameAndUserId_ShouldSuccesfullyCreatePublicChat()
        {
            //Arrange
            _moqIUnitOfWork.Setup(x => x.Room.GetChatByName(It.IsAny<string>())).ReturnsAsync(() => null);

            //Act
            await _sut.CreatePublicRoom(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            _moqIUnitOfWork.Verify(x => x.Room.CreatePublicRoom(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _moqIUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllPublicChats_ShouldReturnAllPublicChats()
        {
            //Arrange
            _moqIUnitOfWork.Setup(x => x.Room.GetAllPublicChats(It.IsAny<string>())).ReturnsAsync(GetTestChats(ChatType.Room));
            _moqIMapper.Setup(x => x.Map<IEnumerable<ReadChatDto>>(It.IsAny<IEnumerable<Chat>>())).Returns((IEnumerable<Chat> source) =>
            {
                var toReturn = new List<ReadChatDto>();

                foreach (var chat in source)
                {
                    var chatDto = new ReadChatDto
                    {
                        Id = chat.Id,
                        Name = chat.Name
                    };

                    toReturn.Add(chatDto);
                }

                return toReturn;
            });

            //Act
            var actual = await _sut.GetAllPublicChats(_fixture.Create<string>());

            //Assert
            actual.Should().BeOfType<List<ReadChatDto>>();
            _moqIUnitOfWork.Verify(x => x.Room.GetAllPublicChats(It.IsAny<string>()));
        }

        [Fact]
        public async Task GetPrivateChats_ShouldReturnUserPrivateChats()
        {
            //Arrange
            _moqIUnitOfWork.Setup(x => x.Room.GetAllPublicChats(It.IsAny<string>())).ReturnsAsync(GetTestChats(ChatType.Private));
            _moqIMapper.Setup(x => x.Map<IEnumerable<ReadChatDto>>(It.IsAny<IEnumerable<Chat>>())).Returns((IEnumerable<Chat> source) =>
            {
                var toReturn = new List<ReadChatDto>();

                foreach (var chat in source)
                {
                    var chatDto = new ReadChatDto
                    {
                        Id = chat.Id,
                        Name = chat.Name
                    };

                    toReturn.Add(chatDto);
                }

                return toReturn;
            });

            //Act
            var actual = await _sut.GetAllPublicChats(_fixture.Create<string>());

            //Assert
            actual.Should().BeOfType<List<ReadChatDto>>();
            _moqIUnitOfWork.Verify(x => x.Room.GetAllPublicChats(It.IsAny<string>()));
        }

        [Fact]
        public async Task GetUserPublicChats_ShouldReturnUserPublicChats()
        {
            //Arrange
            _moqIUnitOfWork.Setup(x => x.Room.GetAllPublicChats(It.IsAny<string>())).ReturnsAsync(GetTestChats(ChatType.Room));
            _moqIMapper.Setup(x => x.Map<IEnumerable<ReadChatDto>>(It.IsAny<IEnumerable<Chat>>())).Returns((IEnumerable<Chat> source) =>
            {
                var toReturn = new List<ReadChatDto>();

                foreach (var chat in source)
                {
                    var chatDto = new ReadChatDto
                    {
                        Id = chat.Id,
                        Name = chat.Name
                    };

                    toReturn.Add(chatDto);
                }

                return toReturn;
            });

            //Act
            var actual = await _sut.GetAllPublicChats(_fixture.Create<string>());

            //Assert
            actual.Should().BeOfType<List<ReadChatDto>>();
            _moqIUnitOfWork.Verify(x => x.Room.GetAllPublicChats(It.IsAny<string>()));
        }

        [Fact]
        public async Task JoinRoom_PassChatIdAndUserIdToMethod_ShouldAddUserToPublicChat()
        {
            //Arrange
            _moqIUnitOfWork.Setup(x => x.Room.JoinRoom(It.IsAny<int>(), It.IsAny<string>()));

            //Act
            await _sut.JoinRoom(It.IsAny<int>(), It.IsAny<string>());

            //Assert
            _moqIUnitOfWork.Verify(x => x.Room.JoinRoom(It.IsAny<int>(), It.IsAny<string>()));
            _moqIUnitOfWork.Verify(x => x.SaveChangesAsync());
        }

        private IEnumerable<Chat> GetTestChats(ChatType type)
        {
            var chatsForTest = new List<Chat>();

            for (int i = 0; i < 100; i++)
            {
                var chat = new Chat()
                {
                    Id = _fixture.Create<int>(),
                    Name = _fixture.Create<string>(),
                    Type = _fixture.Create<ChatType>()
                };
                chatsForTest.Add(chat);
            }

            var result = chatsForTest.Where(x => x.Type == type);

            return result;
        }
    }
}
