using Blazored.LocalStorage;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.BLL.DTOs;
using ChatApp.BLL.Infrastructure.RequestHelper;
using ChatApp.BLL.Interfaces;
using Newtonsoft.Json;
using ChatApp.DAL.Entities;
using System.Text;

namespace ChatApp.IntermediateServices
{
    public class IntermediateChatService : AuthorizationHelper, IIntermediateChatService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private string _baseUrl;

        public IntermediateChatService(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration configuration)
        {
            _localStorage = localStorage;
            _baseUrl = configuration.GetValue<string>("ApiURLs:BasicUrl");
            _httpClient = httpClient;
        }

        public async Task<Message?> CreateMessage(CreateMessageDto messageDto)
        {
            var createMessageUrl = _baseUrl + "createmessage";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.PostAsync(createMessageUrl, new StringContent(JsonConvert.SerializeObject(messageDto), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Message>(responseContent);
            }
            else
            {
                return null;
            }
        }

        public async Task<ServerResponse> CreatePrivateChat(string targetId)
        {
            var createPrivateChatUrl = _baseUrl + "createprivate/" + targetId;

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(createPrivateChatUrl);

            return new ServerResponse
            {
                StatusCode = response.StatusCode
            };
        }

        public async Task<ServerResponse> CreatePublicChat(string chatName)
        {
            var createPublicChatUrl = _baseUrl + "createpublic/" + chatName;

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(createPublicChatUrl);

            return new ServerResponse
            {
                StatusCode = response.StatusCode
            };
            
        }

        public async Task<int?> DeleteMessage(int messageId)
        {
            var deleteMessageUrl = _baseUrl + "deletemessage/" + messageId;

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(deleteMessageUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<int>(responseContent);
            }
            else
            {
                return null;
            }
        }

        public async Task<ServerResponseWithChats> GetAllPublicChats()
        {
            var getAllPublicChatsUrl = _baseUrl + "getallpublic";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(getAllPublicChatsUrl);
            var responseContent = await response.Content.ReadAsStringAsync();

            var publicChats = JsonConvert.DeserializeObject<IEnumerable<ReadChatDto>>(responseContent);

            return new ServerResponseWithChats
            {
                StatusCode = response.StatusCode,
                Chats = publicChats
            };
        }

        public async Task<ServerResponseWithUsers> GetApplicationUsers()
        {
            var getAllUsersUrl = _baseUrl + "getusers";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(getAllUsersUrl);

            return new ServerResponseWithUsers
            {
                StatusCode = response.StatusCode,
                Users = JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync())
            };
        }

        public async Task<ServerResponseWithMessages> GetChatMessages(ReadChatMessagesDto readChatMessagesDto)
        {
            var getChatMessagesUrl = _baseUrl + "messages";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.PostAsync(getChatMessagesUrl, new StringContent(JsonConvert.SerializeObject(readChatMessagesDto), Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();

            var chatMessages = JsonConvert.DeserializeObject<List<Message>>(responseContent);

            return new ServerResponseWithMessages
            {
                StatusCode = response.StatusCode,
                Messages = chatMessages
            };
        }

        public async Task<ServerResponseWithChats> GetUserPrivateChats()
        {
            var getUserPrivateChatsUrl = _baseUrl + "getprivate";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(getUserPrivateChatsUrl);
            var userPrivateChats = JsonConvert.DeserializeObject<IEnumerable<ReadChatDto>>(await response.Content.ReadAsStringAsync());

            return new ServerResponseWithChats
            {
                StatusCode = response.StatusCode,
                Chats = userPrivateChats
            };
        }

        public async Task<ServerResponseWithChats> GetUserPublicChats()
        {
            var getAllUserChatsUrl = _baseUrl + "getuserpublic";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(getAllUserChatsUrl);
            var responseContent = await response.Content.ReadAsStringAsync();

            var publicUserChats = JsonConvert.DeserializeObject<IEnumerable<ReadChatDto>>(responseContent);

            return new ServerResponseWithChats
            {
                StatusCode = response.StatusCode,
                Chats = publicUserChats
            };
        }

        public async Task<ServerResponse> JoinRoom(int chatId)
        {
            var joinPublicRoomUrl = _baseUrl + "joinroom/" + chatId;

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(joinPublicRoomUrl);

            return new ServerResponse
            {
                StatusCode = response.StatusCode
            };
        }

        public async Task<Message?> ReplyMessage(ReplyMessageDto replyMessageDto)
        {
            var replyMessageUrl = _baseUrl + "replymessage";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.PostAsync(replyMessageUrl, new StringContent(JsonConvert.SerializeObject(replyMessageDto), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var messageToReturn = JsonConvert.DeserializeObject<Message>(responseContent);
                return messageToReturn;
            }

            else
            {
                return null;
            }
        }

        public async Task<Message?> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            var updateMessageUrl = _baseUrl + "updatemessage";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.PostAsync(updateMessageUrl, new StringContent(JsonConvert.SerializeObject(updateMessageDto), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Message>(responseContent);
            }

            else
            {
                return null;
            }
        }
    }
}
