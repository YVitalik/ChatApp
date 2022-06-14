using Blazored.LocalStorage;
using ChatApp.BLL.DTOs;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.BLL.Infrastructure.RequestHelper;
using ChatApp.BLL.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace ChatApp.IntermediateServices
{
    public class IntemediateMessageService : AuthorizationHelper, IIntermediateMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private string _baseUrl;

        public IntemediateMessageService(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration configuration)
        {
            _localStorage = localStorage;
            _baseUrl = configuration.GetValue<string>("ApiURLs:BasicUrl");
            _httpClient = httpClient;
        }

        public async Task<ReadMessageDto?> CreateMessage(CreateMessageDto messageDto)
        {
            var createMessageUrl = _baseUrl + "message/create";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.PostAsync(createMessageUrl, new StringContent(JsonConvert.SerializeObject(messageDto), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ReadMessageDto>(responseContent);
            }
            else
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> DeleteMessage(int messageId)
        {
            var deleteMessageUrl = _baseUrl + "message/delete/" + messageId;

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.GetAsync(deleteMessageUrl);

            return response;
        }

        public async Task<ServerResponseWithMessages> GetChatMessages(ReadChatMessagesDto readChatMessagesDto)
        {
            var getChatMessagesUrl = _baseUrl + "message";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.PostAsync(getChatMessagesUrl, new StringContent(JsonConvert.SerializeObject(readChatMessagesDto), Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();

            var chatMessages = JsonConvert.DeserializeObject<List<ReadMessageDto>>(responseContent);

            return new ServerResponseWithMessages
            {
                StatusCode = response.StatusCode,
                Messages = chatMessages
            };
        }

        public async Task<HttpResponseMessage> ReplyMessage(ReplyMessageDto replyMessageDto)
        {
            var replyMessageUrl = _baseUrl + "message/reply";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.PostAsync(replyMessageUrl, new StringContent(JsonConvert.SerializeObject(replyMessageDto), Encoding.UTF8, "application/json"));

            return response;
        }

        public async Task<HttpResponseMessage> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            var updateMessageUrl = _baseUrl + "message/update";

            await SetAuthorizationHeader(_localStorage, _httpClient);

            var response = await _httpClient.PostAsync(updateMessageUrl, new StringContent(JsonConvert.SerializeObject(updateMessageDto), Encoding.UTF8, "application/json"));

            return response;
        }
    }
}
