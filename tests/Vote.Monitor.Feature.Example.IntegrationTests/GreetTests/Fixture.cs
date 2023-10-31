using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Vote.Monitor.Api;
using Vote.Monitor.Domain;
using Xunit.Abstractions;

namespace Vote.Monitor.Feature.PollingStation.IntegrationTests.GreetTests;

public class Fixture : TestFixture<Program>
{
    private static readonly string _databaseName = Guid.NewGuid().ToString();

    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
      .WithDatabase(_databaseName)
      .Build();

    public Fixture(IMessageSink s) : base(s) { }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        _postgresContainer.StartAsync().GetAwaiter().GetResult();
        var connectionString = _postgresContainer.GetConnectionString();

        builder.ConfigureTestServices(services =>
        {
            var descriptorType = typeof(DbContextOptions<VoteMonitorContext>);
            var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<VoteMonitorContext>(options => options.UseNpgsql(connectionString));
        });
    }

    protected override async Task TearDownAsync()
    {
        await _postgresContainer.DisposeAsync().AsTask();
    }
}
