using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Vote.Monitor.Api.Extensions;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using ZiggyCreatures.Caching.Fusion;

namespace Vote.Monitor.Api.IntegrationTests.Interceptors;

using static ApiTesting;

public class CacheVersionInterceptorTests : BaseDbTestFixture
{
    private FusionCache _cache;
    private VoteMonitorContext _context;

    [SetUp]
    public void Init()
    {
        _cache = new FusionCache(new FusionCacheOptions());

        var options = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseNpgsql(DbConnectionString)
            .AddInterceptors(new CacheVersionInterceptor(new FusionCacheTagInvalidator(_cache),
                NullLogger<CacheVersionInterceptor>.Instance))
            .Options;

        _context = new VoteMonitorContext(options);
    }

    [TearDown]
    public void Cleanup()
    {
        _context.Dispose();
        _cache.Dispose();
    }

    [Test]
    public async Task UpdateLocationsVersion_RemovesEntriesOfPreviousVersion()
    {
        // Arrange
        var electionRound = await AddElectionRoundAsync();
        var previousVersion = electionRound.LocationsVersion;

        await _cache.SetAsync("locations-old", "old",
            tags: [CacheTags.LocationsVersion(electionRound.Id, previousVersion)]);
        await _cache.SetAsync("polling-stations", "kept",
            tags: [CacheTags.PollingStationsVersion(electionRound.Id, electionRound.PollingStationsVersion)]);

        // Act
        electionRound.UpdateLocationsVersion();
        await _context.SaveChangesAsync();

        // Assert
        electionRound.LocationsVersion.Should().NotBe(previousVersion);
        (await _cache.TryGetAsync<string>("locations-old")).HasValue.Should().BeFalse();
        (await _cache.TryGetAsync<string>("polling-stations")).Value.Should().Be("kept");
    }

    [Test]
    public void UpdatePollingStationsVersion_RemovesEntriesOfPreviousVersion_WhenSavedSynchronously()
    {
        // Arrange
        var electionRound = AddElectionRoundAsync().GetAwaiter().GetResult();
        var previousVersion = electionRound.PollingStationsVersion;

        _cache.Set("polling-stations-old", "old",
            tags: [CacheTags.PollingStationsVersion(electionRound.Id, previousVersion)]);
        _cache.Set("locations", "kept",
            tags: [CacheTags.LocationsVersion(electionRound.Id, electionRound.LocationsVersion)]);

        // Act
        electionRound.UpdatePollingStationsVersion();
        _context.SaveChanges();

        // Assert
        _cache.TryGet<string>("polling-stations-old").HasValue.Should().BeFalse();
        _cache.TryGet<string>("locations").Value.Should().Be("kept");
    }

    [Test]
    public async Task UpdateFormVersion_RemovesEntriesOfPreviousVersion()
    {
        // Arrange
        var electionRound = await AddElectionRoundAsync();
        var monitoringNgo = new MonitoringNgoAggregateFaker(electionRound: electionRound).Generate();
        _context.MonitoringNgos.Add(monitoringNgo);
        await _context.SaveChangesAsync();
        var previousVersion = monitoringNgo.FormsVersion;

        await _cache.SetAsync("forms-old", "old", tags: [CacheTags.FormsVersion(monitoringNgo.Id, previousVersion)]);

        // Act
        monitoringNgo.UpdateFormVersion();
        await _context.SaveChangesAsync();

        // Assert
        (await _cache.TryGetAsync<string>("forms-old")).HasValue.Should().BeFalse();
    }

    [Test]
    public async Task SaveChanges_KeepsEntries_WhenNoVersionChanged()
    {
        // Arrange
        var electionRound = await AddElectionRoundAsync();
        await _cache.SetAsync("locations", "kept",
            tags: [CacheTags.LocationsVersion(electionRound.Id, electionRound.LocationsVersion)]);

        // Act
        _context.Entry(electionRound).Property(x => x.Title).CurrentValue = "renamed";
        await _context.SaveChangesAsync();

        // Assert
        (await _cache.TryGetAsync<string>("locations")).Value.Should().Be("kept");
    }

    private async Task<ElectionRound> AddElectionRoundAsync()
    {
        var electionRound = new ElectionRoundAggregateFaker(status: ElectionRoundStatus.NotStarted).Generate();
        _context.ElectionRounds.Add(electionRound);
        // countries are seeded
        _context.Entry(electionRound.Country).State = EntityState.Unchanged;
        await _context.SaveChangesAsync();

        return electionRound;
    }
}
