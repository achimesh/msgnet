using System;
namespace MsgNet.Abstractions;

public class MsgNetException: Exception
{
    public MsgNetException(string message): base(message)
    {

    }

    public MsgNetException(string message, Exception exception): base(message, exception)
    {

    }
}

