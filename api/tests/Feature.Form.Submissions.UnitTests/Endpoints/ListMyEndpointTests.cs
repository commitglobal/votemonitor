using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.TestUtils;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class ListMyEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ListMy.Endpoint _endpoint;

    public ListMyEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _endpoint = Factory.Create<ListMy.Endpoint>(_authorizationService, TestContext.Fake());
        
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

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<ListMy.Response>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
