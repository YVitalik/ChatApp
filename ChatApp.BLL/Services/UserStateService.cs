using ChatApp.BLL.DTOs.AdministrationDTOs;
using ChatApp.BLL.Interfaces;
using System.Text;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using ChatApp.BLL.Infrastructure.JwtHelper;
using ChatApp.BLL.Infrastructure.RequestHelper;

namespace ChatApp.BLL.Services
{
    public class UserStateService : AuthorizationHelper ,IUserStateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public UserStateService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public async Task<string> Login(LoginDTO loginDetails)
        {
            var response = await _httpClient.PostAsync("https://localhost:7194/chatapi/account/login", new StringContent(JsonConvert.SerializeObject(loginDetails), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                await SaveToken(response);
            }

            return "Something went wrong!";
        }

        public async Task<string> Register(RegisterDTO registerDetails)
        {
            var response = await _httpClient.PostAsync("https://localhost:7194/chatapi/account/register", new StringContent(JsonConvert.SerializeObject(registerDetails), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("You are registered successfully");
                return "Yeah";
            }

            return "Something went wrong!";
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
