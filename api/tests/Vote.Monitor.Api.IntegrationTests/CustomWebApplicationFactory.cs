using System.Data.Common;
using Authorization.Policies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Serilog;
using Vote.Monitor.Api.IntegrationTests.Services;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection;
    private readonly ITimeProvider _timeProvider;
    private readonly NpgsqlConnectionStringBuilder _connectionDetails;

    public const string AdminEmail = "integration@testing.com";
    public const string AdminPassword = "toTallyNotTestPassw0rd";

    public CustomWebApplicationFactory(string connectionString, DbConnection connection, ITimeProvider timeProvider)
    {
        _connection = connection;
        _connectionDetails =  new NpgsqlConnectionStringBuilder { ConnectionString = connectionString };
        _timeProvider = timeProvider;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("AuthFeatureConfig:JWTConfig:TokenSigningKey", "SecretKeyOfDoomThatMustBeAMinimumNumberOfBytes" );
        builder.UseSetting("AuthFeatureConfig:JWTConfig:TokenExpirationInMinutes", "10080" );
        builder.UseSetting("AuthFeatureConfig:JWTConfig:RefreshTokenExpirationInDays", "30");
        builder.UseSetting("Domain:DbConnectionConfig:Server", _connectionDetails.Host );
        builder.UseSetting("Domain:DbConnectionConfig:Port", _connectionDetails.Port.ToString() );
        builder.UseSetting("Domain:DbConnectionConfig:Database", _connectionDetails.Database );
        builder.UseSetting("Domain:DbConnectionConfig:UserId", _connectionDetails.Username );
        builder.UseSetting("Domain:DbConnectionConfig:Password", _connectionDetails.Password);
        builder.UseSetting("Core:EnableHangfire", "false" );
        builder.UseSetting("Core:HangfireConnectionConfig:Server", _connectionDetails.Host );
        builder.UseSetting("Core:HangfireConnectionConfig:Port", _connectionDetails.Port.ToString() );
        builder.UseSetting("Core:HangfireConnectionConfig:Database", _connectionDetails.Database);
        builder.UseSetting("Core:HangfireConnectionConfig:UserId", _connectionDetails.Username);
        builder.UseSetting("Core:HangfireConnectionConfig:Password", _connectionDetails.Password);
        builder.UseSetting("Sentry:Enabled", "false" );
        builder.UseSetting("Sentry:Dsn", "" );
        builder.UseSetting("Sentry:TracesSampleRate", "0.2" );
        builder.UseSetting("Seeders:PlatformAdminSeeder:FirstName", "John" );
        builder.UseSetting("Seeders:PlatformAdminSeeder:LastName", "Doe" );
        builder.UseSetting("Seeders:PlatformAdminSeeder:Email", AdminEmail);
        builder.UseSetting("Seeders:PlatformAdminSeeder:PhoneNumber", "1234567890" );
        builder.UseSetting("Seeders:PlatformAdminSeeder:Password", AdminPassword);
        
        builder.ConfigureTestServices((services) =>
        {
            services.RemoveAll<ITimeProvider>();
            services.AddSingleton<ITimeProvider>(_ => _timeProvider);
            
            services
                .RemoveAll<DbContextOptions<VoteMonitorContext>>()
                .AddDbContext<VoteMonitorContext>((sp, options) =>
                {
                    options.UseNpgsql(_connection)
                        .AddInterceptors(new AuditingInterceptor(new CurrentUserProvider(), _timeProvider));
                });

            services.AddLogging(logging =>
            {
                Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

                var loggerConfiguration = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentUserName()
                    .Destructure.ToMaximumDepth(3)
                    .WriteTo.NUnitOutput();

                var logger = Log.Logger = loggerConfiguration.CreateLogger();

                logging.AddSerilog(logger);
            });
        });
    }
}
