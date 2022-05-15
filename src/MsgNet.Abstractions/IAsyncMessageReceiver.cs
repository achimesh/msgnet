namespace MsgNet.Abstractions;

public interface IAsyncMessageReceiver<in TMessage> where TMessage : IAsyncMessage
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken);
}

public interface IAsyncMessageReceiver<in TMessage, TResult> where TMessage : IAsyncMessage<TResult>
{
    Task<TResult> HandleAsync(TMessage message, CancellationToken cancellationToken);
}