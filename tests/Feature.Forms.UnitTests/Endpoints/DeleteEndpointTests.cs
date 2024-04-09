using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IRepository<Form> _repository = Substitute.For<IRepository<Form>>();
    private readonly Delete.Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _endpoint = Factory.Create<Delete.Endpoint>(_authorizationService, _repository);
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenUserIsNotAuthorized()
    {
        // Arrange
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Delete.Request();

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task Should_DeleteForm_And_ReturnNoContent_WhenFormExists()
    {
        // Arrange
        var form = new FormAggregateFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .Returns(form);

        // Act
        var request = new Delete.Request { Id = form.Id };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository.Received(1).DeleteAsync(form);

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormNotFound()
    {
        // Arrange
        var request = new Delete.Request { Id = Guid.NewGuid() };

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .ReturnsNullForAnyArgs();

        // Act
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository.DidNotReceiveWithAnyArgs().DeleteAsync(Arg.Any<Form>());

        result
            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
