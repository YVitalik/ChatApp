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
    [Route("api/message")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> GetChatMessages(ReadChatMessagesDto readChatMessagesDto)
        {
            var result = await _messageService.GetChatMessages(readChatMessagesDto.ChatId, readChatMessagesDto.AmountOfMessagesToTake, readChatMessagesDto.TimeOfSending);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMessage(CreateMessageDto messageDto)
        {
            messageDto.Name = User.Identity.Name;
            messageDto.SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var message = await _messageService.AddMessage(messageDto);

            return Ok(message);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            try
            {
                updateMessageDto.SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var message = await _messageService.UpdateMessage(updateMessageDto);

                return Ok(message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidUserException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reply")]
        public async Task<IActionResult> ReplyMessage(ReplyMessageDto replyMessageDto)
        {
            try
            {
                var message = await _messageService.ReplyMessage(replyMessageDto);

                return Ok(message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("delete/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            try
            {
                var deleteMessageDto = new DeleteMessageDto
                {
                    MessageId = messageId,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                var deletedMessageId = await _messageService.DeleteMessage(deleteMessageDto);

                return Ok(deletedMessageId);
            }
            catch (InvalidUserException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
