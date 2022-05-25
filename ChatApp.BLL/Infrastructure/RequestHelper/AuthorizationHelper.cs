using Blazored.LocalStorage;
using System.Net.Http.Headers;


namespace ChatApp.BLL.Infrastructure.RequestHelper
{
	public class AuthorizationHelper
	{
        public async Task SetAuthorizationHeader(ILocalStorageService localStorage, HttpClient httpClient)
        {
            var token = await localStorage.GetItemAsync<string>("authToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
