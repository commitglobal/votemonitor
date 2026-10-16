using System.Runtime.Serialization;
using System.Text.Json;
using Vote.Monitor.Core.Services.Caching;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Vote.Monitor.Api.Extensions;

public enum CacheType
{
    [EnumMember(Value = "Memory")]
    Memory,

    [EnumMember(Value = "Redis")]
    Redis
}

public static class CacheExtensions
{
    public const string SectionKey = "Cache";

    /// <summary>
    /// Registers FusionCache. With CacheType "Memory" entries live only in the memory of each instance.
    /// With CacheType "Redis" Redis is added as a distributed (L2) cache and as a backplane,
    /// so all instances share entries and get notified when one of them changes.
    /// </summary>
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheTagInvalidator, FusionCacheTagInvalidator>();

        var fusionCache = services.AddFusionCache()
            .WithDefaultEntryOptions(options =>
            {
                options.Duration = TimeSpan.FromMinutes(30);
                // Soft refresh near expiry + fail-safe reduce stampedes and keep serving data under load.
                options.EagerRefreshThreshold = 0.8f;
                options.IsFailSafeEnabled = true;
                options.FailSafeMaxDuration = TimeSpan.FromHours(2);
                options.FailSafeThrottleDuration = TimeSpan.FromSeconds(30);
            });

        switch (configuration.GetValue<CacheType>("CacheType"))
        {
            case CacheType.Memory:
                return services;

            case CacheType.Redis:
                var connectionString = configuration.GetValue<string>("Redis:ConnectionString");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new ArgumentException("Cache:Redis:ConnectionString is required when CacheType is Redis",
                        nameof(configuration));
                }
                
                var muxer = ConnectionMultiplexer.Connect("localhost:6379");

                fusionCache
                    // IncludeFields lets FusionCache serialize the value tuples it uses internally.
                    .WithSerializer(new FusionCacheSystemTextJsonSerializer(new JsonSerializerOptions { IncludeFields = true }))
                    .WithDistributedCache(
                        new RedisCache(new RedisCacheOptions
                        {
                            ConnectionMultiplexerFactory = async () => muxer,
                        })
                    )
                    .WithBackplane(
                            new RedisBackplane(new RedisBackplaneOptions
                            {
                                ConnectionMultiplexerFactory = async () => muxer,
                            })
                    );
                return services;

            default:
                throw new ArgumentException("Unknown configuration for CacheType", nameof(configuration));
        }
    }
}

public sealed class FusionCacheTagInvalidator(IFusionCache cache) : ICacheTagInvalidator
{
    public async Task RemoveByTagsAsync(IReadOnlyCollection<string> tags, CancellationToken ct = default)
    {
        foreach (var tag in tags)
        {
            await cache.RemoveByTagAsync(tag, token: ct);
        }
    }

    public void RemoveByTags(IReadOnlyCollection<string> tags)
    {
        foreach (var tag in tags)
        {
            cache.RemoveByTag(tag);
        }
    }
}
