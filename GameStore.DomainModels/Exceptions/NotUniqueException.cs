using System;

namespace GameStore.DomainModels.Exceptions
{
    public class NotUniqueException : Exception
    {
        public NotUniqueException(string message) : base(message)
        {
        }
    }
}
