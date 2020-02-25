using System;

namespace ToqueToqueApi.Exceptions
{
    public class MealIdNotFoundException : Exception
    {
        public MealIdNotFoundException(string message) : base(message)
        {
        }
    }
}