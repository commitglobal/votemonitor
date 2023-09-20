using Greet = Vote.Monitor.Feature.Example.Greet;

namespace Tests.GreetTests;

public class GreetShould : TestClass<Fixture>
{
    public GreetShould(Fixture f, ITestOutputHelper o) : base(f, o) { }

    [Fact, Priority(1)]
    public async Task Greet_when_name_is_valid()
    {
        var request = Fixture.GreetRequest;

        var (rsp, response) = await Fixture.Client.POSTAsync<Greet.Endpoint, Greet.Request, Greet.Response>(request);

        rsp!.IsSuccessStatusCode.Should().BeTrue();
        response!.Greeting.Should().NotBeNullOrEmpty();
    }
}
