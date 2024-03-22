using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Domain.Entities.Auditing;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.UnitTests.Interceptors;

public class AuditTrailInterceptorTests
{
    private readonly ITimeProvider _fakeTimeProvider;
    private readonly ICurrentUserProvider _fakeCurrentUserProvider;
    private readonly TestContext _context;

    public AuditTrailInterceptorTests()
    {
        _fakeTimeProvider = Substitute.For<ITimeProvider>();
        _fakeCurrentUserProvider = Substitute.For<ICurrentUserProvider>();
        DbContextOptionsBuilder<VoteMonitorContext> dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new TestContext(dbContextOptions.Options,
            new SerializerService(),
            _fakeTimeProvider,
            _fakeCurrentUserProvider);
    }

    [Fact]
    public async Task Interceptor_OnEntityAdd_SaveChangesAsync_AddsCreateAuditTrail()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);

        _fakeTimeProvider.UtcNow.Returns(createdOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId);

        //Act 
        var ngoName = "my ngo";
        var testEntity = new Ngo(ngoName, _fakeTimeProvider);

        await _context.Ngos.AddAsync(testEntity);

        await _context.SaveChangesAsync(CancellationToken.None);

        //Assert
        var result = _context.AuditTrails.ToList();

        result.Count.Should().Be(1);
        var auditTrail = result.First();

        auditTrail.Type.Should().Be(TrailType.Create);
        auditTrail.PrimaryKey.Should().Contain(testEntity.Id.ToString());
        auditTrail.NewValues.Should().Contain(ngoName);
    }

    [Fact]
    public async Task Interceptor_OnEntityAdd_SaveChanges_AddsCreateAuditTrail()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);
        
        _fakeTimeProvider.UtcNow.Returns(createdOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId);

        //Act 
        var ngoName = "my ngo";
        var testEntity = new Ngo(ngoName, _fakeTimeProvider);

        await _context.Ngos.AddAsync(testEntity);

        _context.SaveChanges();

        //Assert
        var result = _context.AuditTrails.ToList();

        result.Count.Should().Be(1);
        var auditTrail = result.First();

        auditTrail.Type.Should().Be(TrailType.Create);
        auditTrail.PrimaryKey.Should().Contain(testEntity.Id.ToString());
        auditTrail.NewValues.Should().Contain(ngoName);
    }

    [Fact]
    public async Task Interceptor_OnEntityUpdate_SaveChangesAsync_AddsUpdateAudit()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);
        var lastModifiedOn = new DateTime(2024, 03, 24, 0, 0, 0);

        _fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId, anotherUserId);

        var ngoName = "my ngo";
        var testEntity = new Ngo(ngoName, _fakeTimeProvider);

        await _context.Ngos.AddAsync(testEntity);
        await _context.SaveChangesAsync(CancellationToken.None);

        //Act 
        var newNgoName = "new name";
        testEntity.UpdateDetails(newNgoName);

        await _context.SaveChangesAsync(CancellationToken.None);

        //Assert
        var result = _context.AuditTrails.ToList();

        result.Count.Should().Be(2);
        var auditTrail = result.First(x => x.Type == TrailType.Update);

        auditTrail.Type.Should().Be(TrailType.Update);
        auditTrail.PrimaryKey.Should().Contain(testEntity.Id.ToString());
        auditTrail.OldValues.Should().Contain(ngoName);
        auditTrail.NewValues.Should().Contain(newNgoName);
    }

    [Fact]
    public async Task Interceptor_OnEntityUpdate_SaveChanges_AddsUpdateAudit()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);
        var lastModifiedOn = new DateTime(2024, 03, 24, 0, 0, 0);
        
        _fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId, anotherUserId);
        
        var ngoName = "my ngo";
        var testEntity = new Ngo(ngoName, _fakeTimeProvider);

        await _context.Ngos.AddAsync(testEntity);
        await _context.SaveChangesAsync(CancellationToken.None);

        //Act 
        var newNgoName = "new name";
        testEntity.UpdateDetails(newNgoName);

        _context.SaveChanges();

        //Assert
        var result = _context.AuditTrails.ToList();

        result.Count.Should().Be(2);
        var auditTrail = result.First(x => x.Type == TrailType.Update);

        auditTrail.Type.Should().Be(TrailType.Update);
        auditTrail.PrimaryKey.Should().Contain(testEntity.Id.ToString());
        auditTrail.OldValues.Should().Contain(ngoName);
        auditTrail.NewValues.Should().Contain(newNgoName);
    }

    [Fact]
    public async Task Interceptor_OnEntityDelete_SaveChangesAsync_AddsDeleteAudit()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);
        var lastModifiedOn = new DateTime(2024, 03, 24, 0, 0, 0);

        _fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId, anotherUserId);

        var ngoName = "my ngo";
        var testEntity = new Ngo(ngoName, _fakeTimeProvider);

        await _context.Ngos.AddAsync(testEntity);
        await _context.SaveChangesAsync(CancellationToken.None);

        //Act 
        _context.Ngos.Remove(testEntity);

        await _context.SaveChangesAsync(CancellationToken.None);

        //Assert
        var result = _context.AuditTrails.ToList();

        result.Count.Should().Be(2);
        var auditTrail = result.First(x => x.Type == TrailType.Delete);

        auditTrail.Type.Should().Be(TrailType.Delete);
        auditTrail.PrimaryKey.Should().Contain(testEntity.Id.ToString());
        auditTrail.OldValues.Should().Contain(ngoName);
    }

    [Fact]
    public async Task Interceptor_OnEntityDelete_SaveChanges_AddsDeleteAudit()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);
        var lastModifiedOn = new DateTime(2024, 03, 24, 0, 0, 0);

        _fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId, anotherUserId);

        var ngoName = "my ngo";
        var testEntity = new Ngo(ngoName, _fakeTimeProvider);

        await _context.Ngos.AddAsync(testEntity);
        await _context.SaveChangesAsync(CancellationToken.None);

        //Act 
        _context.Ngos.Remove(testEntity);

        _context.SaveChanges();

        //Assert
        var result = _context.AuditTrails.ToList();

        result.Count.Should().Be(2);
        var auditTrail = result.First(x => x.Type == TrailType.Delete);

        auditTrail.Type.Should().Be(TrailType.Delete);
        auditTrail.PrimaryKey.Should().Contain(testEntity.Id.ToString());
        auditTrail.OldValues.Should().Contain(ngoName);
    }
}
