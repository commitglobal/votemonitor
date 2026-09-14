using Feature.IncidentReports.Comments.Create;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportCommentAggregate;

namespace Feature.IncidentReports.Comments.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IReadRepository<IncidentReport> _incidentReportRepository;
    private readonly IRepository<IncidentReportComment> _repository;
    private readonly Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _incidentReportRepository = Substitute.For<IReadRepository<IncidentReport>>();
        _repository = Substitute.For<IRepository<IncidentReportComment>>();
        _endpoint = Factory.Create<Endpoint>(_incidentReportRepository, _repository);
    }

    [Fact]
    public async Task ShouldAddComment_WhenAuthorized()
    {
        var electionRoundId = Guid.NewGuid();
        var incidentReportId = Guid.NewGuid();
        var text = "an incident report comment";

        _incidentReportRepository.AnyAsync(Arg.Any<ISpecification<IncidentReport>>(), Arg.Any<CancellationToken>())
            .Returns(true);

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

    [Fact]
    public async Task ShouldReturnNotFound_WhenIncidentReportNotFromSameNgo()
    {
        _incidentReportRepository.AnyAsync(Arg.Any<ISpecification<IncidentReport>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _endpoint.ExecuteAsync(new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IncidentReportId = Guid.NewGuid(),
            Text = "an incident report comment"
        }, CancellationToken.None);

        result.Result.Should().BeOfType<NotFound>();
        await _repository.DidNotReceive().AddAsync(Arg.Any<IncidentReportComment>());
    }
}
