﻿using ChatApp.BLL.DTOs.AdministrationDTOs;
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
        private readonly IConfiguration _configuration;

        public UserStateService(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration configuration)
        {
            _localStorage = localStorage;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> Login(LoginDTO loginDetails)
        {
            var loginUrl = _configuration.GetValue<string>("ApiURLs:BasicUrl") + "account/login";
            var response = await _httpClient.PostAsync(loginUrl, new StringContent(JsonConvert.SerializeObject(loginDetails), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                await SaveToken(response);
            }

            return "Something went wrong!";
        }

        public async Task<string> Register(RegisterDTO registerDetails)
        {
            var registerUrl = _configuration.GetValue<string>("ApiURLs:BasicUrl") + "account/register";
            var response = await _httpClient.PostAsync(registerUrl, new StringContent(JsonConvert.SerializeObject(registerDetails), Encoding.UTF8, "application/json"));

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