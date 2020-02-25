using System;

namespace ToqueToqueApi.Hubs
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public DateTime SendingTime { get; set; }
    }
}