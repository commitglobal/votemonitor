using Feature.CitizenReports.Comments.Specifications;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Feature.CitizenReports.Comments.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<CitizenReport> citizenReportRepository,
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
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new CitizenReportingNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var citizenReportExists = await citizenReportRepository.AnyAsync(
            new GetCitizenReportInElectionRoundSpecification(req.ElectionRoundId, req.CitizenReportId), ct);

        if (!citizenReportExists)
        {
            return TypedResults.NotFound();
        }

        var comment = CitizenReportCommentAggregate.Create(
            req.ElectionRoundId,
            req.CitizenReportId,
            req.Text);

        await repository.AddAsync(comment, ct);

        return TypedResults.Ok(CitizenReportCommentModel.FromEntity(comment));
    }
}
