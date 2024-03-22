using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Vote.Monitor.Domain.UnitTests.Interceptors;

public class TestContext : VoteMonitorContext
{
    public TestContext(DbContextOptions<VoteMonitorContext> options, ISerializerService serializerService, ITimeProvider timeProvider, ICurrentUserProvider currentUserProvider)
        : base(options, serializerService, timeProvider, currentUserProvider)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //needed because the in memory ef core provider cannot map type JsonDocument
        builder.Entity<PollingStation>().Ignore(t => t.Tags);
    }
}
