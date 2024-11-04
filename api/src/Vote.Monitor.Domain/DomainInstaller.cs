using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.Domain.Seeders;


namespace Vote.Monitor.Domain;

public static class DomainInstaller
{
    public const string SectionKey = "Domain";

    public static IServiceCollection AddApplicationDomain(this IServiceCollection services, IConfiguration config, bool isProductionEnvironment)
    {
        var connectionString = config.GetNpgsqlConnectionString("DbConnectionConfig");

        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();

        services.AddDbContext<VoteMonitorContext>(options =>
        {
            options.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null
                ).UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            options.EnableSensitiveDataLogging(!isProductionEnvironment);
            
        });
        services.AddSingleton<INpgsqlConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        services.AddHealthChecks().AddNpgSql(name: "domain-db", connectionString: connectionString);

        return services;
    }

    public static IServiceCollection AddSeeders(this IServiceCollection services)
    {
        services.AddScoped<IAmDbSeeder, PlatformAdminSeeder>();

        return services;
    }

    public static IServiceCollection AddIdentity(this IServiceCollection services) =>
        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<VoteMonitorContext>()
            .AddDefaultTokenProviders()
            .Services;

    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoteMonitorContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
        var seeders = scope.ServiceProvider.GetServices<IAmDbSeeder>();

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync();
        }
    }
}
