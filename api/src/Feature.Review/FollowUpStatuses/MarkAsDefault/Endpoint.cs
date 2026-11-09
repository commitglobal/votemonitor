using Feature.Review.Services;

namespace Feature.Review.FollowUpStatuses.MarkAsDefault;

public class Request : FollowUpStatusesRequest
{
    public Guid Id { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
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
        Post("/api/election-rounds/{electionRoundId}/follow-up-statuses/{id}:default");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("follow-up"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Makes a follow-up status the default for reports the NGO hasn't set one for";
            s.Description = "Reports still on the previous default keep it.";
        });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
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

        var previousDefault = statuses.First(x => x.IsDefault);
        if (previousDefault.Id == status.Id)
        {
            return TypedResults.NoContent();
        }

        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(ct);

            await followUpService.MaterializeDefaultAsync(req.ElectionRoundId, req.NgoId, previousDefault, ct);

            // Two saves: the partial unique index allows only one default at a time.
            previousDefault.UnmarkAsDefault();
            await context.SaveChangesAsync(ct);
            status.MarkAsDefault();
            await context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        });

        return TypedResults.NoContent();
    }
}
