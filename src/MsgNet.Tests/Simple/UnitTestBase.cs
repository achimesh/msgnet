namespace MsgNet.Tests.Simple;

internal class UnitTestBase
{
    protected IMessenger Client;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection().AddMsgNet().BuildServiceProvider();
        Client = services.GetRequiredService<IMessenger>();
    }
}