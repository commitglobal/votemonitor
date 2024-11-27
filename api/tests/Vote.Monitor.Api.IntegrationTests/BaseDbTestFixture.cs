namespace Vote.Monitor.Api.IntegrationTests;

using static ApiTesting;

[TestFixture]
public abstract class BaseDbTestFixture
{
    [SetUp]
    public async Task BaseTestSetUp()
    {
        await ResetState();
    }
}
