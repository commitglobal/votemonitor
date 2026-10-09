using Feature.CitizenReports.ListFormsOverview;
using Vote.Monitor.Domain.Specifications;

namespace Feature.CitizenReports.ListFormsOverviewV2;

public class Endpoint(VoteMonitorContext context, IAuthorizationService authorizationService)
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

        var query = context
            .CitizenReports
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.ElectionRound.CitizenReportingEnabled
                        && x.ElectionRound.MonitoringNgoForCitizenReporting.NgoId == req.NgoId)
            .GroupBy(cr => new { cr.FormId, cr.Form.Code, cr.Form.Name, cr.Form.DefaultLanguage })
            .Select(cr => new AggregatedFormOverview
            {
                FormId = cr.Key.FormId,
                FormCode = cr.Key.Code,
                FormName = cr.Key.Name,
                FormDefaultLanguage = cr.Key.DefaultLanguage,
                NumberOfNotes = cr.Sum(x => x.Notes.Count),
                NumberOfMediaFiles = cr.Sum(x => x.Attachments.Count),
                NumberOfSubmissions = cr.Count(),
                NumberOfFlaggedAnswers = cr.Sum(x => x.NumberOfFlaggedAnswers)
            });

        var totalRowCount = await query.CountAsync(ct);

        query = ApplySorting(query, req.SortColumnName, req.IsAscendingSorting);

        var aggregatedFormOverviews = await query
            .Skip(PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber))
            .Take(req.PageSize)
            .ToListAsync(ct);

        return TypedResults.Ok(
            new PagedResponse<AggregatedFormOverview>(aggregatedFormOverviews, totalRowCount, req.PageNumber,
                req.PageSize));
    }

    private static IQueryable<AggregatedFormOverview> ApplySorting(IQueryable<AggregatedFormOverview> query,
        string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return query.OrderBy(x => x.FormCode);
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting ? query.OrderBy(x => x.FormCode) : query.OrderByDescending(x => x.FormCode);
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfSubmissions),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? query.OrderBy(x => x.NumberOfSubmissions)
                : query.OrderByDescending(x => x.NumberOfSubmissions);
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? query.OrderBy(x => x.NumberOfFlaggedAnswers)
                : query.OrderByDescending(x => x.NumberOfFlaggedAnswers);
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfNotes),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? query.OrderBy(x => x.NumberOfNotes)
                : query.OrderByDescending(x => x.NumberOfNotes);
        }

        if (string.Equals(sortColumnName, nameof(AggregatedFormOverview.NumberOfMediaFiles),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscendingSorting
                ? query.OrderBy(x => x.NumberOfMediaFiles)
                : query.OrderByDescending(x => x.NumberOfMediaFiles);
        }

        return query.OrderBy(x => x.FormCode);
    }
}
