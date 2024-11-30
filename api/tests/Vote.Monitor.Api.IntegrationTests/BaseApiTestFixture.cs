namespace Vote.Monitor.Api.IntegrationTests;

using static ApiTesting;

[TestFixture]
public abstract class BaseApiTestFixture
{
    [SetUp]
    public async Task BaseTestSetUp()
    {
        await ResetState();
    }
}
