using Blazored.LocalStorage;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.BLL.DTOs;
using ChatApp.BLL.Infrastructure.RequestHelper;
using ChatApp.BLL.Interfaces;
using Newtonsoft.Json;
using ChatApp.DAL.Entities;

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
        
        public async Task<ServerResponse> CreatePrivateChat(string targetId)
        {
            var createPrivateChatUrl = _baseUrl + "chat/createprivate/" + targetId;

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(createPrivateChatUrl);

            return new ServerResponse
            {
                StatusCode = response.StatusCode,
                Response = response
            };
        }

        public async Task<ServerResponse> CreatePublicChat(string chatName)
        {
            var createPublicChatUrl = _baseUrl + "chat/createpublic/" + chatName;

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(createPublicChatUrl);

            return new ServerResponse
            {
                StatusCode = response.StatusCode,
                Response = response
            };
            
        }

        public async Task<ServerResponseWithChats> GetAllPublicChats()
        {
            var getAllPublicChatsUrl = _baseUrl + "chat/getallpublic";

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
            var getAllUsersUrl = _baseUrl + "user";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(getAllUsersUrl);

            return new ServerResponseWithUsers
            {
                StatusCode = response.StatusCode,
                Users = JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync())
            };
        }

        public async Task<ServerResponseWithChats> GetUserPrivateChats()
        {
            var getUserPrivateChatsUrl = _baseUrl + "chat/getprivate";

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
            var getAllUserChatsUrl = _baseUrl + "chat/getuserpublic";

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
            var joinPublicRoomUrl = _baseUrl + "chat/joinroom/" + chatId;

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(joinPublicRoomUrl);

            return new ServerResponse
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
