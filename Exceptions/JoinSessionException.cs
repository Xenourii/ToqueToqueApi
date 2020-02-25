using System;

namespace ToqueToqueApi.Exceptions
{
    public class JoinSessionException : Exception
    {
        public JoinSessionException(string message) : base(message)
        {
        }
    }
}