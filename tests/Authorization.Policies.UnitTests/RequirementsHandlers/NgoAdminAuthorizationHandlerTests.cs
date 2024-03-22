using Authorization.Policies.RequirementHandlers;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Authorization.Policies.UnitTests.RequirementsHandlers;

public class NgoAdminAuthorizationHandlerTests
{
    private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
    private readonly IReadRepository<NgoAdmin> _ngoAdminRepository = Substitute.For<IReadRepository<NgoAdmin>>();

    private readonly Guid _ngoId = Guid.NewGuid();
    private readonly Guid _ngoAdminId = Guid.NewGuid();

    private readonly AuthorizationHandlerContext _context;

    public NgoAdminAuthorizationHandlerTests()
    {
        var requirement = new NgoAdminRequirement(_ngoId);
        _context = new AuthorizationHandlerContext([requirement], null, null);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsNotNgoAdmin_Failure()
    {
        // Arrange
        _currentUserProvider.IsNgoAdmin().Returns(false);
        _currentUserProvider.GetNgoId().Returns(_ngoId);
        _currentUserProvider.GetUserId().Returns(_ngoAdminId);

        var handler = new NgoAdminAuthorizationHandler(_currentUserProvider, _ngoAdminRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_NgoAdminNotFound_Failure()
    {
        // Arrange
        _currentUserProvider.IsNgoAdmin().Returns(true);
        _currentUserProvider.GetNgoId().Returns(_ngoId);
        _currentUserProvider.GetUserId().Returns(_ngoAdminId);

        _ngoAdminRepository
            .FirstOrDefaultAsync(Arg.Any<GetNgoAdminSpecification>())
            .ReturnsNull();

        var handler = new NgoAdminAuthorizationHandler(_currentUserProvider, _ngoAdminRepository);

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

        var handler = new NgoAdminAuthorizationHandler(_currentUserProvider, _ngoAdminRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_UserIsDeactivated_Failure()
    {
        // Arrange
        _currentUserProvider.IsNgoAdmin().Returns(true);
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

        var handler = new NgoAdminAuthorizationHandler(_currentUserProvider, _ngoAdminRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_ValidNgoAdmin_Success()
    {
        // Arrange
        _currentUserProvider.IsNgoAdmin().Returns(true);
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

        var handler = new NgoAdminAuthorizationHandler(_currentUserProvider, _ngoAdminRepository);

        // Act
        await handler.HandleAsync(_context);

        // Assert
        _context.HasSucceeded.Should().BeTrue();
    }
}
