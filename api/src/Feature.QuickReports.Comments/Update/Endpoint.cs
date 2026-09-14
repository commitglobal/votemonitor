using Feature.QuickReports.Comments.Specifications;

namespace Feature.QuickReports.Comments.Update;

public class Endpoint(
    IRepository<QuickReportCommentAggregate> repository)
    : Endpoint<Request, Results<Ok<QuickReportCommentModel>, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/comments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("quick-report-comments"));
        Summary(s => { s.Summary = "Updates a quick report comment. Only the author can update it."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<QuickReportCommentModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var comment = await repository.FirstOrDefaultAsync(
            new GetCommentByIdSpecification(req.ElectionRoundId, req.QuickReportId, req.UserId, req.Id), ct);

        if (comment is null)
        {
            return TypedResults.NotFound();
        }

        comment.UpdateText(req.Text);
        await repository.UpdateAsync(comment, ct);

        return TypedResults.Ok(QuickReportCommentModel.FromEntity(comment));
    }
}
