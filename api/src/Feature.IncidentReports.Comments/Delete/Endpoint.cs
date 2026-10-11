using Feature.IncidentReports.Comments.Specifications;

namespace Feature.IncidentReports.Comments.Delete;

public class Endpoint(
    IRepository<IncidentReportCommentAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/comments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-comments"));
        Summary(s => { s.Summary = "Deletes an incident report comment. Only the author can delete it."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var comment = await repository.FirstOrDefaultAsync(
            new GetCommentByIdSpecification(req.ElectionRoundId, req.IncidentReportId, req.UserId, req.Id), ct);

        if (comment is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(comment, ct);

        return TypedResults.NoContent();
    }
}
