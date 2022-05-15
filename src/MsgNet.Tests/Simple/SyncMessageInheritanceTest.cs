namespace MsgNet.Tests.Simple;

internal class SyncMessageInheritanceTest: UnitTestBase
{
    abstract class AbstractMessage : IMessage
    {
        public bool AbstractDummyPayload { get; set; }
    }

    class ExtendedMessage : AbstractMessage
    {
        public bool DummyPayload { get; set; }
    }

    abstract class AbstractReceiver<TMessage> : IMessageReceiver<TMessage> where TMessage : AbstractMessage
    {
        public abstract void Handle(TMessage message);
    }

    class ExtendedMessageReceiver : AbstractReceiver<ExtendedMessage>
    {
        public override void Handle(ExtendedMessage message)
        {
            message.AbstractDummyPayload = true;
            message.DummyPayload = true;
        }
    }

    [Test]
    public void Test()
    {
        var message = new ExtendedMessage();

        Client.Send(message);

        Assert.IsTrue(message.AbstractDummyPayload == true && message.DummyPayload == true);
    }
}