using Feature.CitizenReports.Comments.Create;
using Vote.Monitor.Domain.Entities.CitizenReportCommentAggregate;

namespace Feature.CitizenReports.Comments.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IRepository<CitizenReportComment> _repository;
    private readonly Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _repository = Substitute.For<IRepository<CitizenReportComment>>();
        _endpoint = Factory.Create<Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldAddComment_WhenAuthorized()
    {
        var electionRoundId = Guid.NewGuid();
        var citizenReportId = Guid.NewGuid();
        var text = "a citizen report comment";

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            CitizenReportId = citizenReportId,
            Text = text
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _repository
            .Received(1)
            .AddAsync(Arg.Is<CitizenReportComment>(x => x.Text == text
                                                        && x.ElectionRoundId == electionRoundId
                                                        && x.CitizenReportId == citizenReportId));

        var model = result.Result.As<Ok<CitizenReportCommentModel>>();
        model.Value!.Text.Should().Be(text);
        model.Value.CitizenReportId.Should().Be(citizenReportId);
    }
}
