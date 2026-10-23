using Feature.Review.Stages;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

namespace Feature.Review.Metrics;

/// <summary>
/// How many of the records the caller's NGO can see sit in each of its review stages and severities, per record type
/// and optionally per place. Records the NGO never touched count in its default stage with severity None.
/// </summary>
public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<ReviewMetricsModel>, NotFound>>
{
    private static readonly SeverityLevel[] SeverityOrder =
        SeverityLevel.List.OrderByDescending(x => x.Level).ToArray();

    private static readonly ReportRecordType[] MonitoringRecordTypes =
        [ReportRecordType.FormSubmission, ReportRecordType.QuickReport, ReportRecordType.IncidentReport];

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/reviews:metrics");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Counts the caller's NGO's records per review stage and severity, per record type and place";
            s.Description =
                "Counts the records the report lists show for the data source. Every stage (in workflow order) and " +
                "severity (Urgent to None) is listed, with zero counts. Citizen reports are included only when the " +
                "NGO does citizen reporting in the round. With level, places lists one entry per level path (the " +
                "records without a place at that level in one entry with all levels null), largest first.";
        });
    }

    public override async Task<Results<Ok<ReviewMetricsModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorization = await authorizationService.AuthorizeAsync(User, null,
            new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorization.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var stages = (await context.ReviewStages
                .AsNoTracking()
                .Where(x => x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId &&
                            x.MonitoringNgo.NgoId == req.NgoId)
                .ToListAsync(ct))
            .InReviewOrder()
            .ToArray();
        var defaultStage = stages.FirstOrDefault(x => x.IsDefault);
        if (defaultStage is null)
        {
            return TypedResults.NotFound();
        }

        var withCitizenReports = await context.ElectionRounds
            .AnyAsync(x => x.Id == req.ElectionRoundId
                           && x.CitizenReportingEnabled
                           && x.MonitoringNgoForCitizenReporting != null
                           && x.MonitoringNgoForCitizenReporting.NgoId == req.NgoId, ct);
        ReportRecordType[] recordTypes = withCitizenReports
            ? [..MonitoringRecordTypes, ReportRecordType.CitizenReport]
            : MonitoringRecordTypes;

        const string sql = """
                           SELECT * FROM "GetReviewMetrics"(@electionRoundId, @ngoId, @dataSource, @level)
                           """;

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        var groups = (await dbConnection.QueryAsync<MetricsGroup>(new CommandDefinition(sql, new
            {
                electionRoundId = req.ElectionRoundId,
                ngoId = req.NgoId,
                dataSource = req.DataSource.ToString(),
                level = req.Level
            }, cancellationToken: ct)))
            .ToList();

        var stageIds = stages.Select(x => x.Id).ToArray();
        var result = recordTypes.Select(recordType =>
        {
            var ofType = groups.Where(x => x.RecordType == recordType.Value).ToList();
            var places = req.Level is null
                ? null
                : ofType
                    .GroupBy(x => (x.Level1, x.Level2, x.Level3, x.Level4, x.Level5))
                    .Select(place => new PlaceMetricsModel(place.Key.Level1,
                        place.Key.Level2,
                        place.Key.Level3,
                        place.Key.Level4,
                        place.Key.Level5,
                        place.Sum(x => x.Count),
                        CountStages(stageIds, place),
                        CountSeverities(place)))
                    .OrderByDescending(x => x.Total)
                    .ThenBy(x => x.Level1 is null)
                    .ThenBy(x => x.Level1, StringComparer.Ordinal)
                    .ThenBy(x => x.Level2, StringComparer.Ordinal)
                    .ThenBy(x => x.Level3, StringComparer.Ordinal)
                    .ThenBy(x => x.Level4, StringComparer.Ordinal)
                    .ThenBy(x => x.Level5, StringComparer.Ordinal)
                    .ToArray();

            return new RecordTypeMetricsModel(recordType,
                ofType.Sum(x => x.Count),
                CountStages(stageIds, ofType),
                CountSeverities(ofType),
                places);
        }).ToArray();

        return TypedResults.Ok(new ReviewMetricsModel(result));
    }

    private static StageCountModel[] CountStages(Guid[] stageIds, IEnumerable<MetricsGroup> groups)
    {
        var counts = groups.GroupBy(x => x.StageId).ToDictionary(x => x.Key, x => x.Sum(g => g.Count));
        return stageIds.Select(id => new StageCountModel(id, counts.GetValueOrDefault(id))).ToArray();
    }

    private static SeverityCountModel[] CountSeverities(IEnumerable<MetricsGroup> groups)
    {
        var counts = groups.GroupBy(x => x.Severity).ToDictionary(x => x.Key, x => x.Sum(g => g.Count));
        return SeverityOrder.Select(x => new SeverityCountModel(x.Value, counts.GetValueOrDefault(x.Value))).ToArray();
    }

    private sealed class MetricsGroup
    {
        public string RecordType { get; init; } = null!;
        public Guid StageId { get; init; }
        public string Severity { get; init; } = null!;
        public string? Level1 { get; init; }
        public string? Level2 { get; init; }
        public string? Level3 { get; init; }
        public string? Level4 { get; init; }
        public string? Level5 { get; init; }
        public int Count { get; init; }
    }
}
