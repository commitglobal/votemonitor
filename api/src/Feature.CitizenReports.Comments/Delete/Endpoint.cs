using Feature.CitizenReports.Comments.Specifications;

namespace Feature.CitizenReports.Comments.Delete;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<CitizenReportCommentAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/comments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-report-comments"));
        Summary(s => { s.Summary = "Deletes a citizen report comment. Only the author can delete it."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new CitizenReportingNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        
        var comment = await repository.FirstOrDefaultAsync(
            new GetCommentByIdSpecification(req.ElectionRoundId, req.CitizenReportId, req.UserId, req.Id), ct);

        if (comment is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(comment, ct);

        return TypedResults.NoContent();
    }
}
