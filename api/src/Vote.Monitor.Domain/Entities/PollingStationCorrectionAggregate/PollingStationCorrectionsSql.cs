namespace Vote.Monitor.Domain.Entities.PollingStationCorrectionAggregate;

public static class PollingStationCorrectionsSql
{
    /// <summary>
    /// A scalar subquery returning the record's corrections as a jsonb array of PollingStationCorrectionModel (oldest first).
    /// </summary>
    /// <param name="recordType">The record type.</param>
    /// <param name="recordIdColumn">The SQL expression holding the record id, e.g. <c>FS."Id"</c>.</param>
    public static string ForRecord(CorrectableRecordType recordType, string recordIdColumn) =>
        ForRecord($"'{recordType.Value}'", recordIdColumn);

    /// <param name="recordTypeExpression">The SQL expression holding the record type, e.g. <c>R."RecordType"</c>.</param>
    /// <param name="recordIdColumn">The SQL expression holding the record id, e.g. <c>R."Id"</c>.</param>
    public static string ForRecord(string recordTypeExpression, string recordIdColumn) => $"""
        COALESCE((SELECT jsonb_agg(jsonb_build_object(
                    'CorrectedAt', to_char(PSC."CorrectedAt" AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS.US"Z"'),
                    'From', jsonb_build_object('Id', FPS."Id", 'Number', FPS."Number", 'Level1', FPS."Level1", 'Level2', FPS."Level2", 'Level3', FPS."Level3", 'Level4', FPS."Level4", 'Level5', FPS."Level5", 'Address', FPS."Address"),
                    'To', jsonb_build_object('Id', TPS."Id", 'Number', TPS."Number", 'Level1', TPS."Level1", 'Level2', TPS."Level2", 'Level3', TPS."Level3", 'Level4', TPS."Level4", 'Level5', TPS."Level5", 'Address', TPS."Address")
                  ) ORDER BY PSC."CorrectedAt")
                  FROM "PollingStationCorrections" PSC
                      INNER JOIN "PollingStations" FPS ON FPS."Id" = PSC."FromPollingStationId"
                      INNER JOIN "PollingStations" TPS ON TPS."Id" = PSC."ToPollingStationId"
                  WHERE PSC."RecordType" = {recordTypeExpression} AND PSC."RecordId" = {recordIdColumn}), '[]'::jsonb)
        """;
}
