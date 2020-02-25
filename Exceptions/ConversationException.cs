using System;
using System.Runtime.Serialization;

namespace ToqueToqueApi.Exceptions
{
    public class ConversationException : Exception
    {
        public ConversationException(string message):base(message)
        {
        }
    }
}