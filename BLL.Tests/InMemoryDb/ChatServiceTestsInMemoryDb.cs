using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.DAL.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BLL.Tests.InMemoryDb
{
    public class ChatServiceTestsInMemoryDb : BaseTestsClassInMemoryDb
    {
        public ChatServiceTestsInMemoryDb() : base()
        {

        }

        [Fact]
        public async Task CreatePrivateRoom_PassRootIdAndTargetIdToMethod_ShouldThrowExceptionIfChatWithUserAlreadyExists()
        {
            //Arrange
            var usersForPrivateChat = new List<User>();
            usersForPrivateChat.Add(new User
            {
                Id = "afalfhaslf",
                UserName = "user1"
            });
            usersForPrivateChat.Add(new User
            {
                Id = "qoifncoaho",
                UserName = "user2"
            });
            _userManagementService.Setup(x => x.GetUsersForPrivateChats(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(usersForPrivateChat);

            //Assert
            await _sutChatService.Invoking(x => x.CreatePrivateRoom("afalfhaslf", "qoifncoaho")).Should().ThrowAsync<ItemWithSuchNameAlreadyExists>();
        }

        [Fact]
        public async Task CreatePrivateRoom_PassRootIdAndTargetIdToMethod_ShouldSuccesfullyAddNewPrivateChat()
        {
            //Arrange
            var usersForPrivateChat = new List<User>();
            usersForPrivateChat.Add(new User
            {
                Id = "afalurjtlh",
                UserName = "user3"
            });
            usersForPrivateChat.Add(new User
            {
                Id = "qoifnpiitm",
                UserName = "user4"
            });
            _userManagementService.Setup(x => x.GetUsersForPrivateChats("afalurjtlh", "qoifnpiitm")).ReturnsAsync(usersForPrivateChat);

            //Act
            await _sutChatService.CreatePrivateRoom("afalurjtlh", "qoifnpiitm");

            //Assert
            var checkIfPrivateChatIsAdded = _context.Chats.FirstOrDefault(x => x.Name == "user3 and user4 private chat");
            checkIfPrivateChatIsAdded.Should().NotBeNull();
        }

        [Fact]
        public async Task CreatePublicChat_PassUserIdAndChatNameToMethod_ShouldThrowExceptionIfChatWithSuchNameAlreadyExists()
        {
            //Arrange
            var chatName = "Chat 1";
            var userId = "asfkljasfsjf";

            //Assert
            await _sutChatService.Invoking(x => x.CreatePublicRoom(chatName, userId)).Should().ThrowAsync<ItemWithSuchNameAlreadyExists>();
        }

        [Fact]
        public async Task CreatePublicChat_PassUserIdAndChatNameToMethod_ShouldSuccesfullyAddNewPublicChat()
        {
            //Arrange
            var chatName = "Hello I am New chat";
            var userId = "yurbglaiufh";

            //Act
            await _sutChatService.CreatePublicRoom(chatName, userId);

            //Assert
            var checkIfPublicChatIsAdded = _context.Chats.FirstOrDefault(x => x.Name == chatName);
            checkIfPublicChatIsAdded.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllPublicChats_PassUserIdToMethod_ShouldReturnPublicChatsWhichUserHasNotJoined()
        {
            //Arrange
            _mapper.Setup(x => x.Map<IEnumerable<ReadChatDto>>(It.IsAny<IEnumerable<Chat>>())).Returns((IEnumerable<Chat> source) =>
            {
                var toReturn = new List<ReadChatDto>();

                foreach (var chat in source)
                {
                    var chatDto = new ReadChatDto
                    {
                        Id = chat.Id,
                        Name = chat.Name,
                    };

                    toReturn.Add(chatDto);
                }

                return toReturn;
            });

            //Act
            var expected = await _context.Chats
                    .Where(x => x.Type == ChatType.Room &&
                        !x.Users
                        .Any(y => y.UserId == "testUser"))
                    .ToListAsync();
            var expectedChatNames = expected.Select(x => x.Name);
            
            var actual = await _sutChatService.GetAllPublicChats("testUser");
            var actualChatNames = expected.Select(x => x.Name);

            //Assert
            actual.Should().BeOfType<List<ReadChatDto>>();
            Assert.True(expectedChatNames.SequenceEqual(actualChatNames));
        }

        [Fact]
        public async Task GetAllPublicChats_PassUserIdToMethod_ShouldReturnPublicChatsWhichUserJoined()
        {
            //Arrange
            _mapper.Setup(x => x.Map<IEnumerable<ReadChatDto>>(It.IsAny<IEnumerable<Chat>>())).Returns((IEnumerable<Chat> source) =>
            {
                var toReturn = new List<ReadChatDto>();

                foreach (var chat in source)
                {
                    var chatDto = new ReadChatDto
                    {
                        Id = chat.Id,
                        Name = chat.Name,
                    };

                    toReturn.Add(chatDto);
                }

                return toReturn;
            });

            //Act
            var expected = await _context.Chats
                    .Where(x => x.Type == ChatType.Room &&
                        x.Users
                        .Any(y => y.UserId == "testUser"))
                    .ToListAsync();
            var expectedChatNames = expected.Select(x => x.Name);

            var actual = await _sutChatService.GetUserPublicChats("testUser");
            var actualChatNames = expected.Select(x => x.Name);

            //Assert
            actual.Should().BeOfType<List<ReadChatDto>>();
            Assert.True(expectedChatNames.SequenceEqual(actualChatNames));
        }

        [Fact]
        public async Task GetPrivateChats_PassUserIdToMethod_ShouldReturnUserPrivateChats()
        {
            //Arrange
            _mapper.Setup(x => x.Map<IEnumerable<ReadChatDto>>(It.IsAny<IEnumerable<Chat>>())).Returns((IEnumerable<Chat> source) =>
            {
                var toReturn = new List<ReadChatDto>();

                foreach (var chat in source)
                {
                    var chatDto = new ReadChatDto
                    {
                        Id = chat.Id,
                        Name = chat.Name,
                    };

                    toReturn.Add(chatDto);
                }

                return toReturn;
            });

            //Act
            var expected = await _context.Chats
                  .Where(x => x.Type == ChatType.Private
                      && x.Users
                          .Any(y => y.UserId == "testUser"))
                  .ToListAsync();
            var expectedChatNames = expected.Select(x => x.Name);

            var actual = await _sutChatService.GetPrivateChats("testUser");
            var actualChatNames = expected.Select(x => x.Name);

            //Assert
            actual.Should().BeOfType<List<ReadChatDto>>();
            Assert.True(expectedChatNames.SequenceEqual(actualChatNames));
        }

        [Fact]
        public async Task JoinRoom_PaasChatIdAndUserIdToTheMethod_ShouldSuccesfullyAddUserToRoom()
        {
            //Arrange
            var userId = "testUser";
            var chatId = 7;

            //Act
            await _sutChatService.JoinRoom(chatId, userId);

            var checkIfUserSuccesfullyJoinedRoom = _context.ChatUsers.FirstOrDefault(x => x.ChatId == chatId && x.UserId == userId);

            //Assert
            checkIfUserSuccesfullyJoinedRoom.Should().NotBeNull();
        }
    }
}
