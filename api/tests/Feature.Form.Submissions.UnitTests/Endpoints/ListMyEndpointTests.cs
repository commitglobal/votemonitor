using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class ListMyEndpointTests
{
    private readonly IReadRepository<FormSubmission> _repository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ListMy.Endpoint _endpoint;

    public ListMyEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<FormSubmission>>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _endpoint = Factory.Create<ListMy.Endpoint>(_authorizationService, _repository);
        
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
        var request = new ListMy.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<ListMy.Response>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
    
    
    [Fact]
    public async Task Should_GetPollingStationInformation_ForNgo()
    {
        // Arrange
        var formSubmissions = new FormSubmissionFaker()
            .Generate(10)
            .Select(FormSubmissionModel.FromEntity)
            .ToList();

        _repository
            .ListAsync(Arg.Any<GetFormSubmissionForObserverSpecification>())
            .Returns(formSubmissions);

        // Act
        var request = new ListMy.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var okResult = result
            .Should()
            .BeOfType<Results<Ok<ListMy.Response>, NotFound>>()
            .Which.Result
            .As<Ok<ListMy.Response>>();

        okResult.Value!.Submissions.Should().BeEquivalentTo(formSubmissions);
    }
}
