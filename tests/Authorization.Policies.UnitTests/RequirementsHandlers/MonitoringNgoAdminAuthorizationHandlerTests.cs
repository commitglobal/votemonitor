using Authorization.Policies.RequirementHandlers;

namespace Authorization.Policies.UnitTests.RequirementsHandlers;

public class MonitoringNgoAdminAuthorizationHandlerTests
{
    private readonly ICurrentUserRoleProvider _currentUserRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
    private readonly IReadRepository<MonitoringNgo> _monitoringNgoRepository = Substitute.For<IReadRepository<MonitoringNgo>>();

    private readonly Guid _electionRoundId = Guid.NewGuid();
    private readonly Guid _ngoId = Guid.NewGuid();

    private readonly AuthorizationHandlerContext _context;

    public MonitoringNgoAdminAuthorizationHandlerTests()
    {
        var requirement = new MonitoringNgoAdminRequirement(_electionRoundId);
        _context = new AuthorizationHandlerContext([requirement], null!, null);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsNotNgoAdmin_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(false);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        var handler = new MonitoringNgoAdminAuthorizationHandler(_currentUserRoleProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_MonitoringNgoNotFound_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .ReturnsNull();

        var handler = new MonitoringNgoAdminAuthorizationHandler(_currentUserRoleProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_ElectionRoundArchived_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(CreateMonitoringNgoView.With().ArchivedElectionRound());

        var handler = new MonitoringNgoAdminAuthorizationHandler(_currentUserRoleProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_NgoIsDeactivated_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(CreateMonitoringNgoView.With().DeactivatedNgo());

        var handler = new MonitoringNgoAdminAuthorizationHandler(_currentUserRoleProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_MonitoringNgoIsSuspended_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(CreateMonitoringNgoView.With().SuspendedMonitoringNgo());

        var handler = new MonitoringNgoAdminAuthorizationHandler(_currentUserRoleProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_ValidMonitoringNgo_Success()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(CreateMonitoringNgoView.ForValidAccess());

        var handler = new MonitoringNgoAdminAuthorizationHandler(_currentUserRoleProvider, _monitoringNgoRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeTrue();
    }
}
