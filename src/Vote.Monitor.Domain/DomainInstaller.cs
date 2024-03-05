using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.Domain;

public static class DomainInstaller
{
    public const string SectionKey = "Domain";

    public static IServiceCollection AddApplicationDomain(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = GetConnectionString(config);
        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();

        services.AddDbContext<VoteMonitorContext>(options =>
            options.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null
                );
            }));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        return services;
    }

    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoteMonitorContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    private static string GetConnectionString(IConfiguration config)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = config["DbConnectionConfig:Server"]!,
            Port = int.Parse(config["DbConnectionConfig:Port"]!),
            Database = config["DbConnectionConfig:Database"]!,
            Username = config["DbConnectionConfig:UserId"],
            Password = config["DbConnectionConfig:Password"],
            IncludeErrorDetail = true
        };
        return connectionStringBuilder.ToString();
    }
}
