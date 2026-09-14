using Feature.IncidentReports.Comments.Specifications;
using Feature.IncidentReports.Comments.Update;
using Vote.Monitor.Domain.Entities.IncidentReportCommentAggregate;

namespace Feature.IncidentReports.Comments.UnitTests.Endpoints;

public class UpdateEndpointTests
{
    private readonly IRepository<IncidentReportComment> _repository;
    private readonly Endpoint _endpoint;

    public UpdateEndpointTests()
    {
        _repository = Substitute.For<IRepository<IncidentReportComment>>();
        _endpoint = Factory.Create<Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCommentDoesNotExist()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<GetCommentByIdSpecification>())
            .ReturnsNull();

        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IncidentReportId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Text = "updated"
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result
            .Should().BeOfType<Results<Ok<IncidentReportCommentModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldUpdateComment_WhenUserIsTheAuthor()
    {
        var userId = Guid.NewGuid();
        var fakeComment = new IncidentReportCommentFaker(authorId: userId).Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetCommentByIdSpecification>())
            .Returns(fakeComment);

        var updatedText = "updated comment";
        var request = new Request
        {
            ElectionRoundId = fakeComment.ElectionRoundId,
            IncidentReportId = fakeComment.IncidentReportId,
            Id = fakeComment.Id,
            UserId = userId,
            Text = updatedText
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _repository
            .Received(1)
            .UpdateAsync(Arg.Is<IncidentReportComment>(x => x.Text == updatedText && x.Id == fakeComment.Id));

        var model = result.Result.As<Ok<IncidentReportCommentModel>>();
        model.Value!.Text.Should().Be(updatedText);
        model.Value.Id.Should().Be(fakeComment.Id);
    }
}
