using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MsgNet.Abstractions;

namespace MsgNet;

public class MsgNetMessenger: IMessenger
{
    private static readonly string EXECUTION_METHOD_NAME = "Handle";

    private readonly IServiceProvider ServiceProvider;

    public MsgNetMessenger(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public void Send(IMessage value)
    {
        var messageType = value.GetType();
        var receiverBaseType = typeof(IMessageReceiver<>).MakeGenericType(messageType);

        var receiver = ServiceProvider.GetRequiredService(receiverBaseType);
        var methodInfo = GetExecutionMethod(receiver);

        methodInfo.Invoke(receiver, new[] { value });
    }

    public TResult Send<TResult>(IMessage<TResult> value)
    {
        var messageType = value.GetType();
        var receiverBaseType = typeof(IMessageReceiver<,>).MakeGenericType(messageType, typeof(TResult));

        var receiver = ServiceProvider.GetRequiredService(receiverBaseType);
        var methodInfo = GetExecutionMethod(receiver);

        var result = methodInfo.Invoke(receiver, new[] { value });

        if (result != null)
        {
            return (TResult)result;
        }
        else
        {
            throw new MsgNetException($"Execution method from \"{receiverBaseType.FullName}\" does not return any value");
        }
    }

    public Task SendAsync(IAsyncMessage value, CancellationToken cancellationToken = default)
    {
        var messageType = value.GetType();
        var receiverBaseType = typeof(IAsyncMessageReceiver<>).MakeGenericType(messageType);

        var receiver = ServiceProvider.GetRequiredService(receiverBaseType);
        var methodInfo = GetExecutionMethod(receiver);

        var result = methodInfo.Invoke(receiver, new object[] { value, cancellationToken });

        if (result != null)
        {
            return (Task)result;
        }
        else
        {
            throw new MsgNetException($"Execution method from \"{receiverBaseType.FullName}\" does not return any value");
        }
    }

    public Task<TResult> SendAsync<TResult>(IAsyncMessage<TResult> value, CancellationToken cancellationToken = default)
    {
        var messageType = value.GetType();
        var receiverBaseType = typeof(IAsyncMessageReceiver<,>).MakeGenericType(messageType, typeof(TResult));

        var receiver = ServiceProvider.GetRequiredService(receiverBaseType);
        var methodInfo = GetExecutionMethod(receiver);

        var result = methodInfo.Invoke(receiver, new object[] { value, cancellationToken });

        if (result != null)
        {
            return (Task<TResult>)result;
        }
        else
        {
            throw new MsgNetException($"Execution method from \"{receiverBaseType.FullName}\" does not return any value");
        }
    }

    private static MethodInfo GetExecutionMethod(object instance)
    {
        var methodInfo = instance.GetType().GetMethod(EXECUTION_METHOD_NAME);

        if (methodInfo != null)
        {
            return methodInfo;
        }
        else
        {
            throw new MsgNetException($"Could not find execution method \"{EXECUTION_METHOD_NAME}\" from \"{instance.GetType().FullName}\"");
        }
    }
}