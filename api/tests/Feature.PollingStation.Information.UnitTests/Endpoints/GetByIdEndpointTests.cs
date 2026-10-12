using System.Security.Claims;
using Feature.PollingStation.Information.GetById;
using Feature.PollingStation.Information.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;

namespace Feature.PollingStation.Information.UnitTests.Endpoints;

public class GetByIdEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserRoleProvider _userRoleProvider;
    private readonly IReadRepository<PollingStationInformation> _repository;
    private readonly Endpoint _endpoint;

    public GetByIdEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _userRoleProvider = Substitute.For<ICurrentUserRoleProvider>();
        _repository = Substitute.For<IReadRepository<PollingStationInformation>>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService, _userRoleProvider, _repository);
    }

    [Fact]
    public async Task ShouldReturnPollingStationInformation_ForPlatformAdmin_WithoutNgoAuthorization()
    {
        // Arrange
        _userRoleProvider.IsPlatformAdmin().Returns(true);
        var model = PollingStationInformationModel.FromEntity(new PollingStationInformationFaker().Generate());

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationByIdSpecification>())
            .Returns(model);

        // Act
        var request = new Request { ElectionRoundId = Guid.NewGuid(), Id = model.Id };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _authorizationService
            .DidNotReceiveWithAnyArgs()
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>());

        result
            .Result.Should().BeOfType<Ok<PollingStationInformationModel>>()
            .Which.Value.Should().BeEquivalentTo(model);
    }

    [Fact]
    public async Task ShouldReturnPollingStationInformation_ForAuthorizedNgoUser()
    {
        // Arrange
        _userRoleProvider.IsPlatformAdmin().Returns(false);
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());
        var model = PollingStationInformationModel.FromEntity(new PollingStationInformationFaker().Generate());

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationByIdSpecification>())
            .Returns(model);

        // Act
        var request = new Request { ElectionRoundId = Guid.NewGuid(), Id = model.Id, NgoId = Guid.NewGuid() };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Result.Should().BeOfType<Ok<PollingStationInformationModel>>()
            .Which.Value.Should().BeEquivalentTo(model);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNgoUserIsNotAuthorized()
    {
        // Arrange
        _userRoleProvider.IsPlatformAdmin().Returns(false);
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Request { ElectionRoundId = Guid.NewGuid(), Id = Guid.NewGuid(), NgoId = Guid.NewGuid() };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFound>();
        await _repository
            .DidNotReceiveWithAnyArgs()
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationByIdSpecification>());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationInformationNotFound()
    {
        // Arrange
        _userRoleProvider.IsPlatformAdmin().Returns(true);
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationByIdSpecification>())
            .ReturnsNull();

        // Act
        var request = new Request { ElectionRoundId = Guid.NewGuid(), Id = Guid.NewGuid() };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFound>();
    }
}
