using Feature.CitizenReports.ListFormsOverview;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports.ListFormsOverviewV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<AggregatedFormOverview>>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports:byFormV2");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);

        Summary(x => { x.Summary = "Citizen report submissions aggregated by form (v2)"; });
    }

    public override async Task<Results<Ok<PagedResponse<AggregatedFormOverview>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new CitizenReportingNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var (aggregatedFormOverviews, totalRowCount) = await CitizenReportFilterQuery.GetAggregatedFormsAsync(
            dbConnectionFactory,
            req.ElectionRoundId,
            req.NgoId,
            req.FilterConditions,
            req.SortColumnName,
            req.IsAscendingSorting,
            req.PageNumber,
            req.PageSize,
            ct);

        return TypedResults.Ok(
            new PagedResponse<AggregatedFormOverview>(aggregatedFormOverviews, totalRowCount, req.PageNumber,
                req.PageSize));
    }
}
