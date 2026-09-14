using Feature.IncidentReports.Comments.Specifications;

namespace Feature.IncidentReports.Comments.Update;

public class Endpoint(
    IRepository<IncidentReportCommentAggregate> repository)
    : Endpoint<Request, Results<Ok<IncidentReportCommentModel>, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/comments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-comments"));
        Summary(s => { s.Summary = "Updates an incident report comment. Only the author can update it."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<IncidentReportCommentModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var comment = await repository.FirstOrDefaultAsync(
            new GetCommentByIdSpecification(req.ElectionRoundId, req.IncidentReportId, req.UserId, req.Id), ct);

        if (comment is null)
        {
            return TypedResults.NotFound();
        }

        comment.UpdateText(req.Text);
        await repository.UpdateAsync(comment, ct);

        return TypedResults.Ok(IncidentReportCommentModel.FromEntity(comment));
    }
}
