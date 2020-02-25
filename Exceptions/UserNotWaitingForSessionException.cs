using System;

namespace ToqueToqueApi.Exceptions
{
    public class UserNotWaitingForSessionException : Exception
    {
        public UserNotWaitingForSessionException(string message) : base(message)
        {
        }
    }
}