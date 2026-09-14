using System.Security.Claims;
using Feature.CitizenReports.Comments.Delete;
using Feature.CitizenReports.Comments.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.CitizenReportCommentAggregate;

namespace Feature.CitizenReports.Comments.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<CitizenReportComment> _repository;
    private readonly Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IRepository<CitizenReportComment>>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService, _repository);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorized()
    {
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        var result = await _endpoint.ExecuteAsync(new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        }, CancellationToken.None);

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();

        await _repository.DidNotReceive().DeleteAsync(Arg.Any<CitizenReportComment>());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCommentDoesNotExist()
    {
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.FirstOrDefaultAsync(Arg.Any<GetCommentByIdSpecification>())
            .ReturnsNull();

        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldDeleteComment_WhenUserIsTheAuthor()
    {
        var userId = Guid.NewGuid();
        var fakeComment = new CitizenReportCommentFaker(authorId: userId).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.FirstOrDefaultAsync(Arg.Any<GetCommentByIdSpecification>())
            .Returns(fakeComment);

        var request = new Request
        {
            ElectionRoundId = fakeComment.ElectionRoundId,
            CitizenReportId = fakeComment.CitizenReportId,
            Id = fakeComment.Id,
            UserId = userId
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _repository
            .Received(1)
            .DeleteAsync(Arg.Is<CitizenReportComment>(x => x.Id == fakeComment.Id));

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }
}
