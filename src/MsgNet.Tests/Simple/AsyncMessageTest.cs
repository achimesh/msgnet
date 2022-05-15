using System.Threading;
using System.Threading.Tasks;

namespace MsgNet.Tests.Simple;

internal class AsyncMessageTest: UnitTestBase
{
    class TestMessage : IAsyncMessage
    {
        public bool DummyPayload { get; set; }
    }

    class TestMessageReceiver : IAsyncMessageReceiver<TestMessage>
    {
        public async Task HandleAsync(TestMessage message, CancellationToken cancellationToken = default)
        {
            await Task.Delay(200, cancellationToken);
            message.DummyPayload = true;
        }
    }

    [Test]
    public async Task Test()
    {
        var message = new TestMessage();

        await Client.SendAsync(message);

        Assert.IsTrue(message.DummyPayload == true);
    }
}