using Authorization.Policies;
using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.CitizenNotificationAggregate;

namespace Feature.Citizen.Notifications.Send;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<CitizenNotification> repository,
    IHtmlStringSanitizer htmlStringSanitizer) :
    Endpoint<Request, Results<Ok<CitizenNotificationModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-notifications:send");
        DontAutoTag();
        Options(x => x.WithTags("citizen-notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<CitizenNotificationModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var requirement = new CitizenReportingNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sanitizedMessage = htmlStringSanitizer.Sanitize(req.Body);

        var notification = CitizenNotification.Create(req.ElectionRoundId,
            req.UserId,
            req.Title,
            sanitizedMessage);

        await repository.AddAsync(notification, ct);
        
        return TypedResults.Ok(CitizenNotificationModel.FromEntity(notification));
    }
}