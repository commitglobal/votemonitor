using Feature.Review.Services;

namespace Feature.Review.SetFollowUp;

public class Request : ReportRequest
{
    public Guid FollowUpStatusId { get; set; }
}

public class Validator : ReportRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.FollowUpStatusId).NotEmpty();
    }
}

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    IFollowUpService followUpService,
    VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/follow-ups/{recordType}/{recordId}");
        DontAutoTag();
        Options(x => x.WithTags("follow-up"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Sets a record's follow-up status in the caller's NGO";
            s.Description = "NGO admins and staff can change any record's follow-up, also in archived rounds.";
        });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        if (!await authorizationService.CanWorkOnReportsAsync(User, req.ElectionRoundId, req.RecordType))
        {
            return TypedResults.NotFound();
        }

        var report = await reviewService.ResolveAsync(req.ElectionRoundId, req.NgoId, req.RecordType, req.RecordId, ct);
        var actor = await reviewService.GetActorAsync(req.UserId, ct);
        if (report is null || actor is null)
        {
            return TypedResults.NotFound();
        }

        var status = await context.FollowUpStatuses.FirstOrDefaultAsync(
            x => x.Id == req.FollowUpStatusId && x.MonitoringNgoId == report.Target.MonitoringNgoId, ct);
        if (status is null)
        {
            AddError(x => x.FollowUpStatusId, "Pick one of your NGO's follow-up statuses");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        await followUpService.SetAsync(report, status, actor, ct);
        return TypedResults.NoContent();
    }
}
