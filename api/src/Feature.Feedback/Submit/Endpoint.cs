using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Feedback.Submit;

public class Endpoint(IAuthorizationService authorizationService,
    IRepository<FeedbackAggregate> repository,
    ITimeProvider timeProvider) : Endpoint<Request, Results<NotFound, NoContent>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/feedback/");
        DontAutoTag();
        Options(x => x.WithTags("feedback"));
        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<NotFound, NoContent>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var feedback = new FeedbackAggregate(req.ElectionRoundId,
            req.ObserverId, req.UserFeedback, timeProvider.UtcNow, req.Metadata);

        await repository.AddAsync(feedback, ct);

        return TypedResults.NoContent();
    }
}
