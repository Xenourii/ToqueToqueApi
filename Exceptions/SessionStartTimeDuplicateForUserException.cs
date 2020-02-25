using System;

namespace ToqueToqueApi.Exceptions
{
    public class SessionStartTimeDuplicateForUserException : Exception
    {
        public SessionStartTimeDuplicateForUserException(string message) : base(message)
        {
        }
    }
}