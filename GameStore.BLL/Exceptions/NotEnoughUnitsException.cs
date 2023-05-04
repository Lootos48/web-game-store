using System;

namespace GameStore.BLL.Exceptions
{
    public class NotEnoughUnitsException : Exception
    {
        public NotEnoughUnitsException(string message) : base(message)
        {
        }
    }
}
