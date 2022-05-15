namespace MsgNet.Tests.Simple;

internal class SyncMessageWithResultTest: UnitTestBase
{
    class TestMessage : IMessage<TestMessageResult>
    {
        public bool DummyPayload { get; set; }
    }

    class TestMessageReceiver : IMessageReceiver<TestMessage, TestMessageResult>
    {
        public TestMessageResult Handle(TestMessage message)
        {
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
    public void Test()
    {
        var message = new TestMessage();

        var result = Client.Send(message);

        Assert.IsTrue(message.DummyPayload == true && result.DummyResult == true);
    }
}