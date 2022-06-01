using Blazored.LocalStorage;
using ChatApp.BLL.DTOs.ChatDTOs;
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
        
        public async Task<bool> CreatePublicChat(string chatName)
        {
            var createPublicChatUrl = _baseUrl + "createpublic/" + chatName;

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(createPublicChatUrl);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Created succesfully!");
                return true;
            }

            else
            {
                Console.WriteLine(response.StatusCode);
                return false;
            }
        }

        public async Task<IEnumerable<ReadChatDto>> GetAllPublicChats()
        {
            var getAllPublicChatsUrl = _baseUrl + "getallpublic";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(getAllPublicChatsUrl);
            var responseContent = await response.Content.ReadAsStringAsync();

            var publicChats = JsonConvert.DeserializeObject<IEnumerable<ReadChatDto>>(responseContent);

            return publicChats;
        }

        public async Task<IEnumerable<ReadChatDto>> GetUserPublicChats()
        {
            var getAllUserChatsUrl = _baseUrl + "getuserpublic";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(getAllUserChatsUrl);
            var responseContent = await response.Content.ReadAsStringAsync();

            var publicUserChats = JsonConvert.DeserializeObject<IEnumerable<ReadChatDto>>(responseContent);

            return publicUserChats;
        }

        public async Task<bool> JoinRoom(int chatId)
        {
            var joinPublicRoomUrl = _baseUrl + "joinroom/" + chatId;

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(joinPublicRoomUrl);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("You have joined succesfully!");
                return true;
            }

            else
            {
                Console.WriteLine(response.StatusCode);
                return false;
            }
        }
    }
}
