using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.UnitTests;

public class AuditingInterceptorTests
{
    [Fact]
    public async Task Interceptor_OnEntityAdd_SaveChangesAsync_SetsCreatedFields()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);

        var dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase("TestInMemory");

        var fakeTimeProvider = Substitute.For<ITimeProvider>();
        var fakeSerializationService = Substitute.For<ISerializerService>();
        var fakeCurrentUserProvider = Substitute.For<ICurrentUserProvider>();

        fakeTimeProvider.UtcNow.Returns(createdOn);
        fakeCurrentUserProvider.GetUserId().Returns(userId);

        var context = new TestContext(dbContextOptions.Options,
            fakeSerializationService,
            fakeTimeProvider,
            fakeCurrentUserProvider);

        //Act 
        var testEntity = new Ngo(string.Empty,
            fakeTimeProvider);

        await context.Ngos.AddAsync(testEntity);

        await context.SaveChangesAsync(CancellationToken.None);

        //Assert
        testEntity.CreatedOn.Should().Be(createdOn);
        testEntity.CreatedBy.Should().Be(userId);

        testEntity.LastModifiedOn.Should().BeNull();
        testEntity.LastModifiedBy.Should().Be(userId);
    }

    [Fact]
    public async Task Interceptor_OnEntityAdd_SaveChanges_SetsCreatedFields()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);

        var dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase("TestInMemory");

        var fakeTimeProvider = Substitute.For<ITimeProvider>();
        var fakeSerializationService = Substitute.For<ISerializerService>();
        var fakeCurrentUserProvider = Substitute.For<ICurrentUserProvider>();

        fakeTimeProvider.UtcNow.Returns(createdOn);
        fakeCurrentUserProvider.GetUserId().Returns(userId);

        var context = new TestContext(dbContextOptions.Options,
            fakeSerializationService,
            fakeTimeProvider,
            fakeCurrentUserProvider);

        //Act 
        var testEntity = new Ngo(string.Empty,
            fakeTimeProvider);

        await context.Ngos.AddAsync(testEntity);

        context.SaveChanges();

        //Assert
        testEntity.CreatedOn.Should().Be(createdOn);
        testEntity.CreatedBy.Should().Be(userId);

        testEntity.LastModifiedOn.Should().BeNull();
        testEntity.LastModifiedBy.Should().Be(userId);
    }

    [Fact]
    public async Task Interceptor_OnEntityUpdate_SaveChangesAsync_SetsLastModifiedFields()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);
        var lastModifiedOn = new DateTime(2024, 03, 24, 0, 0, 0);

        var dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase("TestInMemory");

        var fakeTimeProvider = Substitute.For<ITimeProvider>();
        var fakeSerializationService = Substitute.For<ISerializerService>();
        var fakeCurrentUserProvider = Substitute.For<ICurrentUserProvider>();

        fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        fakeCurrentUserProvider.GetUserId().Returns(userId, anotherUserId);

        var context = new TestContext(dbContextOptions.Options,
            fakeSerializationService,
            fakeTimeProvider,
            fakeCurrentUserProvider);

        var testEntity = new Ngo(string.Empty,
            fakeTimeProvider);

        await context.Ngos.AddAsync(testEntity);
        await context.SaveChangesAsync(CancellationToken.None);

        //Act 
        testEntity.UpdateDetails("new name");

        await context.SaveChangesAsync(CancellationToken.None);

        //Assert
        testEntity.LastModifiedOn.Should().Be(lastModifiedOn);
        testEntity.LastModifiedBy.Should().Be(anotherUserId);
    }

    [Fact]
    public async Task Interceptor_OnEntityUpdate_SaveChanges_SetsLastModifiedFields()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);
        var lastModifiedOn = new DateTime(2024, 03, 24, 0, 0, 0);

        var dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase("TestInMemory");

        var fakeTimeProvider = Substitute.For<ITimeProvider>();
        var fakeSerializationService = Substitute.For<ISerializerService>();
        var fakeCurrentUserProvider = Substitute.For<ICurrentUserProvider>();

        fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        fakeCurrentUserProvider.GetUserId().Returns(userId, anotherUserId);

        var context = new TestContext(dbContextOptions.Options,
            fakeSerializationService,
            fakeTimeProvider,
            fakeCurrentUserProvider);

        var testEntity = new Ngo(string.Empty,
            fakeTimeProvider);

        await context.Ngos.AddAsync(testEntity);
        await context.SaveChangesAsync(CancellationToken.None);

        //Act 
        testEntity.UpdateDetails("new name");

        context.SaveChanges();

        //Assert
        testEntity.LastModifiedOn.Should().Be(lastModifiedOn);
        testEntity.LastModifiedBy.Should().Be(anotherUserId);
    }
}


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
