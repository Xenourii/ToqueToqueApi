using System;

namespace ToqueToqueApi.Exceptions
{
    public class MealTitleDuplicateForUserException : Exception
    {
        public MealTitleDuplicateForUserException(string message) : base(message)
        {
        }
    }
}