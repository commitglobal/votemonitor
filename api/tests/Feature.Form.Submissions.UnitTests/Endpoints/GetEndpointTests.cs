using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<FormSubmission> _repository;
    private readonly IAuthorizationService _authorizationService;
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IReadRepository<FormSubmission>>();
        _endpoint = Factory.Create<Get.Endpoint>(_authorizationService,_repository);
        
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());
    }
    
    [Fact]
    public async Task ShouldReturnNotFound_WhenUserIsNotAuthorized()
    {
        // Arrange
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            FormId = Guid.NewGuid()
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
    
    [Fact]
    public async Task Should_GetPollingStationInformation_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var formSubmission = new FormSubmissionFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormSubmissionSpecification>())
            .Returns(formSubmission);

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert

        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<FormSubmissionModel>>()
            .Which.Value.Should().BeEquivalentTo(formSubmission, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationInformationNotFound()
    {
        // Arrange
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormSubmissionSpecification>())
            .ReturnsNull();

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert

        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
