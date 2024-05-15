using System.Security.Claims;
using FastEndpoints;
using Feature.Notes.Get;
using Feature.Notes.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Notes.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<NoteAggregate> _repository;
    private readonly Endpoint _endpoint;

    public GetEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IReadRepository<NoteAggregate>>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService,
            _repository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModel_WhenAllIdsExist()
    {
        // Arrange
        var noteId = Guid.NewGuid();
        var noteText = "a polling station note";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakeNote = new NoteFaker(noteId, noteText).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .Returns(fakeNote);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakeNote.PollingStationId,
            ObserverId = fakeMonitoringObserver.Id,
            Id = noteId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<NoteModel>>();
        model.Value!.Text.Should().Be(noteText);
        model.Value.Id.Should().Be(noteId);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = Guid.NewGuid(),
            ObserverId = fakeMonitoringObserver.Id,
            Id = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NoteModel>, BadRequest<ProblemDetails>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
