using Authorization.Policies.RequirementHandlers;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Authorization.Policies.UnitTests.RequirementsHandlers;

public class MonitoringNgoAdminOrObserverAuthorizationHandlerTests
{
    private readonly ICurrentUserIdProvider _currentUserIdProvider = Substitute.For<ICurrentUserIdProvider>();
    private readonly ICurrentUserRoleProvider _currentUserRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
    private readonly IReadRepository<MonitoringNgo> _monitoringNgoRepository = Substitute.For<IReadRepository<MonitoringNgo>>();
    private readonly IReadRepository<MonitoringObserver> _monitoringObserverRepository = Substitute.For<IReadRepository<MonitoringObserver>>();

    private readonly Guid _electionRoundId = Guid.NewGuid();
    private readonly Guid _ngoId = Guid.NewGuid();
    private readonly Guid _observerId = Guid.NewGuid();

    private readonly AuthorizationHandlerContext _context;
    private readonly MonitoringNgoAdminOrObserverAuthorizationHandler _handler;

    public MonitoringNgoAdminOrObserverAuthorizationHandlerTests()
    {
        var requirement = new MonitoringNgoAdminOrObserverRequirement(_electionRoundId);
        _context = new AuthorizationHandlerContext([requirement], null!, null);
        _handler = new MonitoringNgoAdminOrObserverAuthorizationHandler(_currentUserIdProvider,
            _currentUserRoleProvider,
            _monitoringObserverRepository,
            _monitoringNgoRepository);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsNotNgoAdminOrObserver_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(false);
        _currentUserRoleProvider.IsObserver().Returns(false);

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_MonitoringNgoNotFound_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserRoleProvider.IsObserver().Returns(false);
        _currentUserIdProvider.GetUserId().Returns(_observerId);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .ReturnsNull();

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_ElectionRoundArchived_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserRoleProvider.IsObserver().Returns(false);
        _currentUserIdProvider.GetUserId().Returns(_observerId);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(CreateMonitoringNgoView.With().ArchivedElectionRound());

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_NgoIsDeactivated_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserRoleProvider.IsObserver().Returns(false);
        _currentUserIdProvider.GetUserId().Returns(_observerId);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(CreateMonitoringNgoView.With().DeactivatedNgo());

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_MonitoringNgoIsSuspended_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserRoleProvider.IsObserver().Returns(false);
        _currentUserIdProvider.GetUserId().Returns(_observerId);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(CreateMonitoringNgoView.With().SuspendedMonitoringNgo());

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_ValidMonitoringNgo_Success()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserRoleProvider.IsObserver().Returns(false);
        _currentUserIdProvider.GetUserId().Returns(_observerId);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(CreateMonitoringNgoView.ForValidAccess());

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_MonitoringObserverNotFound_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(true);
        _currentUserRoleProvider.IsNgoAdmin().Returns(false);
        _currentUserIdProvider.GetUserId().Returns(_observerId);
        _currentUserRoleProvider.GetNgoId().Returns(_ngoId);

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
        _currentUserRoleProvider.IsNgoAdmin().Returns(false);
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
    public async Task HandleRequirementAsync_ObserverNgoIsDeactivated_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(true);
        _currentUserRoleProvider.IsNgoAdmin().Returns(false);
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
    public async Task HandleRequirementAsync_ObserverMonitoringNgoIsSuspended_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsObserver().Returns(true);
        _currentUserRoleProvider.IsNgoAdmin().Returns(false);
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
        _currentUserRoleProvider.IsNgoAdmin().Returns(false);
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
        _currentUserRoleProvider.IsNgoAdmin().Returns(false);
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
        _currentUserRoleProvider.IsNgoAdmin().Returns(false);
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
