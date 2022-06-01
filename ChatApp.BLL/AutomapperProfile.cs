using AutoMapper;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.DAL.Entities;

namespace ChatApp.BLL
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Chat, ReadChatDto>();
            CreateMap<Message, ReadMessageDto>();
        }
    }
}
