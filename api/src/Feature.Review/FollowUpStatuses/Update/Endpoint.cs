using Feature.Review.Services;

namespace Feature.Review.FollowUpStatuses.Update;

public class Request : FollowUpStatusBody
{
    public Guid Id { get; set; }

    /// <summary>The position; when left out it stays where it is.</summary>
    public double? Sequence { get; set; }
}

public class Validator : FollowUpStatusBodyValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class Endpoint(IAuthorizationService authorizationService, IReviewService reviewService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<FollowUpStatusModel>, NotFound, ValidationProblem, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/follow-up-statuses/{id}");
        DontAutoTag();
        Options(x => x.WithTags("follow-up"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s => { s.Summary = "Renames, describes, recolours or moves a follow-up status (replaces the whole status)"; });
    }

    public override async Task<Results<Ok<FollowUpStatusModel>, NotFound, ValidationProblem, Conflict<ProblemDetails>>>
        ExecuteAsync(Request req, CancellationToken ct)
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
        if (status is null)
        {
            return TypedResults.NotFound();
        }

        if (await reviewService.IsArchivedAsync(req.ElectionRoundId, ct))
        {
            AddError(x => x.ElectionRoundId, "Election round is archived");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var name = req.Name.Trim();
        if (statuses.Any(x => x.Id != status.Id && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            AddError(x => x.Name, "A follow-up status with this name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures, StatusCodes.Status409Conflict));
        }

        status.Update(name, req.Description?.Trim(), req.Sequence ?? status.Sequence, req.Color, req.Icon);
        await context.SaveChangesAsync(ct);

        return TypedResults.Ok(FollowUpStatusModel.FromEntity(status));
    }
}
