using Vote.Monitor.Api;

namespace Vote.Monitor.Feature.Example.IntegrationTests.GreetTests;

public class Fixture : TestFixture<Program>
{
    public Fixture(IMessageSink s) : base(s) { }

    internal Greet.Request GreetRequest { get; private set; } = default!;

    protected override Task SetupAsync()
    {
        GreetRequest = Fake.GreetRequest();
        return Task.CompletedTask;
    }
}
