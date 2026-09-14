using Feature.CitizenReports.Comments.Specifications;

namespace Feature.CitizenReports.Comments.Update;

public class Endpoint(
    IRepository<CitizenReportCommentAggregate> repository)
    : Endpoint<Request, Results<Ok<CitizenReportCommentModel>, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/comments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-report-comments"));
        Summary(s => { s.Summary = "Updates a citizen report comment. Only the author can update it."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<CitizenReportCommentModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var comment = await repository.FirstOrDefaultAsync(
            new GetCommentByIdSpecification(req.ElectionRoundId, req.CitizenReportId, req.UserId, req.Id), ct);

        if (comment is null)
        {
            return TypedResults.NotFound();
        }

        comment.UpdateText(req.Text);
        await repository.UpdateAsync(comment, ct);

        return TypedResults.Ok(CitizenReportCommentModel.FromEntity(comment));
    }
}
