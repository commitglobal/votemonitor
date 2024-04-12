using System.Security.Claims;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Api.Feature.Auth.Login;
using Vote.Monitor.Api.Feature.Auth.Services;
using Vote.Monitor.Api.Feature.ElectionRound;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Core.Services.Security;
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
    public ElectionRoundModel ElectionRound { get; private set; }

    private readonly ClaimsPrincipal _integrationTestingUser = new([new ClaimsIdentity([new Claim(ApplicationClaimTypes.UserId, "007e57ed-7e57-7e57-7e57-007e57ed0000")], "fake")]);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            var descriptorType = typeof(DbContextOptions<VoteMonitorContext>);
            RemoveService(services, descriptorType);

            services.AddDbContext<VoteMonitorContext>(options => options.UseNpgsql(_postgresContainer.GetConnectionString()));
            services.AddScoped<IDataSeeder, TDataSeeder>();
        });

        builder.ConfigureLogging(l => l.ClearProviders().AddXUnit(this).SetMinimumLevel(LogLevel.Debug));
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var firstName = Fake.Name.FirstName();
        var lastName = Fake.Name.FirstName();
        var phoneNumber = Fake.Phone.PhoneNumber();
        var email = Fake.Internet.Email();
        var password = Fake.Internet.Password();
        var currentUserInitializer = Services.GetRequiredService<ICurrentUserInitializer>();
        currentUserInitializer.SetCurrentUser(_integrationTestingUser);

        using var userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();

        var platformAdmin = ApplicationUser.Create(firstName, lastName, email, phoneNumber, password);
        platformAdmin.EmailConfirmed = true;
        await userManager.CreateAsync(platformAdmin);
        await userManager.AddToRoleAsync(platformAdmin, UserRole.PlatformAdmin);

        Client = CreateClient();

        var ( tokenResult, tokenResponse) = await Client.POSTAsync<Endpoint, Request, TokenResponse>(new()
        {
            Email = email,
            Password = password
        });
        
        tokenResult.IsSuccessStatusCode.Should().BeTrue();

        PlatformAdmin = CreateClient();
        PlatformAdmin.DefaultRequestHeaders.Authorization = new("Bearer", tokenResponse.Token);

        (var electionRoundResult, ElectionRound) = await PlatformAdmin.POSTAsync<ElectionRoundCreateEndpoint, ElectionRoundCreateRequest, ElectionRoundModel>(new()
        {
            CountryId = CountriesList.RO.Id,
            Title = Guid.NewGuid().ToString(),
            EnglishTitle = Guid.NewGuid().ToString(),
            StartDate = Fake.Date.FutureDateOnly()
        });

        electionRoundResult.IsSuccessStatusCode.Should().BeTrue();

        var seeder = Services.GetRequiredService<IDataSeeder>();
        await seeder.SeedDataAsync();
    }

    public new async Task DisposeAsync() => await _postgresContainer.DisposeAsync().AsTask();

    private static void RemoveService(IServiceCollection services, Type descriptorType)
    {
        var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);
        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }
}
