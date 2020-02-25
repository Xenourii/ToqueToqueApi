using System;

namespace ToqueToqueApi.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public DateTime SendingTime { get; set; }
        public int ConversationId { get; set; }
    }
}
