using Feature.QuickReports.Comments.Specifications;

namespace Feature.QuickReports.Comments.Delete;

public class Endpoint(
    IRepository<QuickReportCommentAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/comments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("quick-report-comments"));
        Summary(s => { s.Summary = "Deletes a quick report comment. Only the author can delete it."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var comment = await repository.FirstOrDefaultAsync(
            new GetCommentByIdSpecification(req.ElectionRoundId, req.QuickReportId, req.UserId, req.Id), ct);

        if (comment is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(comment, ct);

        return TypedResults.NoContent();
    }
}
