using Feature.QuickReports.Comments.Specifications;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Comments.Create;

public class Endpoint(
    IReadRepository<QuickReport> quickReportRepository,
    IRepository<QuickReportCommentAggregate> repository)
    : Endpoint<Request, Results<Ok<QuickReportCommentModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("quick-report-comments"));
        Summary(s => { s.Summary = "Creates a comment for a quick report."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<QuickReportCommentModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var quickReportExists = await quickReportRepository.AnyAsync(
            new GetQuickReportForNgoSpecification(req.ElectionRoundId, req.NgoId, req.QuickReportId), ct);

        if (!quickReportExists)
        {
            return TypedResults.NotFound();
        }

        var comment = QuickReportCommentAggregate.Create(
            req.ElectionRoundId,
            req.QuickReportId,
            req.Text);

        await repository.AddAsync(comment, ct);

        return TypedResults.Ok(QuickReportCommentModel.FromEntity(comment));
    }
}
