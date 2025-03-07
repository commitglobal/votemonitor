using System.Security.Claims;
using Feature.Form.Submissions.ListEntries;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly Endpoint _endpoint;

    public ListEndpointTests()
    {
        var connectionFactory = Substitute.For<INpgsqlConnectionFactory>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService, connectionFactory);
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
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            FormId = Guid.NewGuid()
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PagedResponse<FormSubmissionEntry>>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}