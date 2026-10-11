using Authorization.Policies.RequirementHandlers;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.NgoStaffAggregate;

namespace Authorization.Policies.UnitTests.RequirementsHandlers;

public class CitizenReportingNgoAdminOrStaffAuthorizationHandlerTests
{
    private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
    private readonly ICurrentUserRoleProvider _currentUserRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
    private readonly IReadRepository<ElectionRound> _electionRoundRepository = Substitute.For<IReadRepository<ElectionRound>>();
    private readonly IReadRepository<NgoStaff> _ngoStaffRepository = Substitute.For<IReadRepository<NgoStaff>>();

    private readonly Guid _ngoId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    private readonly AuthorizationHandlerContext _context;
    private readonly CitizenReportingNgoAdminOrStaffAuthorizationHandler _handler;

    public CitizenReportingNgoAdminOrStaffAuthorizationHandlerTests()
    {
        _context = new AuthorizationHandlerContext([new CitizenReportingNgoAdminOrStaffRequirement(Guid.NewGuid())], null!, null);
        _handler = new CitizenReportingNgoAdminOrStaffAuthorizationHandler(_currentUserProvider, _currentUserRoleProvider,
            _electionRoundRepository, _ngoStaffRepository);

        _currentUserProvider.GetNgoId().Returns(_ngoId);
        _currentUserProvider.GetUserId().Returns(_userId);
        _electionRoundRepository
            .FirstOrDefaultAsync(Arg.Any<GetCitizenReportingMonitoringNgoSpecification>())
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
    public async Task HandleRequirementAsync_UserIsNgoAdmin_Success()
    {
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);

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
    public async Task HandleRequirementAsync_NgoIsNotCitizenReportingNgo_Failure()
    {
        _currentUserRoleProvider.IsNgoStaff().Returns(true);
        _ngoStaffRepository.FirstOrDefaultAsync(Arg.Any<GetNgoStaffSpecification>()).Returns(Staff(UserStatus.Active));
        _electionRoundRepository
            .FirstOrDefaultAsync(Arg.Any<GetCitizenReportingMonitoringNgoSpecification>())
            .ReturnsNull();

        await _handler.HandleAsync(_context);

        _context.HasSucceeded.Should().BeFalse();
    }
}
