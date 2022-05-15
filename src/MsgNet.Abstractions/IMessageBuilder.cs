using Microsoft.Extensions.DependencyInjection;

namespace MsgNet.Abstractions;

public interface IMessageBuilder
{
    ServiceLifetime DefaultMessageReceiverLifetime { get; set; }

    IServiceCollection GetServiceDescriptors();
}