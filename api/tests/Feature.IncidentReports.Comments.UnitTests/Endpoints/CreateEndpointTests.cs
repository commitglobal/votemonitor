using Feature.IncidentReports.Comments.Create;
using Vote.Monitor.Domain.Entities.IncidentReportCommentAggregate;

namespace Feature.IncidentReports.Comments.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IRepository<IncidentReportComment> _repository;
    private readonly Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _repository = Substitute.For<IRepository<IncidentReportComment>>();
        _endpoint = Factory.Create<Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldAddComment_WhenAuthorized()
    {
        var electionRoundId = Guid.NewGuid();
        var incidentReportId = Guid.NewGuid();
        var text = "an incident report comment";

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            IncidentReportId = incidentReportId,
            Text = text
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _repository
            .Received(1)
            .AddAsync(Arg.Is<IncidentReportComment>(x => x.Text == text
                                                         && x.ElectionRoundId == electionRoundId
                                                         && x.IncidentReportId == incidentReportId));

        var model = result.Result.As<Ok<IncidentReportCommentModel>>();
        model.Value!.Text.Should().Be(text);
        model.Value.IncidentReportId.Should().Be(incidentReportId);
    }
}
