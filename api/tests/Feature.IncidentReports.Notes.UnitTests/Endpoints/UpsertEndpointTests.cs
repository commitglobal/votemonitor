using Vote.Monitor.Domain.Entities.ReviewAggregate;
using Feature.IncidentReports.Notes.Specifications;
using Feature.IncidentReports.Notes.Upsert;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportNoteAggregate;

namespace Feature.IncidentReports.Notes.UnitTests.Endpoints;

public class UpsertEndpointTests
{
    private readonly IRepository<IncidentReportNote> _repository;
    private readonly IReadRepository<IncidentReport> _incidentReportRepository;
    private readonly Endpoint _endpoint;

    public UpsertEndpointTests()
    {
        _repository = Substitute.For<IRepository<IncidentReportNote>>();
        _incidentReportRepository = Substitute.For<IReadRepository<IncidentReport>>();
        _endpoint = Factory.Create<Endpoint>(_repository, _incidentReportRepository,
            Substitute.For<IRecordAutomation>());
    }

    [Fact]
    public async Task ShouldAddNote_WhenNoteNotFound()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var incidentReportId = Guid.NewGuid();
        var formId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        // Act
        var noteText = "a polling station note";
        var noteId = Guid.NewGuid();

        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            IncidentReportId = incidentReportId,
            FormId = formId,
            QuestionId = questionId,
            Text = noteText,
            Id = noteId
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<IncidentReportNote>(x => x.Text == noteText
                                                     && x.ElectionRoundId == fakeElectionRound.Id
                                                     && x.IncidentReportId == incidentReportId
                                                     && x.FormId == formId
                                                     && x.QuestionId == questionId
                                                     && x.Id == noteId));

        var model = result.Result.As<Ok<IncidentReportNoteModel>>();
        model.Value!.Text.Should().Be(noteText);
        model.Value.UpdatedAt.Should().BeNull();
        model.Value.Id.Should().Be(noteId);
    }

    [Fact]
    public async Task ShouldUpdatedNote_WhenAllIdsExist()
    {
        // Arrange
        var fakeNote = new IncidentReportNoteFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .Returns(fakeNote);

        // Act
        var updatedText = "an updated note";
        var request = new Request
        {
            ElectionRoundId = fakeNote.ElectionRoundId,
            IncidentReportId = fakeNote.IncidentReportId,
            FormId = fakeNote.FormId,
            Id = fakeNote.Id,
            Text = updatedText,
            QuestionId = fakeNote.QuestionId
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .UpdateAsync(Arg.Is<IncidentReportNote>(x => x.Text == updatedText
                                                     && x.ElectionRoundId == fakeNote.ElectionRoundId
                                                     && x.IncidentReportId == fakeNote.IncidentReportId
                                                     && x.FormId == fakeNote.FormId
                                                     && x.QuestionId == fakeNote.QuestionId));

        var model = result.Result.As<Ok<IncidentReportNoteModel>>();
        model.Value!.Text.Should().Be(updatedText);
        model.Value.Id.Should().Be(fakeNote.Id);
    }

    [Fact]
    public async Task ShouldReturnConflict_AndNotChangeNotes_WhenIncidentReportIsCompleted()
    {
        // Arrange
        _incidentReportRepository
            .AnyAsync(Arg.Any<CompletedIncidentReportSpecification>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IncidentReportId = Guid.NewGuid(),
            FormId = Guid.NewGuid(),
            QuestionId = Guid.NewGuid(),
            Text = "a note on a locked report",
            Id = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<Conflict>();
        await _repository.DidNotReceiveWithAnyArgs().AddAsync(default!);
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }
}
