namespace Vote.Monitor.Api.IntegrationTests;

[TestFixture]
public abstract class BaseDbTestFixture
{
    [SetUp]
    public async Task BaseTestSetUp()
    {
        await DbTesting.ResetState();
    }
}
