using System.Security.Claims;
using FastEndpoints;
using Feature.Notes.Delete;
using Feature.Notes.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Notes.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<NoteAggregate> _repository;
    private readonly Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IRepository<NoteAggregate>>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService,
            _repository);
    }

    [Fact]
    public async Task ShouldReturnNoContent_WhenAllIdsExist()
    {
        // Arrange
        var noteId = Guid.NewGuid();
        var noteText = "a polling station note";

        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakeNote = new NoteFaker(noteId, noteText, monitoringObserver: fakeMonitoringObserver).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .Returns(fakeNote);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeNote.ElectionRoundId,
            ObserverId = fakeMonitoringObserver.ObserverId,
            Id = fakeNote.Id,
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NoContent>();

        await _repository.Received(1).DeleteAsync(fakeNote);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenDeletingNoteMadeByOtherObserver()
    {
        // Arrange
        var noteId = Guid.NewGuid();
        var noteText = "a polling station note";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeNote = new NoteFaker(noteId, noteText).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.GetByIdAsync(noteId)
            .Returns(fakeNote);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
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
            ObserverId = fakeMonitoringObserver.Id,
            Id = Guid.NewGuid(),
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
