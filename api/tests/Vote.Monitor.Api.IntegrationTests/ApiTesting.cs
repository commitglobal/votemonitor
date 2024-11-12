using Vote.Monitor.Api.IntegrationTests.Db;

namespace Vote.Monitor.Api.IntegrationTests;

[SetUpFixture]
public class ApiTesting
{
    private static ITestDatabase _database = null!;
    private static CustomWebApplicationFactory _factory = null!;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _database = await TestDatabaseFactory.CreateAsync();
        await _database.InitialiseAsync();
        _factory = new CustomWebApplicationFactory(_database.GetConnectionString(), _database.GetConnection()); 
    }

    public static async Task ResetState()
    {
        try
        {
            await _database.ResetAsync();
        }
        catch (Exception)
        {
        }
    }

    public static HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await _database.DisposeAsync();
        await _factory.DisposeAsync();
    }
}
