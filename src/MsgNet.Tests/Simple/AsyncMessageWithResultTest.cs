using System.Threading;
using System.Threading.Tasks;

namespace MsgNet.Tests.Simple;

internal class AsyncMessageWithResultTest : UnitTestBase
{
    class TestMessage : IAsyncMessage<TestMessageResult>
    {
        public bool DummyPayload { get; set; }
    }

    class TestMessageReceiver : IAsyncMessageReceiver<TestMessage, TestMessageResult>
    {
        public async Task<TestMessageResult> HandleAsync(TestMessage message, CancellationToken cancellationToken)
        {
            await Task.Delay(200, cancellationToken);
            message.DummyPayload = true;

            return new TestMessageResult()
            {
                DummyResult = true
            };
        }
    }

    class TestMessageResult
    {
        public bool DummyResult { get; set; }
    }

    [Test]
    public async Task Test()
    {
        var message = new TestMessage();

        var result = await Client.SendAsync(message);

        Assert.IsTrue(message.DummyPayload == true && result.DummyResult == true);
    }
}