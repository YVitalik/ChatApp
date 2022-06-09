using ChatApp.BLL.DTOs.AdministrationDTOs;
using ChatApp.BLL.Interfaces;
using System.Text;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using ChatApp.BLL.Infrastructure.JwtHelper;

namespace ChatApp.IntermediateServices
{
    public class UserStateService : IUserStateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private string _baseUrl;

        public UserStateService(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration configuration)
        {
            _localStorage = localStorage;
            _baseUrl = configuration.GetValue<string>("ApiURLs:BasicUrl");
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> Login(LoginDTO loginDetails)
        {
            var loginUrl = _baseUrl + "account/login";
            var response = await _httpClient.PostAsync(loginUrl, new StringContent(JsonConvert.SerializeObject(loginDetails), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                await SaveToken(response);
                return response;
            }

            else
            {
                return response;
            }
        }

        public async Task<HttpResponseMessage> Register(RegisterDTO registerDetails)
        {
            var registerUrl = _baseUrl + "account/register";
            var response = await _httpClient.PostAsync(registerUrl, new StringContent(JsonConvert.SerializeObject(registerDetails), Encoding.UTF8, "application/json"));

            return response;
        }

        public async Task Logout()
        {
            await _localStorage.ClearAsync();
        }

        private async Task SaveToken(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenHelper = JsonConvert.DeserializeObject<TokenHelper>(responseContent);

            await _localStorage.SetItemAsync("authToken", tokenHelper.Token);
        }
        
    }
}
