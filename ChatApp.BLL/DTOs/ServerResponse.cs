using System.Net;

namespace ChatApp.BLL.DTOs
{
    public class ServerResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public HttpResponseMessage? Response { get; set; }
    }
}
