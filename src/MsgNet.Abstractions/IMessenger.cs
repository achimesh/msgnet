namespace MsgNet.Abstractions;

public interface IMessenger
{
    void Send(IMessage value);
    TResult Send<TResult>(IMessage<TResult> value);
    Task SendAsync(IAsyncMessage value, CancellationToken cancellationToken = default);
    Task<TResult> SendAsync<TResult>(IAsyncMessage<TResult> value, CancellationToken cancellationToken = default);
}