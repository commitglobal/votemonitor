using Feature.QuickReports.Comments.Create;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Domain.Entities.QuickReportCommentAggregate;

namespace Feature.QuickReports.Comments.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IReadRepository<QuickReport> _quickReportRepository;
    private readonly IRepository<QuickReportComment> _repository;
    private readonly Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _quickReportRepository = Substitute.For<IReadRepository<QuickReport>>();
        _repository = Substitute.For<IRepository<QuickReportComment>>();
        _endpoint = Factory.Create<Endpoint>(_quickReportRepository, _repository);
    }

    [Fact]
    public async Task ShouldAddComment_WhenAuthorized()
    {
        var electionRoundId = Guid.NewGuid();
        var quickReportId = Guid.NewGuid();
        var text = "a quick report comment";

        _quickReportRepository.AnyAsync(Arg.Any<ISpecification<QuickReport>>(), Arg.Any<CancellationToken>())
            .Returns(true);

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

    [Fact]
    public async Task ShouldReturnNotFound_WhenQuickReportNotFromSameNgo()
    {
        _quickReportRepository.AnyAsync(Arg.Any<ISpecification<QuickReport>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _endpoint.ExecuteAsync(new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            QuickReportId = Guid.NewGuid(),
            Text = "a quick report comment"
        }, CancellationToken.None);

        result.Result.Should().BeOfType<NotFound>();
        await _repository.DidNotReceive().AddAsync(Arg.Any<QuickReportComment>());
    }
}
