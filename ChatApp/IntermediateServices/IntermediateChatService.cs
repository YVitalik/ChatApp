using Blazored.LocalStorage;
using ChatApp.BLL.Infrastructure.RequestHelper;
using ChatApp.BLL.Interfaces;

namespace ChatApp.IntermediateServices
{
    public class IntermediateChatService : AuthorizationHelper, IIntermediateChatService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _configuration;

        public IntermediateChatService(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration configuration)
        {
            _localStorage = localStorage;
            _configuration = configuration;
            _httpClient = httpClient;
        }
        
        public async Task<bool> CreatePublicChat(string chatName)
        {
            var createPublicChatUrl = _configuration.GetValue<string>("ApiURLs:BasicUrl") + "createpublic/" + chatName;
            
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
    }
}
