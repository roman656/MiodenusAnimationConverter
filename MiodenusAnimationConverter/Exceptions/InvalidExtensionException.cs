using System;

namespace MiodenusAnimationConverter.Exceptions
{
    [Serializable]
    public class InvalidExtensionException : Exception
    {
        public InvalidExtensionException() {}

        public InvalidExtensionException(string message) : base(message) {}

        public InvalidExtensionException(string message, Exception inner) : base(message, inner) {}
    }
}