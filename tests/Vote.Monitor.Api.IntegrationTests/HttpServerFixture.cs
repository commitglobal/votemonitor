using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Api.Feature.Auth.Login;
using Vote.Monitor.Api.Feature.ElectionRound;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using ElectionRoundCreateEndpoint = Vote.Monitor.Api.Feature.ElectionRound.Create.Endpoint;
using ElectionRoundCreateRequest = Vote.Monitor.Api.Feature.ElectionRound.Create.Request;

namespace Vote.Monitor.Api.IntegrationTests;

public class HttpServerFixture<TDataSeeder> : WebApplicationFactory<Program>, IAsyncLifetime, ITestOutputHelperAccessor where TDataSeeder : class, IDataSeeder
{
    private static readonly Faker _faker = new();
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithDatabase(Guid.NewGuid().ToString())
        .WithCleanUp(true)
        .Build();

    public ITestOutputHelper? OutputHelper { get; set; }

    /// <summary>
    /// bogus data generator
    /// </summary>
    public Faker Fake => _faker;

    /// <summary>
    /// the default http client
    /// </summary>
    public HttpClient Client { get; private set; }

    /// <summary>
    /// The platform admin http client
    /// </summary>
    public HttpClient PlatformAdmin { get; private set; }
    public ElectionRoundBaseModel ElectionRound { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            var descriptorType = typeof(DbContextOptions<VoteMonitorContext>);
            var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<VoteMonitorContext>(options => options.UseNpgsql(_postgresContainer.GetConnectionString()));
            services.AddScoped<IDataSeeder, TDataSeeder>();
        });

        builder.ConfigureLogging(l => l.ClearProviders().AddXUnit(this).SetMinimumLevel(LogLevel.Debug));
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var email = Fake.Internet.Email();
        var password = Fake.Internet.Password();

        using var voteMonitorContext = Services.GetRequiredService<VoteMonitorContext>();
        voteMonitorContext.PlatformAdmins.Add(new PlatformAdmin("Integration test platform admin", email, password, CurrentUtcTimeProvider.Instance));
        await voteMonitorContext.SaveChangesAsync();

        Client = CreateClient();

        var (_, tokenResponse) = await Client.POSTAsync<Endpoint, Request, Response>(new()
        {
            Username = email,
            Password = password
        });

        PlatformAdmin = CreateClient();
        PlatformAdmin.DefaultRequestHeaders.Authorization = new("Bearer", tokenResponse.Token);
      
        (_, ElectionRound) = await PlatformAdmin.POSTAsync<ElectionRoundCreateEndpoint, ElectionRoundCreateRequest, ElectionRoundBaseModel>(new()
        {
            CountryId = CountriesList.RO.Id,
            Title = Guid.NewGuid().ToString(),
            EnglishTitle = Guid.NewGuid().ToString(),
            StartDate = Fake.Date.FutureDateOnly()
        });


        var seeder = Services.GetRequiredService<IDataSeeder>();
        await seeder.SeedDataAsync();

    }

    public new async Task DisposeAsync() => await _postgresContainer.DisposeAsync().AsTask();
}
