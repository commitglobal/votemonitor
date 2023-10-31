using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Vote.Monitor.Api;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Feature.Example.IntegrationTests.GreetTests;

public class Fixture : TestFixture<Program>
{
    private static readonly string _databaseName = Guid.NewGuid().ToString();

    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
      .WithDatabase(_databaseName)
      .Build();

    public Fixture(IMessageSink s) : base(s) { }

    internal Greet.Request GreetRequest { get; private set; } = default!;


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

    protected override Task SetupAsync()
    {
        GreetRequest = Fake.GreetRequest();
        return Task.CompletedTask;
    }

    protected override async Task TearDownAsync()
    {
        await _postgresContainer.DisposeAsync().AsTask();
    }
}
