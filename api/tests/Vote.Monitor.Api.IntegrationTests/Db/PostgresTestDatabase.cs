using System.Data.Common;
using Authorization.Policies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Respawn;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Api.IntegrationTests.Db;

public class PostgresTestDatabase : ITestDatabase
{
    private readonly string _connectionString = null!;
    private NpgsqlConnection _connection = null!;
    private Respawner _respawner = null!;

    public PostgresTestDatabase()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _connectionString = connectionString;
    }

    public async Task InitialiseAsync()
    {
        _connection = new NpgsqlConnection(_connectionString);
        _connection.Open();

        var options = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseNpgsql(_connectionString)
            .Options;

        var context = new VoteMonitorContext(options, new SerializerService(NullLogger<SerializerService>.Instance),
            new CurrentUtcTimeProvider(), new CurrentUserProvider());

        context.Database.Migrate();

        _respawner = await Respawner.CreateAsync(_connection,
            new RespawnerOptions
            {
                TablesToIgnore =
                    ["__EFMigrationsHistory", "AspNetRoles", "Language", "Countries"],
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
    }
}
