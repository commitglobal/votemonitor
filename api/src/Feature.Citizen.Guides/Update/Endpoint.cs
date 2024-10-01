using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Citizen.Guides.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.CitizenGuideAggregate;

namespace Feature.Citizen.Guides.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<CitizenGuideAggregate> repository)
    : Endpoint<Request, Results<Ok<CitizenGuideModel>, NotFound, NoContent>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/citizen-guides/{id}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-guides"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<CitizenGuideModel>, NotFound, NoContent>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var requirement = new CitizenReportingNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetCitizenGuideByIdSpecification(req.ElectionRoundId, req.Id);
        var guide = await repository.FirstOrDefaultAsync(specification, ct);

        if (guide is null)
        {
            return TypedResults.NotFound();
        }

        if (guide.GuideType == CitizenGuideType.Document)
        {
            guide.UpdateTitle(req.Title);
        }
        
        if (guide.GuideType == CitizenGuideType.Text)
        {
            if (string.IsNullOrWhiteSpace(req.Text))
            {
                ThrowError(x => x.Text, "Text is required.");
            }

            guide.UpdateTextGuide(req.Title, req.Text);
        }

        if (guide.GuideType == CitizenGuideType.Website)
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