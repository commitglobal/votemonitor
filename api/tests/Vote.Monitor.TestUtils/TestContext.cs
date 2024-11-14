using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FeedbackAggregate;
using Vote.Monitor.Domain.Entities.LocationAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Vote.Monitor.TestUtils;

public class TestContext : VoteMonitorContext
{
    public TestContext(DbContextOptions<VoteMonitorContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //needed because the in memory ef core provider cannot map type JsonDocument
        builder.Entity<PollingStation>().Ignore(t => t.Tags);
        builder.Entity<Location>().Ignore(t => t.Tags);
        builder.Entity<Feedback>().Ignore(t => t.Metadata);
    }

    public static TestContext Fake()
    {
        var dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

        var context = new TestContext(dbContextOptions.Options);

        return context;
    }
}
