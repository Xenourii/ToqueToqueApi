using System.Collections.Generic;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models;

namespace ToqueToqueApi.Services
{
    public interface IConversationService
    {
        IEnumerable<ConversationDb> GetAll();
        IEnumerable<Conversation> GetAllFromUser(int userId);
        ConversationDb GetById(int id, int userId);
        ConversationDb Create(ConversationDb conversation);
        void Delete(int conversationId);
        IEnumerable<Message> GetMessagesFromConversation(int conversationId);
        void Create(MessageDb message);
    }
}