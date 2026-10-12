namespace Feature.PollingStationCorrections;

/// <summary>
/// The records platform admins can move: every form submission and incident report at a polling station
/// and every quick report from a visited polling station. Citizen reports are never included.
/// </summary>
internal static class CorrectableRecordsSql
{
    /// <summary>
    /// Parameters: @electionRoundId, @recordType, @recordId, @monitoringNgoId, @searchText, @pollingStationNumber,
    /// @correctedOnly, @level1 … @level5. A null parameter doesn't filter.
    /// </summary>
    public const string FromWhere = """
        FROM (SELECT 'FormSubmission' AS "RecordType",
                     FS."Id",
                     FS."MonitoringObserverId",
                     FS."PollingStationId",
                     F."Code" AS "FormCode",
                     F."Name" ->> F."DefaultLanguage" AS "FormName",
                     FS."LastUpdatedAt" AS "SubmittedAt"
              FROM "FormSubmissions" FS
                  INNER JOIN "Forms" F ON F."Id" = FS."FormId"
              WHERE FS."ElectionRoundId" = @electionRoundId
                AND (@recordType IS NULL OR @recordType = 'FormSubmission')
                AND (@recordId IS NULL OR FS."Id" = @recordId)
              UNION ALL
              SELECT 'QuickReport' AS "RecordType",
                     QR."Id",
                     QR."MonitoringObserverId",
                     QR."PollingStationId",
                     NULL AS "FormCode",
                     NULL AS "FormName",
                     QR."LastUpdatedAt" AS "SubmittedAt"
              FROM "QuickReports" QR
              WHERE QR."ElectionRoundId" = @electionRoundId
                AND QR."QuickReportLocationType" = 'VisitedPollingStation'
                AND QR."PollingStationId" IS NOT NULL
                AND (@recordType IS NULL OR @recordType = 'QuickReport')
                AND (@recordId IS NULL OR QR."Id" = @recordId)
              UNION ALL
              SELECT 'IncidentReport' AS "RecordType",
                     IR."Id",
                     IR."MonitoringObserverId",
                     IR."PollingStationId",
                     F."Code" AS "FormCode",
                     F."Name" ->> F."DefaultLanguage" AS "FormName",
                     IR."LastUpdatedAt" AS "SubmittedAt"
              FROM "IncidentReports" IR
                  INNER JOIN "Forms" F ON F."Id" = IR."FormId"
              WHERE IR."ElectionRoundId" = @electionRoundId
                AND IR."PollingStationId" IS NOT NULL
                AND (@recordType IS NULL OR @recordType = 'IncidentReport')
                AND (@recordId IS NULL OR IR."Id" = @recordId)) R
            INNER JOIN "PollingStations" PS ON PS."Id" = R."PollingStationId"
            INNER JOIN "MonitoringObservers" MO ON MO."Id" = R."MonitoringObserverId"
            INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
            INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
            INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
            LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
        WHERE (@monitoringNgoId IS NULL OR MN."Id" = @monitoringNgoId)
            AND (@searchText IS NULL OR U."DisplayName" ILIKE @searchText OR N."Name" ILIKE @searchText)
            AND (@pollingStationNumber IS NULL OR PS."Number" ILIKE @pollingStationNumber)
            AND (@correctedOnly IS NULL OR @correctedOnly = EXISTS (SELECT 1
                                                                    FROM "PollingStationCorrections" PSC
                                                                    WHERE PSC."ElectionRoundId" = @electionRoundId
                                                                      AND PSC."RecordType" = R."RecordType"
                                                                      AND PSC."RecordId" = R."Id"))
            AND (@level1 IS NULL OR PS."Level1" = @level1)
            AND (@level2 IS NULL OR PS."Level2" = @level2)
            AND (@level3 IS NULL OR PS."Level3" = @level3)
            AND (@level4 IS NULL OR PS."Level4" = @level4)
            AND (@level5 IS NULL OR PS."Level5" = @level5)
        """;

    public static readonly string Columns = $"""
        R."Id",
        R."RecordType",
        U."DisplayName" AS "ObserverName",
        N."Name" AS "NgoName",
        R."FormCode",
        R."FormName",
        R."SubmittedAt",
        PS."Id" AS "PollingStationId",
        PS."Number",
        PS."Level1",
        PS."Level2",
        PS."Level3",
        PS."Level4",
        PS."Level5",
        PS."Address",
        {PollingStationCorrectionsSql.ForRecord("R.\"RecordType\"", "R.\"Id\"")} AS "Corrections"
        """;
}
