using AutoMapper;
using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.BLL.Interfaces;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Interfaces;

namespace ChatApp.BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserManagementService _userManagementService;
        public MessageService(IUnitOfWork unitOfWork, IMapper mapper, IUserManagementService userManagementSerivce)
        {
            _userManagementService = userManagementSerivce;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReadMessageDto> AddMessage(CreateMessageDto messageDto)
        {
            var message = new Message
            {
                ChatId = messageDto.ChatId,
                Text = messageDto.MessageText,
                Name = messageDto.Name,
                SenderId = messageDto.SenderId,
                CreatedAt = DateTime.Now
            };

            var messageToReturn = await _unitOfWork.Message.AddMessage(message);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReadMessageDto>(messageToReturn);
        }

        public async Task<int> DeleteMessage(DeleteMessageDto deleteMessageDto)
        {
            var messageFromDb = await _unitOfWork.Message.GetMessage(deleteMessageDto.MessageId);

            if (messageFromDb.SenderId == deleteMessageDto.UserId)
            {
                await _unitOfWork.Message.DeleteMessage(deleteMessageDto.MessageId);
                await _unitOfWork.SaveChangesAsync();
                return deleteMessageDto.MessageId;
            }
            else
            {
                throw new InvalidUserException("You can delete only your own messages!");
            }
        }

        public async Task<IEnumerable<ReadMessageDto>> GetChatMessages(int chatId, int amountOfMessagesToTake, DateTime? timeOfSending)
        {
            var chatMessages = await _unitOfWork.Message.GetChatMessages(chatId, amountOfMessagesToTake, timeOfSending);
            return _mapper.Map<IEnumerable<ReadMessageDto>>(chatMessages);
        }

        public async Task<ReadMessageDto> ReplyMessage(ReplyMessageDto replyMessageDto)
        {
            var chat = await _unitOfWork.Room.GetChatByName(replyMessageDto.ChatNameToReply);

            if (chat is null)
            {
                throw new ArgumentNullException("Chat with such name does not exist!");
            }

            var message = await _unitOfWork.Message.GetMessage(replyMessageDto.MessageId);
            var messageSenderName = await _userManagementService.GetUserByIdAsync(message.SenderId);

            var repliedMessage = new Message();

            if (message.Name == "Replied from: " + messageSenderName.UserName)
            {
                repliedMessage = new Message
                {
                    ChatId = chat.Id,
                    Text = message.Text,
                    Name = message.Name,
                    SenderId = message.SenderId,
                    CreatedAt = DateTime.Now
                };
            }

            else
            {
                repliedMessage = new Message
                {
                    ChatId = chat.Id,
                    Text = message.Text,
                    Name = "Replied from: " + message.Name,
                    SenderId = message.SenderId,
                    CreatedAt = DateTime.Now
                };
            }

            var messageToReturn = await _unitOfWork.Message.AddMessage(repliedMessage);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReadMessageDto>(messageToReturn);
        }

        public async Task<ReadMessageDto> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            var messageFromDb = await _unitOfWork.Message.GetMessage(updateMessageDto.Id);

            if (messageFromDb != null)
            {
                if (messageFromDb.SenderId == updateMessageDto.SenderId)
                {
                    messageFromDb.Text = updateMessageDto.Text;
                    var messageToReturn = _unitOfWork.Message.UpdateMessage(messageFromDb);

                    await _unitOfWork.SaveChangesAsync();

                    return _mapper.Map<ReadMessageDto>(messageToReturn);
                }
                else
                {
                    throw new InvalidUserException("You can edit and delete only your own messages!");
                }
            }
            else
            {
                throw new ArgumentNullException("Message with such id doesnt exist!");
            }
        }
    }
}
