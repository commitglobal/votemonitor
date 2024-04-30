using System.Security.Claims;
using FastEndpoints;
using Feature.Notes.Create;
using Feature.Notes.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Notes.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<NoteAggregate> _repository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IRepository<NoteAggregate>>();
        _monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService,
            _repository,
            _monitoringObserverRepository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModel_WhenAllIdsExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var pollingStationId = Guid.NewGuid();
        var formId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(fakeMonitoringObserver);

        // Act
        var noteText = "a polling station note";
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = pollingStationId,
            ObserverId = fakeMonitoringObserver.Id,
            FormId = formId,
            QuestionId = questionId,
            Text = noteText
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<NoteAggregate>(x => x.Text == noteText
                                                               && x.ElectionRoundId == fakeElectionRound.Id
                                                               && x.PollingStationId == pollingStationId
                                                               && x.FormId == formId
                                                               && x.QuestionId == questionId));

        var model = result.Result.As<Ok<NoteModel>>();
        model.Value!.Text.Should().Be(noteText);
        model.Value.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var pollingStationId = Guid.NewGuid();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var noteText = "a polling station note";
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = pollingStationId,
            ObserverId = fakeMonitoringObserver.Id,
            Text = noteText
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NoteModel>, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
    [Fact]
    public async Task ShouldReturnBadRequest_WhenMonitoringObserverDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var pollingStationId = Guid.NewGuid();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns((MonitoringObserver)null!);

        // Act
        var noteText = "a polling station note";
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = pollingStationId,
            ObserverId = fakeMonitoringObserver.Id,
            Text = noteText
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NoteModel>, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
