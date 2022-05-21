using Microsoft.Extensions.DependencyInjection;
using MsgNet.Abstractions;

namespace MsgNet;

internal class MessageAppBuilder: IMessageAppBuilder
{
    public ServiceLifetime DefaultMessageReceiverLifetime { get; set; }

    private readonly IServiceCollection _serviceDescriptors;

    public MessageAppBuilder(IServiceCollection serviceDescriptors)
    {
        _serviceDescriptors = serviceDescriptors;
    }

    public IServiceCollection GetServiceDescriptors() => _serviceDescriptors;
}