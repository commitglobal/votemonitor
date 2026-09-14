using Feature.QuickReports.Comments.Create;
using Vote.Monitor.Domain.Entities.QuickReportCommentAggregate;

namespace Feature.QuickReports.Comments.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IRepository<QuickReportComment> _repository;
    private readonly Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _repository = Substitute.For<IRepository<QuickReportComment>>();
        _endpoint = Factory.Create<Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldAddComment_WhenAuthorized()
    {
        var electionRoundId = Guid.NewGuid();
        var quickReportId = Guid.NewGuid();
        var text = "a quick report comment";

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            QuickReportId = quickReportId,
            Text = text
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _repository
            .Received(1)
            .AddAsync(Arg.Is<QuickReportComment>(x => x.Text == text
                                                      && x.ElectionRoundId == electionRoundId
                                                      && x.QuickReportId == quickReportId));

        var model = result.Result.As<Ok<QuickReportCommentModel>>();
        model.Value!.Text.Should().Be(text);
        model.Value.QuickReportId.Should().Be(quickReportId);
    }
}
