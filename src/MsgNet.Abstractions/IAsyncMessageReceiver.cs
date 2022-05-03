using System;
namespace MsgNet.Abstractions;

public interface IAsyncMessageReceiver<in TMessage> where TMessage : IAsyncMessage
{
    Task Handle(TMessage message, CancellationToken cancellationToken);
}

public interface IAsyncMessageReceiver<in TMessage, TResult> where TMessage : IAsyncMessage<TResult>
{
    Task<TResult> Handle(TMessage message, CancellationToken cancellationToken);
}