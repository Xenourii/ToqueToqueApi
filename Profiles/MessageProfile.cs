using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models;
using Profile = AutoMapper.Profile;

namespace ToqueToqueApi.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageDb>();
            CreateMap<MessageDb, Message>();
        }
    }
}