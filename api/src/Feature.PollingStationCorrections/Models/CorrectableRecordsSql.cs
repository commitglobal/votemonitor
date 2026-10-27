namespace Feature.PollingStationCorrections.Models;

internal static class CorrectableRecordsSql
{
    /// <summary>
    /// The rows of a "GetCorrectableRecords" call (<paramref name="functionCall"/>) with the observer's email and, for form
    /// submissions, the submission number: which of the observer's submissions of that form at that polling station it is,
    /// by creation (always 1 for PSI forms), as in the form submissions export. The number counts all of the observer's
    /// submissions, so filters and pages never change it. Aliased R, ready for WHERE / ORDER BY on its columns.
    /// </summary>
    public static string WithObserverEmailAndSubmissionNumber(string functionCall) => $"""
        (SELECT R.*,
                U."Email"::text AS "ObserverEmail",
                CASE
                    WHEN R."RecordType" <> 'FormSubmission' THEN NULL
                    WHEN F."FormType" = 'PSI' THEN 1
                    ELSE (SELECT COUNT(*)::int
                          FROM "FormSubmissions" S
                          WHERE S."ElectionRoundId" = FS."ElectionRoundId"
                            AND S."FormId" = FS."FormId"
                            AND S."PollingStationId" = FS."PollingStationId"
                            AND S."MonitoringObserverId" = FS."MonitoringObserverId"
                            AND (S."CreatedAt", S."Id") <= (FS."CreatedAt", FS."Id"))
                END AS "SubmissionNumber"
         FROM {functionCall} R
             LEFT JOIN "FormSubmissions" FS ON R."RecordType" = 'FormSubmission' AND FS."Id" = R."Id"
             LEFT JOIN "Forms" F ON F."Id" = FS."FormId"
             LEFT JOIN "QuickReports" QR ON R."RecordType" = 'QuickReport' AND QR."Id" = R."Id"
             LEFT JOIN "IncidentReports" IR ON R."RecordType" = 'IncidentReport' AND IR."Id" = R."Id"
             LEFT JOIN "MonitoringObservers" MO
                 ON MO."Id" = COALESCE(FS."MonitoringObserverId", QR."MonitoringObserverId", IR."MonitoringObserverId")
             LEFT JOIN "Observers" O ON O."Id" = MO."ObserverId"
             LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId") R
        """;
}
