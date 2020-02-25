using System;

namespace ToqueToqueApi.Exceptions
{
    public class EmailAlreadyTakenException : Exception
    {
        public EmailAlreadyTakenException(string message) : base(message)
        {
        }
    }
}