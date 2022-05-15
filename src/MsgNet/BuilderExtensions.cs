using MsgNet;
using MsgNet.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class BuilderExtensions
{
    public static void UseDefaultMessenger(this IMessageBuilder builder)
    {
        var services = builder.GetServiceDescriptors();

        services.AddSingleton<IMessenger, DefaultMessenger>();
    }
}