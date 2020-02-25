using System.Collections.Generic;

namespace ToqueToqueApi.Databases.Models
{
    public class ConversationDb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ConversationUserDb> ConversationUser { get; set; }
    }
}
