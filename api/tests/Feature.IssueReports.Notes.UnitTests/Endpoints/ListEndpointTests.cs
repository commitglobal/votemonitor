namespace Feature.IssueReports.Notes.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<IssueReportNote> _repository;
    private readonly List.Endpoint _endpoint;

    public ListEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<IssueReportNote>>();
        _endpoint = Factory.Create<List.Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModelList_WhenAllIdsExist()
    {
        // Arrange
        var fakeNote = new IssueReportNoteModelFaker().Generate();
        var anotherFakeNote = new IssueReportNoteModelFaker().Generate();

        _repository
            .ListAsync(Arg.Any<ListNotesSpecification>(), CancellationToken.None)
            .Returns([fakeNote, anotherFakeNote]);

        // Act
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IssueReportId = Guid.NewGuid(),
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