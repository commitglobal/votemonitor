using NSubstitute;
using Vote.Monitor.Api.IntegrationTests.Db;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.IntegrationTests;

[SetUpFixture]
public class ApiTesting
{
    private static ITestDatabase _database = null!;
    private static CustomWebApplicationFactory _factory = null!;
    private static ITimeProvider _apiTimeProvider = null!;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _database = await TestDatabaseFactory.CreateAsync();
        _apiTimeProvider = Substitute.For<ITimeProvider>();
        _apiTimeProvider.UtcNow.Returns(_ => DateTime.UtcNow);
        _apiTimeProvider.UtcNowDate.Returns(_ => DateOnly.FromDateTime(DateTime.UtcNow));
        
        await _database.InitialiseAsync();
        _factory = new CustomWebApplicationFactory(_database.GetConnectionString(), _database.GetConnection(), _apiTimeProvider); 
    }
    
    public static string DbConnectionString => _database.GetConnectionString();

    public static async Task ResetState()
    {
        try
        {
            await _database.ResetAsync();
        }
        catch (Exception e)
        {
            TestContext.Out.WriteLine(e.Message);
        }
    }

    public static ITimeProvider ApiTimeProvider => _apiTimeProvider;

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
