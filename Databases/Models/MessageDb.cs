using System;

namespace ToqueToqueApi.Databases.Models
{
    public class MessageDb
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public DateTime SendingTime { get; set; }
        public int ConversationId { get; set; }
    }
}
