using System;

namespace ToqueToqueApi.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message):base(message)
        {
        }
    }
}