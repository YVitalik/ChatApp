using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.ChatDTOs;
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

        [HttpGet("getallpublic")]
        public async Task<IActionResult> GetAllPublicChats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _chatService.GetAllPublicChats(userId);
            return Ok(result);
        }

        [HttpGet("getuserpublic")]
        public async Task<IActionResult> GetUserPublicChats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _chatService.GetUserPublicChats(userId);
            return Ok(result);
        }

        [HttpGet("createprivate/{targetId}")]
        public async Task<IActionResult> CreatePrivateChat(string targetId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _chatService.CreatePrivateRoom(userId, targetId);
            return Ok();
        }

        [HttpGet("getprivate")]
        public async Task<IActionResult> GetPrivateChats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _chatService.GetPrivateChats(userId);
            return Ok(result);
        }

        [HttpGet("joinroom/{chatId}")]
        public async Task<IActionResult> JoinRoom(int chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _chatService.JoinRoom(chatId, userId);
            return Ok();
        }

        [HttpGet("messages/{chatId}")]
        public async Task<IActionResult> GetChatMessages(int chatId)
        {
            var result = await _chatService.GetChatMessages(chatId);
            return Ok(result);
        }

        [HttpPost("createmessage")]
        public async Task<IActionResult> CreateMessage(CreateMessageDto messageDto)
        {
            messageDto.Name = User.Identity.Name;
            messageDto.SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var message = await _chatService.AddMessage(messageDto);

            return Ok(message);
        }

        [HttpPost("updatemessage")]
        public async Task<IActionResult> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            try
            {
                updateMessageDto.SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var message = await _chatService.UpdateMessage(updateMessageDto);

                return Ok(message);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (InvalidUserException ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpGet("deletemessage/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            try
            {
                var deleteMessageDto = new DeleteMessageDto
                {
                    MessageId = messageId,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                var deletedMessageId = await _chatService.DeleteMessage(deleteMessageDto);

                return Ok(deletedMessageId);
            }
            catch (InvalidUserException ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
    }
}
