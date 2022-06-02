using Blazored.LocalStorage;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.BLL.DTOs;
using ChatApp.BLL.Infrastructure.RequestHelper;
using ChatApp.BLL.Interfaces;
using Newtonsoft.Json;

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
    }
}
