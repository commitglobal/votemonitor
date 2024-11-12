namespace Vote.Monitor.Api.IntegrationTests.Db;

public static class TestDatabaseFactory
{
    public static async Task<ITestDatabase> CreateAsync()
    {
        var database = new TestcontainersTestDatabase();
        // var database = new PostgresTestDatabase();

        await database.InitialiseAsync();

        return database;
    }
}
