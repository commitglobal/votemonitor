using System.Security.Claims;
using FastEndpoints;
using Feature.Notes.List;
using Feature.Notes.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Notes.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<NoteAggregate> _repository;
    private readonly Endpoint _endpoint;

    public ListEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IReadRepository<NoteAggregate>>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService, _repository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModelList_WhenAllIdsExist()
    {
        // Arrange

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakeNote = new NoteFaker().Generate();
        var anotherFakeNote = new NoteFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        var expected = new List<NoteAggregate>
        {
            fakeNote, anotherFakeNote
        };
        _repository.ListAsync(Arg.Any<GetNotesSpecification>())
            .Returns(expected);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = Guid.NewGuid(),
            FormId = Guid.NewGuid(),
            ObserverId = fakeMonitoringObserver.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<List<NoteModel>>>();
        model.Value.Should().HaveCount(2);
        model.Value!.First().Text.Should().Be(fakeNote.Text);
        model.Value!.First().Id.Should().Be(fakeNote.Id);
        model.Value!.Last().Text.Should().Be(anotherFakeNote.Text);
        model.Value!.Last().Id.Should().Be(anotherFakeNote.Id);
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
            ObserverId = fakeMonitoringObserver.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<List<NoteModel>>, NotFound, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
