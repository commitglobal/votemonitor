using Authorization.Policies.RequirementHandlers;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.NgoAdminAggregate;
using Vote.Monitor.Domain.Entities.NgoStaffAggregate;

namespace Authorization.Policies.UnitTests.RequirementsHandlers;

public class MonitoringNgoAdminOrStaffAuthorizationHandlerTests
{
    private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
    private readonly ICurrentUserRoleProvider _currentUserRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
    private readonly IReadRepository<MonitoringNgo> _monitoringNgoRepository = Substitute.For<IReadRepository<MonitoringNgo>>();
    private readonly IReadRepository<NgoAdmin> _ngoAdminRepository = Substitute.For<IReadRepository<NgoAdmin>>();
    private readonly IReadRepository<NgoStaff> _ngoStaffRepository = Substitute.For<IReadRepository<NgoStaff>>();

    private readonly Guid _ngoId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    private readonly AuthorizationHandlerContext _context;
    private readonly MonitoringNgoAdminOrStaffAuthorizationHandler _handler;

    public MonitoringNgoAdminOrStaffAuthorizationHandlerTests()
    {
        _context = new AuthorizationHandlerContext([new MonitoringNgoAdminOrStaffRequirement(Guid.NewGuid())], null!, null);
        _handler = new MonitoringNgoAdminOrStaffAuthorizationHandler(_currentUserProvider, _currentUserRoleProvider,
            _monitoringNgoRepository, _ngoAdminRepository, _ngoStaffRepository);

        _currentUserProvider.GetNgoId().Returns(_ngoId);
        _currentUserProvider.GetUserId().Returns(_userId);
        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(CreateMonitoringNgoView.ForValidAccess());
    }

    private NgoStaffView Staff(UserStatus status) => new()
    {
        NgoId = _ngoId, NgoStatus = NgoStatus.Activated, NgoStaffId = _userId, UserStatus = status
    };

    [Fact]
    public async Task HandleRequirementAsync_UserIsActiveNgoStaff_Success()
    {
        _currentUserRoleProvider.IsNgoStaff().Returns(true);
        _ngoStaffRepository.FirstOrDefaultAsync(Arg.Any<GetNgoStaffSpecification>()).Returns(Staff(UserStatus.Active));

        await _handler.HandleAsync(_context);

        _context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsActiveNgoAdmin_Success()
    {
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _ngoAdminRepository.FirstOrDefaultAsync(Arg.Any<GetNgoAdminSpecification>()).Returns(new NgoAdminView
        {
            NgoId = _ngoId, NgoStatus = NgoStatus.Activated, NgoAdminId = _userId, UserStatus = UserStatus.Active
        });

        await _handler.HandleAsync(_context);

        _context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsNeitherAdminNorStaff_Failure()
    {
        _currentUserRoleProvider.IsObserver().Returns(true);

        await _handler.HandleAsync(_context);

        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_StaffIsDeactivated_Failure()
    {
        _currentUserRoleProvider.IsNgoStaff().Returns(true);
        _ngoStaffRepository.FirstOrDefaultAsync(Arg.Any<GetNgoStaffSpecification>()).Returns(Staff(UserStatus.Deactivated));

        await _handler.HandleAsync(_context);

        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_StaffBelongsToAnotherNgo_Failure()
    {
        _currentUserRoleProvider.IsNgoStaff().Returns(true);
        _ngoStaffRepository.FirstOrDefaultAsync(Arg.Any<GetNgoStaffSpecification>()).ReturnsNull();

        await _handler.HandleAsync(_context);

        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_MonitoringNgoIsSuspended_Failure()
    {
        _currentUserRoleProvider.IsNgoStaff().Returns(true);
        _ngoStaffRepository.FirstOrDefaultAsync(Arg.Any<GetNgoStaffSpecification>()).Returns(Staff(UserStatus.Active));
        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(CreateMonitoringNgoView.With().SuspendedMonitoringNgo());

        await _handler.HandleAsync(_context);

        _context.HasSucceeded.Should().BeFalse();
    }
}
