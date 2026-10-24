using Feature.IncidentReports.Notes.Specifications;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.IncidentReports.Notes.Upsert;

public class Endpoint(
    IRepository<IncidentReportNoteAggregate> repository,
    IRecordAutomation recordAutomation)
    : Endpoint<Request, Results<Ok<IncidentReportNoteModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-notes"));
        Summary(s => { s.Summary = "Upserts a note for a incident report"; });
        
    }

    public override async Task<Results<Ok<IncidentReportNoteModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var note = await repository.FirstOrDefaultAsync(new GetNoteByIdSpecification(req.ElectionRoundId, req.Id), ct);
        return note == null ? await AddNoteAsync(req, ct) : await UpdateNoteAsync(note, req, ct);
    }

    private async Task<Results<Ok<IncidentReportNoteModel>, NotFound>> UpdateNoteAsync(IncidentReportNoteAggregate note,
        Request req, CancellationToken ct)
    {
        var isChanged = note.Text != req.Text;
        note.UpdateText(req.Text);
        await repository.UpdateAsync(note, ct);
        if (isChanged)
        {
            await recordAutomation.RecordChangedByAuthorAsync(req.ElectionRoundId, ReportRecordType.IncidentReport,
                req.IncidentReportId, ct);
        }

        return TypedResults.Ok(IncidentReportNoteModel.FromEntity(note));
    }

    private async Task<Results<Ok<IncidentReportNoteModel>, NotFound>> AddNoteAsync(Request req, CancellationToken ct)
    {
        var note = new IncidentReportNoteAggregate(req.Id,
            req.ElectionRoundId,
            req.IncidentReportId,
            req.FormId,
            req.QuestionId,
            req.Text);

        await repository.AddAsync(note, ct);
        await recordAutomation.RecordChangedByAuthorAsync(req.ElectionRoundId, ReportRecordType.IncidentReport,
            req.IncidentReportId, ct);

        return TypedResults.Ok(IncidentReportNoteModel.FromEntity(note));
    }
}