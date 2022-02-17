using System;

namespace MiodenusAnimationConverter.Exceptions
{
    [Serializable]
    public class WrongModelFileContentException : Exception
    {
        public WrongModelFileContentException() {}
        public WrongModelFileContentException(string message) : base(message) {}
        public WrongModelFileContentException(string message, Exception inner) : base(message, inner) {}
    }
}