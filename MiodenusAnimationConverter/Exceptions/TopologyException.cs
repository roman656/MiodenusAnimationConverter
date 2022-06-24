namespace MiodenusAnimationConverter.Exceptions;

using System;

[Serializable]
public class TopologyException : Exception
{
    public TopologyException() {}
    public TopologyException(string message) : base(message) {}
    public TopologyException(string message, Exception inner) : base(message, inner) {}
}