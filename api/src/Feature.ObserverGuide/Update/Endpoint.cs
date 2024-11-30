using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.ObserverGuide.Model;
using Feature.ObserverGuide.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Feature.ObserverGuide.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<ObserverGuideAggregate> repository,
    IHtmlStringSanitizer htmlStringSanitizer)
    : Endpoint<Request, Results<Ok<ObserverGuideModel>, NotFound, NoContent>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/observer-guide/{id}");
        DontAutoTag();
        Options(x => x.WithTags("observer-guide"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<ObserverGuideModel>, NotFound, NoContent>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetObserverGuideByIdSpecification(req.ElectionRoundId, req.NgoId, req.Id);
        var guide = await repository.FirstOrDefaultAsync(specification, ct);

        if (guide is null)
        {
            return TypedResults.NotFound();
        }

        if (guide.GuideType == ObserverGuideType.Document)
        {
            guide.UpdateTitle(req.Title);
        }

        if (guide.GuideType == ObserverGuideType.Text)
        {
            if (string.IsNullOrWhiteSpace(req.Text))
            {
                ThrowError(x => x.Text, "Text is required.");
            }

            guide.UpdateTextGuide(req.Title, htmlStringSanitizer.Sanitize(req.Text));
        }

        if (guide.GuideType == ObserverGuideType.Website)
        {
            if (string.IsNullOrWhiteSpace(req.WebsiteUrl))
            {
                ThrowError(x => x.WebsiteUrl, "Website url is required.");
            }

            guide.UpdateWebsiteGuide(req.Title, new Uri(req.WebsiteUrl));
        }

        await repository.UpdateAsync(guide, ct);

        return TypedResults.NoContent();
    }
}