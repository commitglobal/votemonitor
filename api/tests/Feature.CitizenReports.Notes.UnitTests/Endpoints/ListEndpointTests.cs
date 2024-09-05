using FastEndpoints;
using Feature.CitizenReports.Notes.Specifications;
using FluentAssertions;
using NSubstitute;
using Vote.Monitor.Domain.Entities.CitizenReportNoteAggregate;
using Vote.Monitor.Domain.Repository;

namespace Feature.CitizenReports.Notes.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<CitizenReportNote> _repository;
    private readonly List.Endpoint _endpoint;

    public ListEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<CitizenReportNote>>();
        _endpoint = Factory.Create<List.Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModelList_WhenAllIdsExist()
    {
        // Arrange
        var fakeNote = new CitizenReportNoteModelFaker().Generate();
        var anotherFakeNote = new CitizenReportNoteModelFaker().Generate();

        _repository
            .ListAsync(Arg.Any<ListNotesSpecification>(), CancellationToken.None)
            .Returns([fakeNote, anotherFakeNote]);

        // Act
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert

        result.Notes.Should().HaveCount(2);
        result.Notes.First().Text.Should().Be(fakeNote.Text);
        result.Notes.First().Id.Should().Be(fakeNote.Id);
        result.Notes.Last().Text.Should().Be(anotherFakeNote.Text);
        result.Notes.Last().Id.Should().Be(anotherFakeNote.Id);
    }
}