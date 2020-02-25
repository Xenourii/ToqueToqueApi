namespace ToqueToqueApi.Databases.Models
{
    public class ConversationUserDb
    {
        public int ConversationId { get; set; }
        public ConversationDb Conversation { get; set; }

        public int UserId { get; set; }
        public UserDb User { get; set; }
    }
}