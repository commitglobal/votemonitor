using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.TestUtils;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly Delete.Endpoint _endpoint;
    private readonly IAuthorizationService _authorizationService;

    public DeleteEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _endpoint = Factory.Create<Delete.Endpoint>(_authorizationService, TestContext.Fake());
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
        var request = new Delete.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            SubmissionId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
