using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.UnitTests.Endpoints;

public class PublishEndpointTests
{
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IRepository<Form> _repository = Substitute.For<IRepository<Form>>();
    private readonly Publish.Endpoint _endpoint;

    public PublishEndpointTests()
    {
        _endpoint = Factory.Create<Publish.Endpoint>(_authorizationService, _repository);
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
        var request = new Publish.Request();

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task Should_PublishForm_And_Return_NoContent_WhenFormExists()
    {
        // Arrange
        var form = new FormAggregateFaker(status: FormStatus.Published).Generate();
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .Returns(form);

        // Act
        var request = new Publish.Request { Id = form.Id };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(1)
            .UpdateAsync(Arg.Is<Form>(x => x.Status == FormStatus.Published));

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormDoesNotExists()
    {
        // Arrange
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .ReturnsNullForAnyArgs();

        var request = new Publish.Request { Id = Guid.NewGuid() };

        // Act
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
