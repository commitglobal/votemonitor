using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.IntegrationTests.Interceptors;

using static ApiTesting;

public class AuditingInterceptorTests : BaseDbTestFixture
{
    private ICurrentUserProvider _currentUserProvider;
    private ITimeProvider _fakeTimeProvider;
    private VoteMonitorContext _context;

    [SetUp]
    public void Init()
    {
        _fakeTimeProvider = Substitute.For<ITimeProvider>();
        _currentUserProvider = Substitute.For<ICurrentUserProvider>();
        var options = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseNpgsql(DbConnectionString)
            .AddInterceptors(new AuditingInterceptor(_currentUserProvider, _fakeTimeProvider))
            .Options;

        _context = new VoteMonitorContext(options);
    }

    [TearDown]
    public void Cleanup()
    {
        _fakeTimeProvider = null!;
        _currentUserProvider = null!;
        _context.Dispose();
    }

    [Test]
    public async Task Interceptor_OnEntityAdd_SaveChangesAsync_SetsCreatedFields()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46, DateTimeKind.Utc);

        _fakeTimeProvider.UtcNow.Returns(createdOn);
        _currentUserProvider.GetUserId().Returns(userId);
        //Act 
        var testEntity = new Ngo(string.Empty);

        await _context.Ngos.AddAsync(testEntity);

        await _context.SaveChangesAsync(CancellationToken.None);

        //Assert
        testEntity.CreatedOn.Should().Be(createdOn);
        testEntity.CreatedBy.Should().Be(userId);

        testEntity.LastModifiedOn.Should().BeNull();
        testEntity.LastModifiedBy.Should().Be(userId);
    }

    [Test]
    public async Task Interceptor_OnEntityAdd_SaveChanges_SetsCreatedFields()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46, DateTimeKind.Utc);

        _fakeTimeProvider.UtcNow.Returns(createdOn);
        _currentUserProvider.GetUserId().Returns(userId);

        //Act 
        var testEntity = new Ngo(string.Empty);

        await _context.Ngos.AddAsync(testEntity);

        _context.SaveChanges();

        //Assert
        testEntity.CreatedOn.Should().Be(createdOn);
        testEntity.CreatedBy.Should().Be(userId);

        testEntity.LastModifiedOn.Should().BeNull();
        testEntity.LastModifiedBy.Should().Be(userId);
    }

    [Test]
    public async Task Interceptor_OnEntityUpdate_SaveChangesAsync_SetsLastModifiedFields()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46, DateTimeKind.Utc);
        var lastModifiedOn = new DateTime(2024, 03, 24, 0, 0, 0, DateTimeKind.Utc);

        _fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        _currentUserProvider.GetUserId().Returns(userId, anotherUserId);

        var testEntity = new Ngo(string.Empty);

        await _context.Ngos.AddAsync(testEntity);
        await _context.SaveChangesAsync(CancellationToken.None);

        //Act 
        testEntity.UpdateDetails("new name");

        await _context.SaveChangesAsync(CancellationToken.None);

        //Assert
        testEntity.LastModifiedOn.Should().Be(lastModifiedOn);
        testEntity.LastModifiedBy.Should().Be(anotherUserId);
    }

    [Test]
    public async Task Interceptor_OnEntityUpdate_SaveChanges_SetsLastModifiedFields()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46, DateTimeKind.Utc);
        var lastModifiedOn = new DateTime(2024, 03, 24, 0, 0, 0, DateTimeKind.Utc);

        _fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        _currentUserProvider.GetUserId().Returns(userId, anotherUserId);

        var testEntity = new Ngo(string.Empty);

        await _context.Ngos.AddAsync(testEntity);
        await _context.SaveChangesAsync(CancellationToken.None);

        //Act 
        testEntity.UpdateDetails("new name");

        _context.SaveChanges();

        //Assert
        testEntity.LastModifiedOn.Should().Be(lastModifiedOn);
        testEntity.LastModifiedBy.Should().Be(anotherUserId);
    }
}
