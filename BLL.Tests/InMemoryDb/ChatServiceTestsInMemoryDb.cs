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

            const string ID_OF_FIRST_USER_OF_PRIVATE_CHAT = "afalfhaslf";
            const string ID_OF_SECOND_USER_OF_PRIVATE_CHAT = "qoifncoaho";

            const string USERNAME_OF_FIRST_USER_OF_PRIVATE_CHAT = "user1";
            const string USERNAME_OF_SECOND_USER_OF_PRIVATE_CHAT = "user2";

            usersForPrivateChat.Add(new User
            {
                Id = ID_OF_FIRST_USER_OF_PRIVATE_CHAT,
                UserName = USERNAME_OF_FIRST_USER_OF_PRIVATE_CHAT
            });
            usersForPrivateChat.Add(new User
            {
                Id = ID_OF_SECOND_USER_OF_PRIVATE_CHAT,
                UserName = USERNAME_OF_SECOND_USER_OF_PRIVATE_CHAT
            });
            
            _userManagementService.Setup(x => x.GetUsersForPrivateChats(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(usersForPrivateChat);

            //Assert
            await _sutChatService.Invoking(x => x.CreatePrivateRoom(ID_OF_FIRST_USER_OF_PRIVATE_CHAT, ID_OF_SECOND_USER_OF_PRIVATE_CHAT)).Should().ThrowAsync<ItemWithSuchNameAlreadyExists>();
        }

        [Fact]
        public async Task CreatePrivateRoom_PassRootIdAndTargetIdToMethod_ShouldSuccesfullyAddNewPrivateChat()
        {
            //Arrange
            var usersForPrivateChat = new List<User>();

            const string ID_OF_FIRST_USER_OF_PRIVATE_CHAT = "afalurjtlh";
            const string ID_OF_SECOND_USER_OF_PRIVATE_CHAT = "qoifnpiitm";

            const string USERNAME_OF_FIRST_USER_OF_PRIVATE_CHAT = "user3";
            const string USERNAME_OF_SECOND_USER_OF_PRIVATE_CHAT = "user4";

            usersForPrivateChat.Add(new User
            {
                Id = ID_OF_FIRST_USER_OF_PRIVATE_CHAT,
                UserName = USERNAME_OF_FIRST_USER_OF_PRIVATE_CHAT
            });
            usersForPrivateChat.Add(new User
            {
                Id = ID_OF_SECOND_USER_OF_PRIVATE_CHAT,
                UserName = USERNAME_OF_SECOND_USER_OF_PRIVATE_CHAT
            });
            _userManagementService.Setup(x => x.GetUsersForPrivateChats(ID_OF_FIRST_USER_OF_PRIVATE_CHAT, ID_OF_SECOND_USER_OF_PRIVATE_CHAT)).ReturnsAsync(usersForPrivateChat);

            //Act
            await _sutChatService.CreatePrivateRoom(ID_OF_FIRST_USER_OF_PRIVATE_CHAT, ID_OF_SECOND_USER_OF_PRIVATE_CHAT);

            //Assert
            var checkIfPrivateChatIsAdded = _context.Chats.FirstOrDefault(x => x.Name == "user3 and user4 private chat");
            checkIfPrivateChatIsAdded.Should().NotBeNull();
        }

        [Fact]
        public async Task CreatePublicChat_PassUserIdAndChatNameToMethod_ShouldThrowExceptionIfChatWithSuchNameAlreadyExists()
        {
            //Arrange
            const string CHAT_NAME = "Chat 1";
            const string USER_ID = "asfkljasfsjf";

            //Assert
            await _sutChatService.Invoking(x => x.CreatePublicRoom(CHAT_NAME, USER_ID)).Should().ThrowAsync<ItemWithSuchNameAlreadyExists>();
        }

        [Fact]
        public async Task CreatePublicChat_PassUserIdAndChatNameToMethod_ShouldSuccesfullyAddNewPublicChat()
        {
            //Arrange
            const string CHAT_NAME = "Hello I am New chat";
            const string USER_ID = "yurbglaiufh";

            //Act
            await _sutChatService.CreatePublicRoom(CHAT_NAME, USER_ID);

            //Assert
            var checkIfPublicChatIsAdded = _context.Chats.FirstOrDefault(x => x.Name == CHAT_NAME);
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

            const string USER_ID = "testUser";

            //Act
            var expected = await _context.Chats
                    .Where(x => x.Type == ChatType.Room &&
                        !x.Users
                        .Any(y => y.UserId == USER_ID))
                    .ToListAsync();
            var expectedChatNames = expected.Select(x => x.Name);
            
            var actual = await _sutChatService.GetAllPublicChats(USER_ID);
            var actualChatNames = expected.Select(x => x.Name);

            //Assert
            actual.Should().BeOfType<List<ReadChatDto>>();
            actualChatNames.Should().Equal(expectedChatNames);
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

            const string USER_ID = "testUser";

            //Act
            var expected = await _context.Chats
                    .Where(x => x.Type == ChatType.Room &&
                        x.Users
                        .Any(y => y.UserId == USER_ID))
                    .ToListAsync();
            var expectedChatNames = expected.Select(x => x.Name);

            var actual = await _sutChatService.GetUserPublicChats(USER_ID);
            var actualChatNames = expected.Select(x => x.Name);

            //Assert
            actual.Should().BeOfType<List<ReadChatDto>>();
            actualChatNames.Should().Equal(expectedChatNames);
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

            const string USER_ID = "testUser";

            //Act
            var expected = await _context.Chats
                  .Where(x => x.Type == ChatType.Private
                      && x.Users
                          .Any(y => y.UserId == USER_ID))
                  .ToListAsync();
            var expectedChatNames = expected.Select(x => x.Name);

            var actual = await _sutChatService.GetPrivateChats(USER_ID);
            var actualChatNames = expected.Select(x => x.Name);

            //Assert
            actual.Should().BeOfType<List<ReadChatDto>>();
            actualChatNames.Should().Equal(expectedChatNames);
        }

        [Fact]
        public async Task JoinRoom_PaasChatIdAndUserIdToTheMethod_ShouldSuccesfullyAddUserToRoom()
        {
            //Arrange
            const string USER_ID = "testUser";
            const int CHAT_ID = 7;

            //Act
            await _sutChatService.JoinRoom(CHAT_ID, USER_ID);

            var checkIfUserSuccesfullyJoinedRoom = _context.ChatUsers.FirstOrDefault(x => x.ChatId == CHAT_ID && x.UserId == USER_ID);

            //Assert
            checkIfUserSuccesfullyJoinedRoom.Should().NotBeNull();
        }
    }
}
