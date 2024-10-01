namespace Feature.IssueReports.Notes.UnitTests.Endpoints;

public class UpsertEndpointTests
{
    private readonly IRepository<IssueReportNote> _repository;
    private readonly Upsert.Endpoint _endpoint;

    public UpsertEndpointTests()
    {
        _repository = Substitute.For<IRepository<IssueReportNote>>();
        _endpoint = Factory.Create<Upsert.Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldAddNote_WhenNoteNotFound()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var issueReportId = Guid.NewGuid();
        var formId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        // Act
        var noteText = "a polling station note";
        var noteId = Guid.NewGuid();

        var request = new Upsert.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            IssueReportId = issueReportId,
            FormId = formId,
            QuestionId = questionId,
            Text = noteText,
            Id = noteId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<IssueReportNote>(x => x.Text == noteText
                                                     && x.ElectionRoundId == fakeElectionRound.Id
                                                     && x.IssueReportId == issueReportId
                                                     && x.FormId == formId
                                                     && x.QuestionId == questionId
                                                     && x.Id == noteId));

        var model = result.Result.As<Ok<IssueReportNoteModel>>();
        model.Value!.Text.Should().Be(noteText);
        model.Value.UpdatedAt.Should().BeNull();
        model.Value.Id.Should().Be(noteId);
    }

    [Fact]
    public async Task ShouldUpdatedNote_WhenAllIdsExist()
    {
        // Arrange
        var fakeNote = new IssueReportNoteFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .Returns(fakeNote);

        // Act
        var updatedText = "an updated note";
        var request = new Upsert.Request
        {
            ElectionRoundId = fakeNote.ElectionRoundId,
            IssueReportId = fakeNote.IssueReportId,
            FormId = fakeNote.FormId,
            Id = fakeNote.Id,
            Text = updatedText,
            QuestionId = fakeNote.QuestionId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(1)
            .UpdateAsync(Arg.Is<IssueReportNote>(x => x.Text == updatedText
                                                     && x.ElectionRoundId == fakeNote.ElectionRoundId
                                                     && x.IssueReportId == fakeNote.IssueReportId
                                                     && x.FormId == fakeNote.FormId
                                                     && x.QuestionId == fakeNote.QuestionId));

        var model = result.Result.As<Ok<IssueReportNoteModel>>();
        model.Value!.Text.Should().Be(updatedText);
        model.Value.Id.Should().Be(fakeNote.Id);
    }
}