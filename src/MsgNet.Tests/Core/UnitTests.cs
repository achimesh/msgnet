using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MsgNet.Tests.Core;

[SetUpFixture]
internal class CoreTestSetup
{
    public static IServiceProvider ServiceProvider { get; private set; }

    [OneTimeSetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddMsgNet(new[] { Assembly.GetExecutingAssembly() }, options =>
        {

        });

        ServiceProvider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void Teardown()
    {

    }
}

[TestFixture]
internal class TestSyncMessageNoResult
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
        var client = CoreTestSetup.ServiceProvider.GetService<IMessenger>();
        var message = new TestMessage();

        client.Send(message);

        Assert.IsTrue(message.DummyPayload == true);
    }

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
    public void TestInheritance()
    {
        var client = CoreTestSetup.ServiceProvider.GetService<IMessenger>();
        var message = new ExtendedMessage();

        client.Send(message);

        Assert.IsTrue(message.AbstractDummyPayload == true && message.DummyPayload == true);
    }
}

[TestFixture]
internal class TestAsyncMessageNoResult
{
    class TestMessage : IAsyncMessage
    {
        public bool DummyPayload { get; set; }
    }

    class TestMessageReceiver : IAsyncMessageReceiver<TestMessage>
    {
        public async Task Handle(TestMessage message, CancellationToken cancellationToken = default)
        {
            await Task.Delay(200, cancellationToken);
            message.DummyPayload = true;
        }
    }

    [Test]
    public async Task Test()
    {
        var client = CoreTestSetup.ServiceProvider.GetService<IMessenger>();
        var message = new TestMessage();

        await client.SendAsync(message);

        Assert.IsTrue(message.DummyPayload == true);
    }

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
        public abstract Task Handle(TMessage message, CancellationToken cancellationToken = default);
    }

    class ExtendedMessageReceiver : AbstractReceiver<ExtendedMessage>
    {
        public override async Task Handle(ExtendedMessage message, CancellationToken cancellationToken = default)
        {
            await Task.Delay(200, cancellationToken);
            message.AbstractDummyPayload = true;
            message.DummyPayload = true;
        }
    }

    [Test]
    public async Task TestInheritance()
    {
        var client = CoreTestSetup.ServiceProvider.GetService<IMessenger>();

        var message = new ExtendedMessage();
        await client.SendAsync(message);

        Assert.IsTrue(message.AbstractDummyPayload == true && message.DummyPayload == true);
    }
}