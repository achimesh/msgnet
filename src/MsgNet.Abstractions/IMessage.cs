using System;
namespace MsgNet.Abstractions;

public interface IMessage
{

}

public interface IMessage<out TResult>
{

}