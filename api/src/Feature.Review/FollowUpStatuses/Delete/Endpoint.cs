using Feature.Review.Services;

namespace Feature.Review.FollowUpStatuses.Delete;

public class Request : FollowUpStatusesRequest
{
    public Guid Id { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    /// <summary>Where the reports on the deleted status go. Required when it is in use.</summary>
    [QueryParam]
    public Guid? MoveTo { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.MoveTo).NotEqual(x => x.Id).When(x => x.MoveTo is not null);
    }
}

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    VoteMonitorContext context,
    ITimeProvider timeProvider)
    : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/follow-up-statuses/{id}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("follow-up"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Deletes a follow-up status, moving its reports to another one";
            s.Description = "The default can't be deleted. A status in use needs moveTo; each moved report gets a history entry.";
        });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem, Conflict<ProblemDetails>>> ExecuteAsync(
        Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        var statuses = await context.FollowUpStatuses.Where(x => x.MonitoringNgoId == monitoringNgoId).ToListAsync(ct);
        var status = statuses.FirstOrDefault(x => x.Id == req.Id);
        var actor = await reviewService.GetActorAsync(req.UserId, ct);
        if (status is null || actor is null)
        {
            return TypedResults.NotFound();
        }

        if (await reviewService.IsArchivedAsync(req.ElectionRoundId, ct))
        {
            AddError(x => x.ElectionRoundId, "Election round is archived");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        if (status.IsDefault)
        {
            AddError(x => x.Id, "The default follow-up status can't be deleted; make another one the default first");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures, StatusCodes.Status409Conflict));
        }

        var rows = await context.RecordFollowUps.Where(x => x.FollowUpStatusId == status.Id).ToListAsync(ct);
        var target = statuses.FirstOrDefault(x => x.Id == req.MoveTo);
        if (rows.Count > 0 && target is null)
        {
            AddError(x => x.MoveTo, "The status is in use; choose another of your statuses to move its reports to");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures, StatusCodes.Status409Conflict));
        }

        var now = timeProvider.UtcNow;
        foreach (var row in rows)
        {
            row.Change(target!, actor.Id, now);
            context.ReviewActivities.Add(ReviewActivity.FollowUpStatusChanged(
                new ReviewTarget(row.ElectionRoundId, row.MonitoringNgoId, row.RecordType, row.RecordId),
                actor,
                target!.Name,
                now));
        }

        await context.SaveChangesAsync(ct);
        context.FollowUpStatuses.Remove(status);
        await context.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
