using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Vote.Monitor.TestUtils;

public class TestContext : VoteMonitorContext
{
    public TestContext(DbContextOptions<VoteMonitorContext> options,
        ISerializerService serializerService,
        ITimeProvider timeProvider,
        ICurrentUserIdProvider currentUserIdProvider)
        : base(options, serializerService, timeProvider, currentUserIdProvider)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //needed because the in memory ef core provider cannot map type JsonDocument
        builder.Entity<PollingStation>().Ignore(t => t.Tags);
    }

    public static TestContext Fake()
    {
        var timeProvider = Substitute.For<ITimeProvider>();
        var serializationService = Substitute.For<ISerializerService>();
        var currentUserIdProvider = Substitute.For<ICurrentUserIdProvider>();

        var dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

        var context = new TestContext(dbContextOptions.Options,
            serializationService,
            timeProvider,
            currentUserIdProvider);

        return context;
    }
}
