namespace Vote.Monitor.Api.IntegrationTests;

/// <summary>
/// Data seeder contract used in <see cref="HttpServerFixture{TDataSeeder}"/>
/// </summary>
public interface IDataSeeder
{
    Task SeedDataAsync();
}
