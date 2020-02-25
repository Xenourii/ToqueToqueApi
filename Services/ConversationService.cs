using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ToqueToqueApi.Databases;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Exceptions;
using ToqueToqueApi.Models;

namespace ToqueToqueApi.Services
{
    public class ConversationService : IConversationService
    {
        private readonly ToqueToqueContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public ConversationService(ToqueToqueContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        /// For test use only
        public IEnumerable<ConversationDb> GetAll() => _dbContext.Conversations.Include(c => c.ConversationUser);

        /// <summary>
        /// Récupère toutes les conversations d'un utilisateur
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Conversation> GetAllFromUser(int userId)
        {
            var conversationsDb = _dbContext.Conversations.Where(x => x.ConversationUser.Any(y => y.UserId == userId)).Include(c => c.ConversationUser);
            var conversations = _mapper.Map<IList<Conversation>>(conversationsDb);

            foreach (var conversation in conversations)
            {
                var userNames = string.Empty;
                foreach (var id in conversation.UsersId)
                {
                    var userDb = _dbContext.Users.Find(id);
                    userNames += $"{userDb.FirstName}, ";
                }

                conversation.UserNames = userNames.Remove(userNames.Length - ", ".Length);
            }

            return conversations;
        }

        /// <summary>
        /// Récupère une conversation selon son id en s'assurant que l'utilisateur fasse partie de cette conversation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ConversationDb GetById(int id, int userId)
        {
            // On s'assure que la conversation existe
            if (!_dbContext.Conversations.Any(x => x.Id == id))
                throw new NotFoundException($"Conversation id '{id} not found.");

            var conversation = _dbContext.Conversations
                .Include(c => c.ConversationUser)
                .First(c => c.Id == id);

            // Si l'utilisateur ne fait pas partie de la conversation on ne lui montre pas
            if (conversation.ConversationUser.All(x => x.UserId != userId))
                throw new ConversationException($"User id '{userId}' is not part of this conversation.");

            return conversation;
        }


        /// <summary>
        /// Crée une nouvelle conversation
        /// </summary>
        /// <param name="conversation"></param>
        /// <returns></returns>
        public ConversationDb Create(ConversationDb conversation)
        {
            // Il nous faut au moins deux utilisateurs pour une conversation
            if (conversation.ConversationUser.Count < 2)
                throw new ConversationException("We need at least 2 user to make a conversation.");

            // Si une conversation avec les même participants existe déjà
            if (_dbContext.Conversations
                .Include(x => x.ConversationUser)
                .ToList()
                .Any(x => x.ConversationUser.Count == conversation.ConversationUser.Count && x.ConversationUser
                              .All(conversation.ConversationUser.Contains)))
                throw new ConversationException("This conversation already exists.");

            _dbContext.Conversations.Add(conversation);
            _dbContext.SaveChanges();
            return conversation;
        }

        public void Delete(int conversationId)
        {
            var conversation = _dbContext.Conversations.Find(conversationId);
            if (conversation == null)
                throw new NotFoundException($"Conversation {conversationId} not found.");

            _dbContext.Conversations.Remove(conversation);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Récupère une liste de messages venant de la conversation
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        public IEnumerable<Message> GetMessagesFromConversation(int conversationId)
        {
            // On vérifie que la conversation existe
            if (!_dbContext.Conversations.Any(x => x.Id == conversationId))
                throw new NotFoundException($"Conversation with id '{conversationId}' not found.");

            var messagesDb = _dbContext.Messages.Where(x => x.ConversationId == conversationId).OrderBy(c => c.SendingTime);
            var messages = _mapper.Map<IList<Message>>(messagesDb);

            foreach (var message in messages)
            {
                var userDb = _dbContext.Users.Find(message.UserId);
                message.Username = userDb.FirstName;
            }

            return messages;
        }

        public void Create(MessageDb message)
        {
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
        }
    }
}