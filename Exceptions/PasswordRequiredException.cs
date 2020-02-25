using System;

namespace ToqueToqueApi.Exceptions
{
    public class PasswordRequiredException : Exception
    {
        public PasswordRequiredException(string message) : base(message)
        {
        }
    }
}