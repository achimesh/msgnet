using Microsoft.Extensions.DependencyInjection;
using MsgNet.Abstractions;

namespace MsgNet.Extensions.DependencyInjection;

internal class MessageBuilder: IMessageBuilder
{
    public ServiceLifetime DefaultMessageReceiverLifetime { get; set; }

    private readonly IServiceCollection _serviceDescriptors;

    public MessageBuilder(IServiceCollection serviceDescriptors)
    {
        _serviceDescriptors = serviceDescriptors;
    }

    public IServiceCollection GetServiceDescriptors() => _serviceDescriptors;
}