using System.Security.Claims;
using Feature.CitizenReports.Comments.Specifications;
using Feature.CitizenReports.Comments.Update;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.CitizenReportCommentAggregate;

namespace Feature.CitizenReports.Comments.UnitTests.Endpoints;

public class UpdateEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<CitizenReportComment> _repository;
    private readonly Endpoint _endpoint;

    public UpdateEndpointTests()
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
            UserId = Guid.NewGuid(),
            Text = "updated"
        }, CancellationToken.None);

        result
            .Should().BeOfType<Results<Ok<CitizenReportCommentModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<CitizenReportComment>());
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
            UserId = Guid.NewGuid(),
            Text = "updated"
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result
            .Should().BeOfType<Results<Ok<CitizenReportCommentModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldUpdateComment_WhenUserIsTheAuthor()
    {
        var userId = Guid.NewGuid();
        var fakeComment = new CitizenReportCommentFaker(authorId: userId).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.FirstOrDefaultAsync(Arg.Any<GetCommentByIdSpecification>())
            .Returns(fakeComment);

        var updatedText = "updated comment";
        var request = new Request
        {
            ElectionRoundId = fakeComment.ElectionRoundId,
            CitizenReportId = fakeComment.CitizenReportId,
            Id = fakeComment.Id,
            UserId = userId,
            Text = updatedText
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _repository
            .Received(1)
            .UpdateAsync(Arg.Is<CitizenReportComment>(x => x.Text == updatedText && x.Id == fakeComment.Id));

        var model = result.Result.As<Ok<CitizenReportCommentModel>>();
        model.Value!.Text.Should().Be(updatedText);
        model.Value.Id.Should().Be(fakeComment.Id);
    }
}
