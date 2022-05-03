using System;
namespace MsgNet.Abstractions;

public interface IMessageReceiver<in TMessage> where TMessage : IMessage
{
    void Handle(TMessage message);
}

public interface IMessageReceiver<in TMessage, TResult> where TMessage : IMessage<TResult>
{
    TResult Handle(TMessage message);
}