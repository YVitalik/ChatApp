using ChatApp.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("chatapi")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("createpublic/{roomName}")]
        public async Task<IActionResult> CreatePublicRoom(string roomName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _chatService.CreatePublicRoom(roomName, userId);

            return Ok();
        }
    }
}
