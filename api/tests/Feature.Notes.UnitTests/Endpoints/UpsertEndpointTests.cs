using System.Security.Claims;
using FastEndpoints;
using Feature.Notes.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Notes.UnitTests.Endpoints;

public class UpsertEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<NoteAggregate> _repository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly Upsert.Endpoint _endpoint;

    public UpsertEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IRepository<NoteAggregate>>();
        _monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<Upsert.Endpoint>(_authorizationService,
            _monitoringObserverRepository,
            _repository,
            new CurrentUtcTimeProvider());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var noteText = "a polling station note";
        var request = new Upsert.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Text = noteText
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<NoteModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldAddNote_WhenNoteNotFound()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverIdSpecification>())
            .Returns(Guid.NewGuid());

        // Act
        var noteText = "a polling station note";
        var request = new Upsert.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            FormId = Guid.NewGuid(),
            QuestionId = Guid.NewGuid(),
            Text = "a polling station note",
            Id = Guid.NewGuid(),
            LastUpdatedAt = DateTime.UtcNow,
        };
        
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<NoteAggregate>(x => x.Text == noteText
                                                 && x.ElectionRoundId == fakeElectionRound.Id
                                                 && x.PollingStationId == request.PollingStationId
                                                 && x.FormId == request.FormId
                                                 && x.QuestionId == request.QuestionId));

        var model = result.Result.As<Ok<NoteModel>>();
        model.Value!.Text.Should().Be(noteText);
        model.Value.LastUpdatedAt.Should().Be(request.LastUpdatedAt);
        model.Value.Id.Should().Be(request.Id);
    }

    [Fact]
    public async Task ShouldUpdatedNote_WhenAllIdsExist()
    {
        // Arrange
        var noteId = Guid.NewGuid();
        var noteText = "a polling station note";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakeNote = new NoteFaker(noteId, noteText).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .Returns(fakeNote);

        // Act
        var updatedText = "an updated note";
        var request = new Upsert.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakeNote.PollingStationId,
            ObserverId = fakeMonitoringObserver.ObserverId,
            Id = fakeNote.Id,
            Text = updatedText,
            LastUpdatedAt = DateTime.UtcNow
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        var model = result.Result.As<Ok<NoteModel>>();
        model.Value!.Text.Should().Be(updatedText);
        model.Value.Id.Should().Be(noteId);
        model.Value.LastUpdatedAt.Should().Be(request.LastUpdatedAt);
    }
}
