using Feature.IncidentsReports.Notes.List;
using Feature.IncidentsReports.Notes.Specifications;
using Vote.Monitor.Domain.Entities.IncidentReportNoteAggregate;

namespace Feature.IncidentReports.Notes.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<IncidentReportNote> _repository;
    private readonly Endpoint _endpoint;

    public ListEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<IncidentReportNote>>();
        _endpoint = Factory.Create<Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModelList_WhenAllIdsExist()
    {
        // Arrange
        var fakeNote = new IncidentReportNoteModelFaker().Generate();
        var anotherFakeNote = new IncidentReportNoteModelFaker().Generate();

        _repository
            .ListAsync(Arg.Any<ListNotesSpecification>(), CancellationToken.None)
            .Returns([fakeNote, anotherFakeNote]);

        // Act
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IncidentReportId = Guid.NewGuid(),
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