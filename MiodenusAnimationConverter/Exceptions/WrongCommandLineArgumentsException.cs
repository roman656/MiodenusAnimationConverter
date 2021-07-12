using System;

namespace MiodenusAnimationConverter.Exceptions
{
    [Serializable]
    public class WrongCommandLineArgumentsException : Exception
    {
        public WrongCommandLineArgumentsException() {}

        public WrongCommandLineArgumentsException(string message) : base(message) {}

        public WrongCommandLineArgumentsException(string message, Exception inner) : base(message, inner) {}
    }
}