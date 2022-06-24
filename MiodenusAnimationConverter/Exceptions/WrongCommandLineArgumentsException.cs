namespace MiodenusAnimationConverter.Exceptions;

using System;

[Serializable]
public class WrongCommandLineArgumentsException : Exception
{
    public WrongCommandLineArgumentsException() {}
    public WrongCommandLineArgumentsException(string message) : base(message) {}
    public WrongCommandLineArgumentsException(string message, Exception inner) : base(message, inner) {}
}