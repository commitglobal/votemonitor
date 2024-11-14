using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Vote.Monitor.Api.IntegrationTests.Services;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Api.IntegrationTests.Db;

public class TestcontainersTestDatabase : ITestDatabase
{
    private readonly PostgreSqlContainer _container;
    private DbConnection _connection = null!;
    private string _connectionString = null!;
    private Respawner _respawner = null!;

    public TestcontainersTestDatabase()
    {
        _container = new PostgreSqlBuilder()
            .WithAutoRemove(true)
            .Build();
    }

    public async Task InitialiseAsync()
    {
        await _container.StartAsync();

        _connectionString = _container.GetConnectionString();
        _connection = new NpgsqlConnection(_connectionString);
        _connection.Open();

        var options = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseNpgsql(_connectionString)
            .Options;

        var context = new VoteMonitorContext(options);

        context.Database.Migrate();

        _respawner = await Respawner.CreateAsync(_connection,
            new RespawnerOptions
            {
                TablesToIgnore =
                [
                    "__EFMigrationsHistory",
                    "AspNetRoles",
                    "AspNetUsers",
                    "Language",
                    "Countries"
                ],
                DbAdapter = DbAdapter.Postgres
            });
    }

    public DbConnection GetConnection()
    {
        return _connection;
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(_connection);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _container.DisposeAsync();
    }
}
