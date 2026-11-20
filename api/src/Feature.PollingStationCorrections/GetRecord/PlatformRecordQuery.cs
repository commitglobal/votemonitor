using Feature.PollingStationCorrections.Models;

namespace Feature.PollingStationCorrections.GetRecord;

/// <summary>
/// One form submission, quick report or incident report as platform admins see it: who sent it, from where, and its
/// polling-station corrections, never what was reported (ADR 9). Each record type has its own route.
/// </summary>
internal static class PlatformRecordQuery
{
    public static async Task<Results<Ok<CorrectableRecordModel>, NotFound>> GetAsync(
        INpgsqlConnectionFactory dbConnectionFactory,
        Guid electionRoundId,
        CorrectableRecordType recordType,
        Guid recordId,
        CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM "GetCorrectableRecords"(@electionRoundId, @recordTypes, @recordId, @monitoringNgoId, @searchText,
                               @pollingStationNumber, @correctedOnly, @level1, @level2, @level3, @level4, @level5);
                           """;

        var args = new
        {
            electionRoundId,
            recordTypes = new[] { recordType.Value },
            recordId = (Guid?)recordId,
            monitoringNgoId = (Guid?)null,
            searchText = (string?)null,
            pollingStationNumber = (string?)null,
            correctedOnly = (bool?)null,
            level1 = Array.Empty<string>(),
            level2 = Array.Empty<string>(),
            level3 = Array.Empty<string>(),
            level4 = Array.Empty<string>(),
            level5 = Array.Empty<string>()
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        var record = await dbConnection.QueryFirstOrDefaultAsync<CorrectableRecordRow>(sql, args);

        return record is null ? TypedResults.NotFound() : TypedResults.Ok(record.ToModel());
    }
}

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid Id { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}

/// <summary>Base of the per-type platform GETs: platform admins only, no answers.</summary>
public abstract class PlatformRecordEndpoint(INpgsqlConnectionFactory dbConnectionFactory, CorrectableRecordType recordType, string route, string tag)
    : Endpoint<Request, Results<Ok<CorrectableRecordModel>, NotFound>>
{
    public override void Configure()
    {
        Get(route);
        DontAutoTag();
        Options(x => x.WithTags(tag, "polling-station-corrections"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Gets a record as platform admins see it: who sent it, from where, and its polling-station corrections";
        });
    }

    public override Task<Results<Ok<CorrectableRecordModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct) =>
        PlatformRecordQuery.GetAsync(dbConnectionFactory, req.ElectionRoundId, recordType, req.Id, ct);
}

public class GetFormSubmissionEndpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : PlatformRecordEndpoint(dbConnectionFactory, CorrectableRecordType.FormSubmission,
        "/api/election-rounds/{electionRoundId}/form-submissions/{id}:admin", "form-submissions");

public class GetQuickReportEndpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : PlatformRecordEndpoint(dbConnectionFactory, CorrectableRecordType.QuickReport,
        "/api/election-rounds/{electionRoundId}/quick-reports/{id}:admin", "quick-reports");

public class GetIncidentReportEndpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : PlatformRecordEndpoint(dbConnectionFactory, CorrectableRecordType.IncidentReport,
        "/api/election-rounds/{electionRoundId}/incident-reports/{id}:admin", "incident-reports");
