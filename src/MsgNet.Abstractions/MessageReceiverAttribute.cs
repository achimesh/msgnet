using Microsoft.Extensions.DependencyInjection;

namespace MsgNet.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MessageReceiverAttribute: Attribute
{
    public ServiceLifetime Lifetime { get; }

    public MessageReceiverAttribute(ServiceLifetime serviceLifetime)
    {
        Lifetime = serviceLifetime;

        if (serviceLifetime != ServiceLifetime.Transient &&
            serviceLifetime != ServiceLifetime.Scoped &&
            serviceLifetime != ServiceLifetime.Singleton)
        {
            throw new MsgNetException($"Unsupported service life time: ${serviceLifetime}");
        }
    }
}