namespace MsgNet.Tests.Simple;

internal class SyncMessageTest: UnitTestBase
{
    class TestMessage : IMessage
    {
        public bool DummyPayload { get; set; }
    }

    class TestMessageReceiver : IMessageReceiver<TestMessage>
    {
        public void Handle(TestMessage message)
        {
            message.DummyPayload = true;
        }
    }

    [Test]
    public void Test()
    {
        var message = new TestMessage();

        Client.Send(message);

        Assert.IsTrue(message.DummyPayload == true);
    }
}