using System.Collections.Generic;
using AutoMapper;
using ToqueToqueApi.Databases;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models;
using Profile = AutoMapper.Profile;

namespace ToqueToqueApi.Profiles
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<ConversationDb, Conversation>()
                .ForMember(dest => dest.UsersId,
                    opt => opt.MapFrom<ConversationResolver>());

            CreateMap<Conversation, ConversationDb>()
                .ForMember(dest => dest.ConversationUser,
                    opt => opt.MapFrom<ConversationDbResolver>());
        }
    }

    public class ConversationResolver : IValueResolver<ConversationDb, Conversation, List<int>>
    {
        public List<int> Resolve(ConversationDb source, Conversation destination, List<int> destMember, ResolutionContext context)
        {
            var usersId = new List<int>();
            foreach (var conversationUserDb in source.ConversationUser)
                usersId.Add(conversationUserDb.UserId);
            return usersId;
        }
    }

    public class ConversationDbResolver : IValueResolver<Conversation, ConversationDb, List<ConversationUserDb>>
    {
        private readonly ToqueToqueContext _dbContext;

        public ConversationDbResolver(ToqueToqueContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ConversationUserDb> Resolve(Conversation source, ConversationDb destination, List<ConversationUserDb> destMember, ResolutionContext context)
        {
            var conversationUsers = new List<ConversationUserDb>();
            foreach (var userId in source.UsersId)
            {
                var userDb = _dbContext.Users.Find(userId);
                var conversationUser = new ConversationUserDb
                {
                    Conversation = destination,
                    User = userDb
                };
                conversationUsers.Add(conversationUser);
            }

            return conversationUsers;
        }
    }
}