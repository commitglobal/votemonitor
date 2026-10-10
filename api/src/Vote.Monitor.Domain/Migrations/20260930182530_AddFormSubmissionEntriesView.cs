using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFormSubmissionEntriesView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP VIEW IF EXISTS "FormSubmissionEntries";""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionEntries"(uuid, uuid, text);""");
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetFormSubmissionEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "SubmissionId"               UUID,
                                "ElectionRoundId"            UUID,
                                "FormType"                   TEXT,
                                "FormCode"                   TEXT,
                                "FormId"                     UUID,
                                "PollingStationId"           UUID,
                                "MonitoringObserverId"       UUID,
                                "NumberOfQuestionsAnswered"  INTEGER,
                                "NumberOfFlaggedAnswers"     INTEGER,
                                "MediaFilesCount"            BIGINT,
                                "NotesCount"                 BIGINT,
                                "TimeSubmitted"              TIMESTAMP WITH TIME ZONE,
                                "CreatedAt"                  TIMESTAMP WITH TIME ZONE,
                                "LastUpdatedAt"              TIMESTAMP WITH TIME ZONE,
                                "FollowUpStatus"             TEXT,
                                "DefaultLanguage"            TEXT,
                                "FormName"                   JSONB,
                                "NumberOfQuestions"          INTEGER,
                                "IsCompleted"                BOOLEAN,
                                "Answers"                    JSONB,
                                "Attachments"                JSONB,
                                "Notes"                      JSONB,
                                "ArrivalTime"                TIMESTAMP WITH TIME ZONE,
                                "DepartureTime"              TIMESTAMP WITH TIME ZONE,
                                "Breaks"                     JSONB,
                                "Level1"                     TEXT,
                                "Level2"                     TEXT,
                                "Level3"                     TEXT,
                                "Level4"                     TEXT,
                                "Level5"                     TEXT,
                                "Number"                     TEXT,
                                "NgoId"                      UUID,
                                "DisplayName"                TEXT,
                                "Email"                      TEXT,
                                "PhoneNumber"                TEXT,
                                "Tags"                       TEXT[],
                                "MonitoringObserverStatus"   TEXT,
                                "NgoName"                    VARCHAR(256),
                                "IsOwnObserver"              BOOLEAN,
                                "HasComments"                BOOLEAN,
                                "HasNotes"                   BOOLEAN,
                                "HasAttachments"             BOOLEAN
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY
                        SELECT psi."Id"                                              AS "SubmissionId",
                               psi."ElectionRoundId",
                               'PSI'::text                                           AS "FormType",
                               'PSI'::text                                           AS "FormCode",
                               psif."Id"                                             AS "FormId",
                               psi."PollingStationId",
                               psi."MonitoringObserverId",
                               psi."NumberOfQuestionsAnswered",
                               psi."NumberOfFlaggedAnswers",
                               0::bigint                                             AS "MediaFilesCount",
                               0::bigint                                             AS "NotesCount",
                               psi."LastUpdatedAt"                                   AS "TimeSubmitted",
                               psi."LastUpdatedAt"                                   AS "CreatedAt",
                               psi."LastUpdatedAt"                                   AS "LastUpdatedAt",
                               psi."FollowUpStatus",
                               psif."DefaultLanguage"::text,
                               psif."Name"                                           AS "FormName",
                               psif."NumberOfQuestions",
                               psi."IsCompleted",
                               psi."Answers",
                               '[]'::jsonb                                           AS "Attachments",
                               '[]'::jsonb                                           AS "Notes",
                               psi."ArrivalTime",
                               psi."DepartureTime",
                               psi."Breaks",
                               ps."Level1",
                               ps."Level2",
                               ps."Level3",
                               ps."Level4",
                               ps."Level5",
                               ps."Number",
                               mo."NgoId",
                               mo."DisplayName",
                               mo."Email",
                               mo."PhoneNumber",
                               mo."Tags",
                               mo."Status"                                           AS "MonitoringObserverStatus",
                               mo."NgoName",
                               mo."IsOwnObserver",
                               FALSE                                                 AS "HasComments",
                               FALSE                                                 AS "HasNotes",
                               FALSE                                                 AS "HasAttachments"
                        FROM "PollingStationInformation" psi
                                 INNER JOIN "PollingStationInformationForms" psif
                                            ON psif."Id" = psi."PollingStationInformationFormId"
                                 INNER JOIN "PollingStations" ps ON ps."Id" = psi."PollingStationId"
                                 INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) mo
                                            ON mo."MonitoringObserverId" = psi."MonitoringObserverId"
                                 INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) af
                                            ON af."FormId" = psi."PollingStationInformationFormId"
                        WHERE psi."ElectionRoundId" = electionRoundId
                        UNION ALL
                        SELECT fs."Id"                                               AS "SubmissionId",
                               fs."ElectionRoundId",
                               f."FormType",
                               f."Code"::text                                        AS "FormCode",
                               f."Id"                                                AS "FormId",
                               fs."PollingStationId",
                               fs."MonitoringObserverId",
                               fs."NumberOfQuestionsAnswered",
                               fs."NumberOfFlaggedAnswers",
                               (SELECT COUNT(1)
                                FROM "Attachments" a
                                WHERE a."MonitoringObserverId" = fs."MonitoringObserverId"
                                  AND (
                                    (a."FormId" = fs."FormId" AND fs."PollingStationId" = a."PollingStationId") -- backwards compatibility
                                        OR a."SubmissionId" = fs."Id"
                                    )
                                  AND a."IsDeleted" = false
                                  AND a."IsCompleted" = true)                        AS "MediaFilesCount",
                               (SELECT COUNT(1)
                                FROM "Notes" n
                                WHERE n."MonitoringObserverId" = fs."MonitoringObserverId"
                                  AND (
                                    (n."FormId" = fs."FormId" AND fs."PollingStationId" = n."PollingStationId") -- backwards compatibility
                                        OR n."SubmissionId" = fs."Id"
                                    ))                                               AS "NotesCount",
                               fs."LastUpdatedAt"                                    AS "TimeSubmitted",
                               fs."CreatedAt",
                               fs."LastUpdatedAt",
                               fs."FollowUpStatus",
                               f."DefaultLanguage"::text,
                               f."Name"                                              AS "FormName",
                               f."NumberOfQuestions",
                               fs."IsCompleted",
                               fs."Answers",
                               COALESCE((SELECT jsonb_agg(jsonb_build_object(
                                                                  'QuestionId', a."QuestionId",
                                                                  'FileName', a."FileName",
                                                                  'MimeType', a."MimeType",
                                                                  'FilePath', a."FilePath",
                                                                  'UploadedFileName', a."UploadedFileName",
                                                                  'TimeSubmitted', a."LastUpdatedAt"))
                                         FROM "Attachments" a
                                         WHERE a."MonitoringObserverId" = fs."MonitoringObserverId"
                                           AND (
                                             (a."FormId" = fs."FormId" AND fs."PollingStationId" = a."PollingStationId") -- backwards compatibility
                                                 OR a."SubmissionId" = fs."Id"
                                             )
                                           AND a."IsDeleted" = false
                                           AND a."IsCompleted" = true), '[]'::jsonb) AS "Attachments",
                               COALESCE((SELECT jsonb_agg(jsonb_build_object(
                                                                  'QuestionId', n."QuestionId",
                                                                  'Text', n."Text",
                                                                  'TimeSubmitted', n."LastUpdatedAt"))
                                         FROM "Notes" n
                                         WHERE n."MonitoringObserverId" = fs."MonitoringObserverId"
                                           AND (
                                             (n."FormId" = fs."FormId" AND fs."PollingStationId" = n."PollingStationId") -- backwards compatibility
                                                 OR n."SubmissionId" = fs."Id"
                                             )), '[]'::jsonb)                        AS "Notes",
                               NULL::timestamp with time zone                        AS "ArrivalTime",
                               NULL::timestamp with time zone                        AS "DepartureTime",
                               '[]'::jsonb                                           AS "Breaks",
                               ps."Level1",
                               ps."Level2",
                               ps."Level3",
                               ps."Level4",
                               ps."Level5",
                               ps."Number",
                               mo."NgoId",
                               mo."DisplayName",
                               mo."Email",
                               mo."PhoneNumber",
                               mo."Tags",
                               mo."Status"                                           AS "MonitoringObserverStatus",
                               mo."NgoName",
                               mo."IsOwnObserver",
                               (EXISTS (SELECT 1
                                        FROM "FormSubmissionComments" c
                                                 INNER JOIN "ApplicationUsers" au ON au."Id" = c."CreatedBy"
                                        WHERE c."SubmissionId" = fs."Id"
                                          AND au."UserRole" = 'NgoAdmin'
                                          AND EXISTS (SELECT 1 FROM "NgoAdmins" na WHERE na."ApplicationUserId" = au."Id" AND na."NgoId" = ngoId)
                                        UNION
                                        SELECT 1
                                        FROM "FormSubmissionComments" c
                                                 INNER JOIN "ApplicationUsers" au ON au."Id" = c."CreatedBy"
                                        WHERE c."SubmissionId" = fs."Id"
                                          AND au."UserRole" = 'NgoStaff'
                                          AND EXISTS (SELECT 1 FROM "NgoStaff" ns WHERE ns."ApplicationUserId" = au."Id" AND ns."NgoId" = ngoId))) AS "HasComments",
                               (SELECT COUNT(1)
                                FROM "Notes" n
                                WHERE n."MonitoringObserverId" = fs."MonitoringObserverId"
                                  AND (
                                    (n."FormId" = fs."FormId" AND fs."PollingStationId" = n."PollingStationId") -- backwards compatibility
                                        OR n."SubmissionId" = fs."Id"
                                    )) > 0                                   AS "HasNotes",
                               (SELECT COUNT(1)
                                FROM "Attachments" a
                                WHERE a."MonitoringObserverId" = fs."MonitoringObserverId"
                                  AND (
                                    (a."FormId" = fs."FormId" AND fs."PollingStationId" = a."PollingStationId") -- backwards compatibility
                                        OR a."SubmissionId" = fs."Id"
                                    )
                                  AND a."IsDeleted" = false
                                  AND a."IsCompleted" = true) > 0              AS "HasAttachments"
                        FROM "FormSubmissions" fs
                                 INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                                 INNER JOIN "PollingStations" ps ON ps."Id" = fs."PollingStationId"
                                 INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) mo
                                            ON mo."MonitoringObserverId" = fs."MonitoringObserverId"
                                 INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) af
                                            ON af."FormId" = fs."FormId"
                        WHERE fs."ElectionRoundId" = electionRoundId;
                END;
                $$ LANGUAGE plpgsql;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionEntries"(uuid, uuid, text);""");
        }
    }
}
