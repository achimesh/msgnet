namespace MsgNet.Tests.ServiceLifetime;

internal class SyncMessageTest
{
    class TestMessage : IMessage
    {
        public int Total { get; set; }
    }

    class TestOperation<T> where T: TestMessage
    {
        private readonly IMessageReceiver<T> MessageReceiver;

        public TestOperation(IMessageReceiver<T> messageReceiver)
        {
            MessageReceiver = messageReceiver;
        }

        public void Calculate(T message)
        {
            MessageReceiver.Handle(message);
        }
    }

    class TestMessageTransient : TestMessage
    {

    }

    [MessageReceiver(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient)]
    class TestMessageTransientReceiver : IMessageReceiver<TestMessageTransient>
    {
        private int Sum = 0;

        public void Handle(TestMessageTransient message)
        {
            Sum += 1;
            message.Total = Sum;
        }
    }

    [Test]
    public void TestTransient()
    {
        var serviceProvider = new ServiceCollection()
            .AddMsgNet()
            .AddTransient<TestOperation<TestMessageTransient>>()
            .BuildServiceProvider();
        var message = new TestMessageTransient();

        var operation1 = serviceProvider.GetRequiredService<TestOperation<TestMessageTransient>>();
        operation1.Calculate(message);
        var operation2 = serviceProvider.GetRequiredService<TestOperation<TestMessageTransient>>();
        operation2.Calculate(message);

        Assert.IsTrue(message.Total == 1);
    }

    class TestMessageScoped : TestMessage
    {

    }

    [MessageReceiver(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped)]
    class TestMessageScopedReceiver : IMessageReceiver<TestMessageScoped>
    {
        private int Sum = 0;

        public void Handle(TestMessageScoped message)
        {
            Sum += 1;
            message.Total = Sum;
        }
    }

    [Test]
    public void TestScoped()
    {
        var serviceProvider = new ServiceCollection()
            .AddMsgNet()
            .AddTransient<TestOperation<TestMessageScoped>>()
            .BuildServiceProvider();
        var message = new TestMessageScoped();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            var operation1 = scope.ServiceProvider.GetRequiredService<TestOperation<TestMessageScoped>>();
            operation1.Calculate(message);
            var operation2 = scope.ServiceProvider.GetRequiredService<TestOperation<TestMessageScoped>>();
            operation2.Calculate(message);
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            var operation1 = scope.ServiceProvider.GetRequiredService<TestOperation<TestMessageScoped>>();
            operation1.Calculate(message);
            var operation2 = scope.ServiceProvider.GetRequiredService<TestOperation<TestMessageScoped>>();
            operation2.Calculate(message);
        }

        Assert.IsTrue(message.Total == 2);
    }

    class TestMessageSingleton : TestMessage
    {

    }

    [MessageReceiver(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    class TestMessageSingletonReceiver : IMessageReceiver<TestMessageSingleton>
    {
        private int Sum = 0;

        public void Handle(TestMessageSingleton message)
        {
            Sum += 1;
            message.Total = Sum;
        }
    }

    [Test]
    public void TestSingleton()
    {
        var serviceProvider = new ServiceCollection()
            .AddMsgNet()
            .AddTransient<TestOperation<TestMessageSingleton>>()
            .BuildServiceProvider();
        var message = new TestMessageSingleton();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            var operation1 = scope.ServiceProvider.GetRequiredService<TestOperation<TestMessageSingleton>>();
            operation1.Calculate(message);
            var operation2 = scope.ServiceProvider.GetRequiredService<TestOperation<TestMessageSingleton>>();
            operation2.Calculate(message);
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            var operation1 = scope.ServiceProvider.GetRequiredService<TestOperation<TestMessageSingleton>>();
            operation1.Calculate(message);
            var operation2 = scope.ServiceProvider.GetRequiredService<TestOperation<TestMessageSingleton>>();
            operation2.Calculate(message);
        }

        Assert.IsTrue(message.Total == 4);
    }
}