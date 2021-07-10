using System;

namespace MiodenusAnimationConverter.Exceptions
{
    [Serializable]
    public class CommandLineArgumentsException : Exception
    {
        public CommandLineArgumentsException() {}

        public CommandLineArgumentsException(string message) : base(message) {}

        public CommandLineArgumentsException(string message, Exception inner) : base(message, inner) {}
    }
}