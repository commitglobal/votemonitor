using Feature.CitizenReports.Comments.Delete;
using Feature.CitizenReports.Comments.Specifications;
using Vote.Monitor.Domain.Entities.CitizenReportCommentAggregate;

namespace Feature.CitizenReports.Comments.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IRepository<CitizenReportComment> _repository;
    private readonly Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _repository = Substitute.For<IRepository<CitizenReportComment>>();
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
