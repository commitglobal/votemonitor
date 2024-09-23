using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.CitizenReports.Guides.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.CitizenReportGuideAggregate;

namespace Feature.CitizenReports.Guides.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<CitizenReportGuideAggregate> repository)
    : Endpoint<Request, Results<Ok<CitizenReportsGuideModel>, NotFound, NoContent>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/citizen-reports-guides/{id}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports-guides"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<CitizenReportsGuideModel>, NotFound, NoContent>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var requirement = new CitizenReportingNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetObserverGuideByIdSpecification(req.ElectionRoundId, req.Id);
        var guide = await repository.FirstOrDefaultAsync(specification, ct);

        if (guide is null)
        {
            return TypedResults.NotFound();
        }

        if (guide.GuideType == CitizenReportGuideType.Document)
        {
            guide.UpdateTitle(req.Title);
        }
        
        if (guide.GuideType == CitizenReportGuideType.Text)
        {
            if (string.IsNullOrWhiteSpace(req.Text))
            {
                ThrowError(x => x.Text, "Text is required.");
            }

            guide.UpdateTextGuide(req.Title, req.Text);
        }

        if (guide.GuideType == CitizenReportGuideType.Website)
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