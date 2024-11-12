using Vote.Monitor.Api.IntegrationTests.Db;

namespace Vote.Monitor.Api.IntegrationTests;

[SetUpFixture]
public class DbTesting
{
    private static ITestDatabase _database = null!;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _database = await TestDatabaseFactory.CreateAsync();
        await _database.InitialiseAsync();
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

    public static string DbConnectionString => _database.GetConnectionString();
    
    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await _database.DisposeAsync();
    }
}
