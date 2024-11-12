using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Serilog;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection;
    private readonly NpgsqlConnectionStringBuilder _connectionDetails;

    public const string AdminEmail = "admin@example.com";
    public const string AdminPassword = "toTallyNotTestPassw0rd";

    public CustomWebApplicationFactory(string connectionString, DbConnection connection)
    {
        _connection = connection;
        _connectionDetails =  new NpgsqlConnectionStringBuilder() { ConnectionString = connectionString };
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
            services
                .RemoveAll<DbContextOptions<VoteMonitorContext>>()
                .AddDbContext<VoteMonitorContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    options.UseNpgsql(_connection);
                });
            
            services.AddLogging(logging =>
            {
                Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

                var loggerConfiguration = new LoggerConfiguration()
                    .WriteTo.NUnitOutput()
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentUserName()
                    .Destructure.ToMaximumDepth(3);

                var logger = Log.Logger = loggerConfiguration.CreateLogger();

                logging.AddSerilog(logger);
            });
        });
    }
}
