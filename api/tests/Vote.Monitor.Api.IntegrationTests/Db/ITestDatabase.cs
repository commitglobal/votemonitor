using System.Data.Common;

namespace Vote.Monitor.Api.IntegrationTests.Db;

public interface ITestDatabase
{
    Task InitialiseAsync();

    DbConnection GetConnection();
    string GetConnectionString();
    Task ResetAsync();

    Task DisposeAsync();
}
