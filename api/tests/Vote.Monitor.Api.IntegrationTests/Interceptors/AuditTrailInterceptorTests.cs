using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.Auditing;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;
using Vote.Monitor.Domain.Entities.ObserverAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Api.IntegrationTests.Interceptors;
using static DbTesting;
public class AuditTrailInterceptorTests: BaseDbTestFixture
{
    private  ITimeProvider _fakeTimeProvider;
    private  ICurrentUserProvider _fakeCurrentUserProvider;
    private  VoteMonitorContext _context;

    [SetUp]
    public void Init()
    {
        _fakeTimeProvider = Substitute.For<ITimeProvider>();
        var fakeSerializationService = Substitute.For<ISerializerService>();
        _fakeCurrentUserProvider = Substitute.For<ICurrentUserProvider>();

        var options = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseNpgsql(DbConnectionString)
            .Options;

        _context = new VoteMonitorContext(options,
            fakeSerializationService,
            _fakeTimeProvider,
            _fakeCurrentUserProvider);
    }

    [TearDown]
    public void Cleanup()
    {
        _fakeTimeProvider = null!;
        _fakeCurrentUserProvider = null!;
        _context.Dispose();
    }


    [Test]
    public async Task Interceptor_OnEntityAdd_SaveChangesAsync_AddsCreateAuditTrail()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);

        _fakeTimeProvider.UtcNow.Returns(createdOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId);

        //Act 
        var ngoName = "my ngo";
        var testEntity = new Ngo(ngoName);

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

    [Test]
    public async Task Interceptor_OnEntityAdd_SaveChanges_AddsCreateAuditTrail()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);

        _fakeTimeProvider.UtcNow.Returns(createdOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId);

        //Act 
        var ngoName = "my ngo";
        var testEntity = new Ngo(ngoName);

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

    [Test]
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
        var testEntity = new Ngo(ngoName);

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

    [Test]
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
        var testEntity = new Ngo(ngoName);

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

    [Test]
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
        var testEntity = new Ngo(ngoName);

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

    [Test]
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
        var testEntity = new Ngo(ngoName);

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

    [Test]
    public async Task Interceptor_OnAddFormSubmission_ShouldNotTriggerAuditTrailForForm()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var createdOn = new DateTime(2024, 03, 22, 12, 35, 46);
        var lastModifiedOn = new DateTime(2024, 03, 24, 0, 0, 0);

        _fakeTimeProvider.UtcNow.Returns(createdOn, lastModifiedOn);
        _fakeCurrentUserProvider.GetUserId().Returns(userId, anotherUserId);


        var electionRound = new ElectionRound(CountriesList.AD.Id, "ololo", "ololoo", new DateOnly(2020, 1, 1));
        var pollingStation = new PollingStationFaker(electionRound: electionRound).Generate();
        var ngo = new Ngo("ngo");
        var monitoringNgo = new MonitoringNgoAggregateFaker(electionRound: electionRound, ngo: ngo).Generate();
        var form = new FormAggregateFaker(electionRound: electionRound, monitoringNgo: monitoringNgo).Generate();
        var applicationUser = ApplicationUser.CreateObserver("test", "test", "email", "222", "password");

        var observer = Observer.Create(applicationUser);
        var monitoringObserver = new MonitoringObserverFaker(electionRound: electionRound, monitoringNgo: monitoringNgo,
                observer: observer)
            .Generate();

        await _context.Countries.AddAsync(CountriesList.AD.ToEntity());
        await _context.ElectionRounds.AddAsync(electionRound);
        await _context.PollingStations.AddAsync(pollingStation);
        await _context.Ngos.AddAsync(ngo);
        await _context.MonitoringNgos.AddAsync(monitoringNgo);
        await _context.Users.AddAsync(applicationUser);
        await _context.Observers.AddAsync(observer);
        await _context.MonitoringObservers.AddAsync(monitoringObserver);
        await _context.Forms.AddAsync(form);
        await _context.SaveChangesAsync(CancellationToken.None);

        var initialTrails = _context.AuditTrails.ToList();

        //Act 
        var formSubmission = form.CreateFormSubmission(pollingStation, monitoringObserver, [], false);
        _context.FormSubmissions.Add(formSubmission);

        await _context.SaveChangesAsync(CancellationToken.None);

        //Assert
        var trailsAfterSubmissions = _context.AuditTrails.ToList();

        var newTrails = trailsAfterSubmissions.Except(initialTrails).ToList();

        newTrails.Should().ContainSingle();

        newTrails.First().Type.Should().Be(TrailType.Create);
        newTrails.First().TableName.Should().Be("FormSubmission");
    }
}
