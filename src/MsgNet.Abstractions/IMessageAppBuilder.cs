using Microsoft.Extensions.DependencyInjection;

namespace MsgNet.Abstractions;

public interface IMessageAppBuilder
{
    ServiceLifetime DefaultMessageReceiverLifetime { get; set; }

    IServiceCollection GetServiceDescriptors();
}