using System.Threading;
using System.Threading.Tasks;

namespace MsgNet.Tests.Simple;

internal class AsyncMessageInheritanceTest: UnitTestBase
{
    abstract class AbstractMessage : IAsyncMessage
    {
        public bool AbstractDummyPayload { get; set; }
    }

    class ExtendedMessage : AbstractMessage
    {
        public bool DummyPayload { get; set; }
    }

    abstract class AbstractReceiver<TMessage> : IAsyncMessageReceiver<TMessage> where TMessage : AbstractMessage
    {
        public abstract Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
    }

    class ExtendedMessageReceiver : AbstractReceiver<ExtendedMessage>
    {
        public override async Task HandleAsync(ExtendedMessage message, CancellationToken cancellationToken = default)
        {
            await Task.Delay(200, cancellationToken);
            message.AbstractDummyPayload = true;
            message.DummyPayload = true;
        }
    }

    [Test]
    public async Task Test()
    {
        var message = new ExtendedMessage();

        await Client.SendAsync(message);

        Assert.IsTrue(message.AbstractDummyPayload == true && message.DummyPayload == true);
    }
}