using Authorization.Policies.RequirementHandlers;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Authorization.Policies.UnitTests.RequirementsHandlers;

public class MonitoringObserverAuthorizationHandlerTests
{
    private readonly ICurrentUserIdProvider _currentUserIdProvider = Substitute.For<ICurrentUserIdProvider>();
    private readonly ICurrentUserRoleProvider _currentUserRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
    private readonly IReadRepository<MonitoringObserver> _monitoringObserverRepository = Substitute.For<IReadRepository<MonitoringObserver>>();

    private readonly Guid _electionRoundId = Guid.NewGuid();
    private readonly Guid _observerId = Guid.NewGuid();

    private readonly AuthorizationHandlerContext _context;
    private readonly MonitoringObserverAuthorizationHandler _handler;

    public MonitoringObserverAuthorizationHandlerTests()
    {
        var requirement = new MonitoringObserverRequirement(_electionRoundId);
        _context = new AuthorizationHandlerContext([requirement], null!, null);
        _handler = new MonitoringObserverAuthorizationHandler(_currentUserIdProvider, 
            _currentUserRoleProvider,
            _monitoringObserverRepository);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsNotObserver_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(false);
        _currentUserIdProvider.GetUserId().Returns(_observerId);

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_MonitoringObserverNotFound_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(true);
        _currentUserIdProvider.GetUserId().Returns(_observerId);

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .ReturnsNull();

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_ElectionRoundIsArchived_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(true);
        _currentUserIdProvider.GetUserId().Returns(_observerId);

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(CreateMonitoringObserverView.With().ArchivedElectionRound());

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }    
    
    [Fact]
    public async Task HandleRequirementAsync_NgoIsDeactivated_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(true);
        _currentUserIdProvider.GetUserId().Returns(_observerId);

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(CreateMonitoringObserverView.With().DeactivatedNgo());

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_MonitoringNgoIsSuspended_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(true);
        _currentUserIdProvider.GetUserId().Returns(_observerId);

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(CreateMonitoringObserverView.With().SuspendedMonitoringNgo());

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsDeactivated_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(true);
        _currentUserIdProvider.GetUserId().Returns(_observerId);

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(CreateMonitoringObserverView.With().DeactivatedObserver());

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_MonitoringObserverIsSuspended_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(true);
        _currentUserIdProvider.GetUserId().Returns(_observerId);

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(CreateMonitoringObserverView.With().SuspendedMonitoringObserver());

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_ValidMonitoringObserver_Success()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(true);
        _currentUserIdProvider.GetUserId().Returns(_observerId);

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(CreateMonitoringObserverView.ForValidAccess());

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeTrue();
    }
}
