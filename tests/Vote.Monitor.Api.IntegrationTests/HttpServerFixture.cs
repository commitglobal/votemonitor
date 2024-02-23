﻿using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Api.Feature.Auth.Login;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

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

        var seeder = Services.GetRequiredService<IDataSeeder>();
        await seeder.SeedDataAsync();

    }

    public new async Task DisposeAsync() => await _postgresContainer.DisposeAsync().AsTask();
}
