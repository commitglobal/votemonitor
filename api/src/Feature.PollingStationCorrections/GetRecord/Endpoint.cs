using Feature.PollingStationCorrections.Models;

namespace Feature.PollingStationCorrections.GetRecord;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<CorrectableRecordModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-station-corrections:records/{recordType}/{recordId}");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-corrections"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Gets a record platform admins can move to another polling station, with its correction history";
        });
    }

    private static readonly string RecordsWithObserverEmailAndSubmissionNumber =
        CorrectableRecordsSql.WithObserverEmailAndSubmissionNumber("""
            "GetCorrectableRecords"(@electionRoundId, @recordType, @recordId, @monitoringNgoId, @searchText,
                @pollingStationNumber, @correctedOnly, @level1, @level2, @level3, @level4, @level5)
            """);

    public override async Task<Results<Ok<CorrectableRecordModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var sql = $"""
                   SELECT *
                   FROM {RecordsWithObserverEmailAndSubmissionNumber};
                   """;

        var args = new
        {
            electionRoundId = req.ElectionRoundId,
            recordType = req.RecordType.Value,
            recordId = (Guid?)req.RecordId,
            monitoringNgoId = (Guid?)null,
            searchText = (string?)null,
            pollingStationNumber = (string?)null,
            correctedOnly = (bool?)null,
            level1 = (string?)null,
            level2 = (string?)null,
            level3 = (string?)null,
            level4 = (string?)null,
            level5 = (string?)null
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        var record = await dbConnection.QueryFirstOrDefaultAsync<CorrectableRecordRow>(sql, args);

        return record is null ? TypedResults.NotFound() : TypedResults.Ok(record.ToModel());
    }
}
