namespace Vote.Monitor.Api.IntegrationTests;
/// <summary>
/// No operation data seeder.
/// </summary>
public class NoopDataSeeder : IDataSeeder
{
    public Task SeedDataAsync()
    {
        return Task.CompletedTask;
    }
}
