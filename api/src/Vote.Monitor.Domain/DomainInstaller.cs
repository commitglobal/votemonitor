using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Npgsql;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Core.Services.Caching;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.Domain.Seeders;


namespace Vote.Monitor.Domain;

public static class DomainInstaller
{
    public const string SectionKey = "Domain";

    public static IServiceCollection AddApplicationDomain(this IServiceCollection services, IConfiguration config,
        bool isProductionEnvironment)
    {
        var connectionString = config.GetNpgsqlConnectionString("DbConnectionConfig");

        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();

        services.AddScoped<AuditingInterceptor>(sp =>
            new AuditingInterceptor(sp.GetRequiredService<ICurrentUserProvider>(),
                sp.GetRequiredService<ITimeProvider>()));

        services.AddScoped<AuditTrailInterceptor>(sp =>
            new AuditTrailInterceptor(sp.GetRequiredService<ISerializerService>(),
                sp.GetRequiredService<ICurrentUserProvider>(), sp.GetRequiredService<ITimeProvider>()));

        // The API registers a real cache invalidator; elsewhere (e.g. background jobs) there is no cache to clean up.
        services.TryAddSingleton<ICacheTagInvalidator, NoopCacheTagInvalidator>();
        services.AddScoped<CacheVersionInterceptor>(sp =>
            new CacheVersionInterceptor(sp.GetRequiredService<ICacheTagInvalidator>(),
                sp.GetRequiredService<ILogger<CacheVersionInterceptor>>()));

        services.AddDbContext<VoteMonitorContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null
                    )
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }).AddInterceptors(sp.GetRequiredService<AuditTrailInterceptor>(),
                sp.GetRequiredService<AuditingInterceptor>(),
                sp.GetRequiredService<CacheVersionInterceptor>());

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

    public static async Task InitializeDatabasesAsync(this IServiceProvider services,
        CancellationToken cancellationToken = default)
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
