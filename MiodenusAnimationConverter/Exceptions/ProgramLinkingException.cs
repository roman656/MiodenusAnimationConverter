using System;

namespace MiodenusAnimationConverter.Exceptions
{
    [Serializable]
    public class ProgramLinkingException : Exception
    {
        public ProgramLinkingException() {}

        public ProgramLinkingException(string message) : base(message) {}

        public ProgramLinkingException(string message, Exception inner) : base(message, inner) {}
    }
}