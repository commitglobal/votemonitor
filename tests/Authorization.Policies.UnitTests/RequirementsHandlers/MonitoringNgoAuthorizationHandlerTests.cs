using Authorization.Policies.RequirementHandlers;

namespace Authorization.Policies.UnitTests.RequirementsHandlers;

public class MonitoringNgoAuthorizationHandlerTests
{
    private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
    private readonly IReadRepository<MonitoringNgo> _monitoringNgoRepository = Substitute.For<IReadRepository<MonitoringNgo>>();

    private readonly Guid _electionRoundId = Guid.NewGuid();
    private readonly Guid _ngoId = Guid.NewGuid();

    private readonly AuthorizationHandlerContext _context;

    public MonitoringNgoAuthorizationHandlerTests()
    {
        var requirement = new MonitoringNgoRequirement(_electionRoundId);
        _context = new AuthorizationHandlerContext([requirement], null, null);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsNotNgoAdmin_Failure()
    {
        // Arrange
        _currentUserProvider.IsNgoAdmin().Returns(false);
        _currentUserProvider.GetNgoId().Returns(_ngoId);

        var handler = new MonitoringNgoAuthorizationHandler(_currentUserProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_MonitoringNgoNotFound_Failure()
    {
        // Arrange
        _currentUserProvider.IsNgoAdmin().Returns(true);
        _currentUserProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .ReturnsNull();

        var handler = new MonitoringNgoAuthorizationHandler(_currentUserProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_NgoIsDeactivated_Failure()
    {
        // Arrange
        _currentUserProvider.IsNgoAdmin().Returns(true);
        _currentUserProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(new MonitoringNgoView
            {
                ElectionRoundId = _electionRoundId,
                ElectionRoundStatus = ElectionRoundStatus.Started,
                NgoId = _ngoId,
                NgoStatus = NgoStatus.Deactivated,
                MonitoringNgoId = Guid.NewGuid(),
                MonitoringNgoStatus = MonitoringNgoStatus.Active,
            });

        var handler = new MonitoringNgoAuthorizationHandler(_currentUserProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_NgoIsSuspended_Failure()
    {
        // Arrange
        _currentUserProvider.IsNgoAdmin().Returns(true);
        _currentUserProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(new MonitoringNgoView
            {
                ElectionRoundId = _electionRoundId,
                ElectionRoundStatus = ElectionRoundStatus.Started,
                NgoId = _ngoId,
                NgoStatus = NgoStatus.Activated,
                MonitoringNgoId = Guid.NewGuid(),
                MonitoringNgoStatus = MonitoringNgoStatus.Suspended,
            });

        var handler = new MonitoringNgoAuthorizationHandler(_currentUserProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_ValidMonitoringNgo_Success()
    {
        // Arrange
        _currentUserProvider.IsNgoAdmin().Returns(true);
        _currentUserProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(new MonitoringNgoView
            {
                ElectionRoundId = _electionRoundId,
                ElectionRoundStatus = ElectionRoundStatus.Started,
                NgoId = _ngoId,
                NgoStatus = NgoStatus.Activated,
                MonitoringNgoId = Guid.NewGuid(),
                MonitoringNgoStatus = MonitoringNgoStatus.Active,
            });

        var handler = new MonitoringNgoAuthorizationHandler(_currentUserProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeTrue();
    }
}
