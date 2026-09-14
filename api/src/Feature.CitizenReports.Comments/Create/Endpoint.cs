namespace Feature.CitizenReports.Comments.Create;

public class Endpoint(
    IRepository<CitizenReportCommentAggregate> repository)
    : Endpoint<Request, Results<Ok<CitizenReportCommentModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("citizen-report-comments"));
        Summary(s => { s.Summary = "Creates a comment for a citizen report."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<CitizenReportCommentModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        // todo: check if citizen report belongs to the NGO responsible for citizen reporting
        var comment = CitizenReportCommentAggregate.Create(
            req.ElectionRoundId,
            req.CitizenReportId,
            req.Text);

        await repository.AddAsync(comment, ct);

        return TypedResults.Ok(CitizenReportCommentModel.FromEntity(comment));
    }
}
