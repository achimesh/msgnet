using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MsgNet.Abstractions;

namespace MsgNet;

internal class MsgNetMessenger: IMessenger
{
    private readonly IServiceProvider ServiceProvider;

    public MsgNetMessenger(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public void Send(IMessage value)
    {
        var messageType = value.GetType();
        var abstractReceiverType = typeof(IMessageReceiver<>).MakeGenericType(messageType);

        var receiverInstance = ServiceProvider.GetRequiredService(abstractReceiverType);
        var methodInfo = GetMethodInfo(receiverInstance, "Handle");

        methodInfo.Invoke(receiverInstance, new[] { value });
    }

    public TResult Send<TResult>(IMessage<TResult> value)
    {
        var messageType = value.GetType();
        var abstractReceiverType = typeof(IMessageReceiver<,>).MakeGenericType(messageType, typeof(TResult));

        var receiverInstance = ServiceProvider.GetRequiredService(abstractReceiverType);
        var methodInfo = GetMethodInfo(receiverInstance, "Handle");

        var result = methodInfo.Invoke(receiverInstance, new[] { value });

        if (result != null)
        {
            return (TResult)result;
        }
        else
        {
            throw new MsgNetException($"Execution method from \"{receiverInstance.GetType().FullName}\" does not return any value");
        }
    }

    public Task SendAsync(IAsyncMessage value, CancellationToken cancellationToken = default)
    {
        var messageType = value.GetType();
        var abstractReceiverType = typeof(IAsyncMessageReceiver<>).MakeGenericType(messageType);

        var receiverInstance = ServiceProvider.GetRequiredService(abstractReceiverType);
        var methodInfo = GetMethodInfo(receiverInstance, "HandleAsync");

        var result = methodInfo.Invoke(receiverInstance, new object[] { value, cancellationToken });

        if (result != null)
        {
            return (Task)result;
        }
        else
        {
            throw new MsgNetException($"Execution method from \"{receiverInstance.GetType().FullName}\" does not return any value");
        }
    }

    public Task<TResult> SendAsync<TResult>(IAsyncMessage<TResult> value, CancellationToken cancellationToken = default)
    {
        var messageType = value.GetType();
        var abstractReceiverType = typeof(IAsyncMessageReceiver<,>).MakeGenericType(messageType, typeof(TResult));

        var receiverInstance = ServiceProvider.GetRequiredService(abstractReceiverType);
        var methodInfo = GetMethodInfo(receiverInstance, "HandleAsync");

        var result = methodInfo.Invoke(receiverInstance, new object[] { value, cancellationToken });

        if (result != null)
        {
            return (Task<TResult>)result;
        }
        else
        {
            throw new MsgNetException($"Execution method from \"{receiverInstance.GetType().FullName}\" does not return any value");
        }
    }

    private static MethodInfo GetMethodInfo(object instance, string methodName)
    {
        var methodInfo = instance.GetType().GetMethod(methodName);

        if (methodInfo != null)
        {
            return methodInfo;
        }
        else
        {
            throw new MsgNetException($"Could not find execution method \"{methodName}\" from \"{instance.GetType().FullName}\"");
        }
    }
}