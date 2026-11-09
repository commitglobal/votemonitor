using Feature.Review.Services;
using Vote.Monitor.Domain.Entities.FollowUpAggregate;

namespace Feature.Review.FollowUpStatuses.Create;

public class Request : FollowUpStatusBody;

public class Validator : FollowUpStatusBodyValidator<Request>;

public class Endpoint(IAuthorizationService authorizationService, IReviewService reviewService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<FollowUpStatusModel>, NotFound, ValidationProblem, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/follow-up-statuses");
        DontAutoTag();
        Options(x => x.WithTags("follow-up"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s => { s.Summary = "Adds a follow-up status at the end"; });
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
        if (monitoringNgoId is null)
        {
            return TypedResults.NotFound();
        }

        if (await reviewService.IsArchivedAsync(req.ElectionRoundId, ct))
        {
            AddError(x => x.ElectionRoundId, "Election round is archived");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var statuses = await context.FollowUpStatuses.Where(x => x.MonitoringNgoId == monitoringNgoId).ToListAsync(ct);
        var name = req.Name.Trim();
        if (statuses.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            AddError(x => x.Name, "A follow-up status with this name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures, StatusCodes.Status409Conflict));
        }

        var sequence = statuses.Select(x => x.Sequence).DefaultIfEmpty(0).Max() + FollowUpStatusDefinition.SequenceStep;
        var status = FollowUpStatusDefinition.Create(monitoringNgoId.Value, name, req.Description?.Trim(), sequence, req.Color, req.Icon);
        context.FollowUpStatuses.Add(status);
        await context.SaveChangesAsync(ct);

        return TypedResults.Ok(FollowUpStatusModel.FromEntity(status));
    }
}
