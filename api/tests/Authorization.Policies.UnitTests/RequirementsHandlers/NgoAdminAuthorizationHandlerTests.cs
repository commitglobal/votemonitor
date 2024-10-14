using Authorization.Policies.RequirementHandlers;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.NgoAdminAggregate;

namespace Authorization.Policies.UnitTests.RequirementsHandlers;

public class NgoAdminAuthorizationHandlerTests
{
    private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
    private readonly ICurrentUserRoleProvider _currentUserRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
    private readonly IReadRepository<NgoAdmin> _ngoAdminRepository = Substitute.For<IReadRepository<NgoAdmin>>();

    private readonly Guid _ngoId = Guid.NewGuid();
    private readonly Guid _ngoAdminId = Guid.NewGuid();

    private readonly AuthorizationHandlerContext _context;
    private readonly NgoAdminAuthorizationHandler _handler;

    public NgoAdminAuthorizationHandlerTests()
    {
        var requirement = new NgoAdminRequirement(_ngoId);
        _context = new AuthorizationHandlerContext([requirement], null!, null);
        _handler = new NgoAdminAuthorizationHandler(_currentUserProvider,
            _currentUserRoleProvider,
            _ngoAdminRepository);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsNotNgoAdmin_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(false);
        _currentUserProvider.GetNgoId().Returns(_ngoId);
        _currentUserProvider.GetUserId().Returns(_ngoAdminId);

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_NgoAdminNotFound_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserProvider.GetNgoId().Returns(_ngoId);
        _currentUserProvider.GetUserId().Returns(_ngoAdminId);

        _ngoAdminRepository
            .FirstOrDefaultAsync(Arg.Any<GetNgoAdminSpecification>())
            .ReturnsNull();

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
        _currentUserProvider.GetNgoId().Returns(_ngoId);
        _currentUserProvider.GetUserId().Returns(_ngoAdminId);

        _ngoAdminRepository
            .FirstOrDefaultAsync(Arg.Any<GetNgoAdminSpecification>())
            .Returns(new NgoAdminView
            {
                NgoId = _ngoId,
                NgoStatus = NgoStatus.Deactivated,
                NgoAdminId = _ngoAdminId,
                UserStatus = UserStatus.Active,
            });

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsDeactivated_Failure()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserProvider.GetNgoId().Returns(_ngoId);
        _currentUserProvider.GetUserId().Returns(_ngoAdminId);

        _ngoAdminRepository
            .FirstOrDefaultAsync(Arg.Any<GetNgoAdminSpecification>())
            .Returns(new NgoAdminView
            {
                NgoId = _ngoId,
                NgoStatus = NgoStatus.Activated,
                NgoAdminId = _ngoAdminId,
                UserStatus = UserStatus.Deactivated,
            });

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_ValidNgoAdmin_Success()
    {
        // Arrange
        _currentUserRoleProvider.IsNgoAdmin().Returns(true);
        _currentUserProvider.GetNgoId().Returns(_ngoId);
        _currentUserProvider.GetUserId().Returns(_ngoAdminId);

        _ngoAdminRepository
            .FirstOrDefaultAsync(Arg.Any<GetNgoAdminSpecification>())
            .Returns(new NgoAdminView
            {
                NgoId = _ngoId,
                NgoStatus = NgoStatus.Activated,
                NgoAdminId = _ngoAdminId,
                UserStatus = UserStatus.Active,
            });

        // Act
        await _handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeTrue();
    }
}
