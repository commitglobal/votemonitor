using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.UnitTests.Interceptors;

public class AuditingInterceptorTests
{
    private readonly ITimeProvider _fakeTimeProvider;
    private readonly ICurrentUserProvider _fakeCurrentUserProvider;
    private readonly TestContext _context;

    public AuditingInterceptorTests()
    {
        _fakeTimeProvider = Substitute.For<ITimeProvider>();
        var fakeSerializationService = Substitute.For<ISerializerService>();
        _fakeCurrentUserProvider = Substitute.For<ICurrentUserProvider>();

        var dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

        _context = new TestContext(dbContextOptions.Options,
            fakeSerializationService,
            _fakeTimeProvider,
            _fakeCurrentUserProvider);
    }

    [Fact]
    public async Task Interceptor_OnEntityAdd_SaveChangesAsync_SetsCreatedFields()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);

        _fakeTimeProvider.UtcNow.Returns(createdOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId);
        //Act 
        var testEntity = new Ngo(string.Empty, _fakeTimeProvider);

        await _context.Ngos.AddAsync(testEntity);

        await _context.SaveChangesAsync(CancellationToken.None);

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

        _fakeTimeProvider.UtcNow.Returns(createdOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId);

        //Act 
        var testEntity = new Ngo(string.Empty, _fakeTimeProvider);

        await _context.Ngos.AddAsync(testEntity);

        _context.SaveChanges();

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

        _fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId, anotherUserId);
        
        var testEntity = new Ngo(string.Empty, _fakeTimeProvider);

        await _context.Ngos.AddAsync(testEntity);
        await _context.SaveChangesAsync(CancellationToken.None);

        //Act 
        testEntity.UpdateDetails("new name");

        await _context.SaveChangesAsync(CancellationToken.None);

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

        _fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId, anotherUserId);
        
        var testEntity = new Ngo(string.Empty, _fakeTimeProvider);

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
