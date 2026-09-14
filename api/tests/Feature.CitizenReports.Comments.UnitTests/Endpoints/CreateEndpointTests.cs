using System.Security.Claims;
using Feature.CitizenReports.Comments.Create;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.CitizenReportCommentAggregate;

namespace Feature.CitizenReports.Comments.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<CitizenReport> _citizenReportRepository;
    private readonly IRepository<CitizenReportComment> _repository;
    private readonly Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _citizenReportRepository = Substitute.For<IReadRepository<CitizenReport>>();
        _repository = Substitute.For<IRepository<CitizenReportComment>>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService, _citizenReportRepository, _repository);
    }

    [Fact]
    public async Task ShouldAddComment_WhenAuthorized()
    {
        var electionRoundId = Guid.NewGuid();
        var citizenReportId = Guid.NewGuid();
        var text = "a citizen report comment";

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _citizenReportRepository.AnyAsync(Arg.Any<ISpecification<CitizenReport>>(), Arg.Any<CancellationToken>())
            .Returns(true);

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
            Text = "a citizen report comment"
        }, CancellationToken.None);

        result.Result.Should().BeOfType<NotFound>();
        await _repository.DidNotReceive().AddAsync(Arg.Any<CitizenReportComment>());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCitizenReportNotInElectionRound()
    {
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _citizenReportRepository.AnyAsync(Arg.Any<ISpecification<CitizenReport>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _endpoint.ExecuteAsync(new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
            Text = "a citizen report comment"
        }, CancellationToken.None);

        result.Result.Should().BeOfType<NotFound>();
        await _repository.DidNotReceive().AddAsync(Arg.Any<CitizenReportComment>());
    }
}
