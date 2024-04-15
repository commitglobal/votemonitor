using System.Security.Claims;
using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Api.Feature.PollingStation.Notes.List;
using Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<PollingStationNoteAggregate> _repository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly Endpoint _endpoint;

    public ListEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IReadRepository<PollingStationNoteAggregate>>();
        _pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService,
            _repository,
            _pollingStationRepository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModelList_WhenAllIdsExist()
    {
        // Arrange
        var noteId = Guid.NewGuid();
        var noteText = "a polling station note";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakePollingStationNote = new PollingStationNoteFaker(noteId, noteText).Generate();
        var anotherFakePollingStationNote = new PollingStationNoteFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(fakePollingStation);

        var expected = new List<PollingStationNoteAggregate>
        {
            fakePollingStationNote, anotherFakePollingStationNote
        };
        _repository.ListAsync(Arg.Any<GetPollingStationNotesSpecification>())
            .Returns(expected);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<List<NoteModel>>>();
        model.Value.Should().HaveCount(2);
        model.Value!.First().Text.Should().Be(fakePollingStationNote.Text);
        model.Value!.First().Id.Should().Be(fakePollingStationNote.Id);
        model.Value!.Last().Text.Should().Be(anotherFakePollingStationNote.Text);
        model.Value!.Last().Id.Should().Be(anotherFakePollingStationNote.Id);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<List<NoteModel>>, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenPollingStationDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns((PollingStationAggregate)null!);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<List<NoteModel>>, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
