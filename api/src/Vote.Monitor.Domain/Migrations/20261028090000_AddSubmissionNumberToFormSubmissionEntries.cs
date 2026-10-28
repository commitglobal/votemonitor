using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionNumberToFormSubmissionEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The submissions list numbers an observer's submissions of the same form at the same polling station,
            // as the export does. New result columns need the function dropped first.
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionEntries"(uuid, uuid, text, uuid);""");
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetFormSubmissionEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    recordId UUID DEFAULT NULL
                )
                    RETURNS TABLE
                            (
                                "SubmissionId"               UUID,
                                "ElectionRoundId"            UUID,
                                "FormType"                   TEXT,
                                "FormCode"                   TEXT,
                                "FormStatus"                 TEXT,
                                "FormId"                     UUID,
                                "PollingStationId"           UUID,
                                "MonitoringObserverId"       UUID,
                                "NumberOfQuestionsAnswered"  INTEGER,
                                "NumberOfFlaggedAnswers"     INTEGER,
                                "MediaFilesCount"            BIGINT,
                                "NotesCount"                 BIGINT,
                                "CommentsCount"              BIGINT,
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
                                "HasAttachments"    BOOLEAN,
                                "QuestionsAnswered" TEXT,
                                "StageId"                    UUID,
                                "StageName"                  TEXT,
                                "ReviewStatus"                 TEXT,
                                "StageChangedAt"             TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"                   TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"                UUID[],
                                "Severity"                   TEXT,
                                -- Which of the observer's submissions of this form at this polling station this is (by creation
                                -- time, then id), and how many there are; counted over all of them, so filters and paging never
                                -- change it. Polling station information is one per station: always 1 of 1.
                                "SubmissionNumber"           INTEGER,
                                "SubmissionCount"            INTEGER
                            )
                AS
                $$
                DECLARE
                    callerMonitoringNgoId UUID;
                    defaultStageId UUID;
                BEGIN
                    -- Review columns are the caller's NGO's: its own review stage and reviewers, never another NGO's.
                    SELECT MN."Id" INTO callerMonitoringNgoId FROM "MonitoringNgos" MN WHERE MN."ElectionRoundId" = electionRoundId AND MN."NgoId" = ngoId;
                    SELECT RS."Id" INTO defaultStageId FROM "ReviewStages" RS WHERE RS."MonitoringNgoId" = callerMonitoringNgoId AND RS."IsDefault";

                    RETURN QUERY
                        SELECT psi."Id"                                              AS "SubmissionId",
                               psi."ElectionRoundId",
                               'PSI'::text                                           AS "FormType",
                               'PSI'::text                                           AS "FormCode",
                               'Published'::text                                     AS "FormStatus",
                               psif."Id"                                             AS "FormId",
                               psi."PollingStationId",
                               psi."MonitoringObserverId",
                               psi."NumberOfQuestionsAnswered",
                               psi."NumberOfFlaggedAnswers",
                               0::bigint                                             AS "MediaFilesCount",
                               0::bigint                                             AS "NotesCount",
                               0::bigint                                             AS "CommentsCount",
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
                               ps."Level1"::text,
                               ps."Level2"::text,
                               ps."Level3"::text,
                               ps."Level4"::text,
                               ps."Level5"::text,
                               ps."Number"::text,
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
                               FALSE                                                 AS "HasAttachments",
                               CASE
                                 WHEN psi."NumberOfQuestionsAnswered" = 0 THEN 'None'::text
                                 WHEN psi."NumberOfQuestionsAnswered" = psif."NumberOfQuestions" THEN 'All'::text
                               ELSE 'Some'::text
                END AS "QuestionsAnswered",
                               RST."Id" AS "StageId",
                               RST."Name"::text AS "StageName",
                               RST."Status"::text AS "ReviewStatus",
                               RWF."StageChangedAt",
                               RWF."ReviewedAt",
                               RAS."Value" AS "ReviewerIds",
                               COALESCE(RSV."Severity", 'None')::text AS "Severity",
                               1 AS "SubmissionNumber",
                               1 AS "SubmissionCount"
                        FROM "PollingStationInformation" psi
                                 LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                                     AND RWF."RecordType" = 'FormSubmission' AND RWF."RecordId" = psi."Id"
                                 LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                                 CROSS JOIN LATERAL (
                                     SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                                     FROM "Reviewers" X
                                     WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'FormSubmission' AND X."RecordId" = psi."Id"
                                 ) RAS
                                 LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = callerMonitoringNgoId
                                     AND RSV."RecordType" = 'FormSubmission' AND RSV."RecordId" = psi."Id"
                                 INNER JOIN "PollingStationInformationForms" psif
                                            ON psif."Id" = psi."PollingStationInformationFormId"
                                 INNER JOIN "PollingStations" ps ON ps."Id" = psi."PollingStationId"
                                 INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) mo
                                            ON mo."MonitoringObserverId" = psi."MonitoringObserverId"
                                 INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) af
                                            ON af."FormId" = psi."PollingStationInformationFormId"
                        WHERE psi."ElectionRoundId" = electionRoundId
                          AND (recordId IS NULL OR psi."Id" = recordId)
                        UNION ALL
                        SELECT fs."Id"                                               AS "SubmissionId",
                               fs."ElectionRoundId",
                               f."FormType",
                               f."Code"::text                                        AS "FormCode",
                               f."Status"::text                                      AS "FormStatus",
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
                               (SELECT COUNT(1)
                                FROM "FormSubmissionComments" c
                                WHERE c."SubmissionId" = fs."Id"
                                  AND EXISTS (
                                      SELECT 1 FROM "NgoAdmins" na 
                                      WHERE na."ApplicationUserId" = c."CreatedBy" AND na."NgoId" = ngoId
                                      UNION
                                      SELECT 1 FROM "NgoStaff" ns 
                                      WHERE ns."ApplicationUserId" = c."CreatedBy" AND ns."NgoId" = ngoId
                                  )) AS "CommentsCount",
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
                               ps."Level1"::text,
                               ps."Level2"::text,
                               ps."Level3"::text,
                               ps."Level4"::text,
                               ps."Level5"::text,
                               ps."Number"::text,
                               mo."NgoId",
                               mo."DisplayName",
                               mo."Email",
                               mo."PhoneNumber",
                               mo."Tags",
                               mo."Status"                                           AS "MonitoringObserverStatus",
                               mo."NgoName",
                               mo."IsOwnObserver",
                               (SELECT COUNT(1)
                                FROM "FormSubmissionComments" c
                                WHERE c."SubmissionId" = fs."Id"
                                  AND EXISTS (
                                      SELECT 1 FROM "NgoAdmins" na 
                                      WHERE na."ApplicationUserId" = c."CreatedBy" AND na."NgoId" = ngoId
                                      UNION
                                      SELECT 1 FROM "NgoStaff" ns 
                                      WHERE ns."ApplicationUserId" = c."CreatedBy" AND ns."NgoId" = ngoId
                                  )) > 0              AS "HasComments",
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
                                  AND a."IsCompleted" = true) > 0              AS "HasAttachments",
                               CASE
                                 WHEN fs."NumberOfQuestionsAnswered" = 0 THEN 'None'::text
                                 WHEN fs."NumberOfQuestionsAnswered" = f."NumberOfQuestions" THEN 'All'::text
                               ELSE 'Some'::text
                END AS "QuestionsAnswered",
                               RST."Id" AS "StageId",
                               RST."Name"::text AS "StageName",
                               RST."Status"::text AS "ReviewStatus",
                               RWF."StageChangedAt",
                               RWF."ReviewedAt",
                               RAS."Value" AS "ReviewerIds",
                               COALESCE(RSV."Severity", 'None')::text AS "Severity",
                               (SELECT COUNT(1)
                                FROM "FormSubmissions" S
                                WHERE S."FormId" = fs."FormId"
                                  AND S."PollingStationId" = fs."PollingStationId"
                                  AND S."MonitoringObserverId" = fs."MonitoringObserverId"
                                  AND (S."CreatedAt", S."Id") <= (fs."CreatedAt", fs."Id"))::INTEGER AS "SubmissionNumber",
                               (SELECT COUNT(1)
                                FROM "FormSubmissions" S
                                WHERE S."FormId" = fs."FormId"
                                  AND S."PollingStationId" = fs."PollingStationId"
                                  AND S."MonitoringObserverId" = fs."MonitoringObserverId")::INTEGER AS "SubmissionCount"
                        FROM "FormSubmissions" fs
                                 LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                                     AND RWF."RecordType" = 'FormSubmission' AND RWF."RecordId" = fs."Id"
                                 LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                                 CROSS JOIN LATERAL (
                                     SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                                     FROM "Reviewers" X
                                     WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'FormSubmission' AND X."RecordId" = fs."Id"
                                 ) RAS
                                 LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = callerMonitoringNgoId
                                     AND RSV."RecordType" = 'FormSubmission' AND RSV."RecordId" = fs."Id"
                                 INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                                 INNER JOIN "PollingStations" ps ON ps."Id" = fs."PollingStationId"
                                 INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) mo
                                            ON mo."MonitoringObserverId" = fs."MonitoringObserverId"
                                 INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) af
                                            ON af."FormId" = fs."FormId"
                        WHERE fs."ElectionRoundId" = electionRoundId
                          AND (recordId IS NULL OR fs."Id" = recordId);
                END;
                $$ LANGUAGE plpgsql;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionEntries"(uuid, uuid, text, uuid);""");
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetFormSubmissionEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    recordId UUID DEFAULT NULL
                )
                    RETURNS TABLE
                            (
                                "SubmissionId"               UUID,
                                "ElectionRoundId"            UUID,
                                "FormType"                   TEXT,
                                "FormCode"                   TEXT,
                                "FormStatus"                 TEXT,
                                "FormId"                     UUID,
                                "PollingStationId"           UUID,
                                "MonitoringObserverId"       UUID,
                                "NumberOfQuestionsAnswered"  INTEGER,
                                "NumberOfFlaggedAnswers"     INTEGER,
                                "MediaFilesCount"            BIGINT,
                                "NotesCount"                 BIGINT,
                                "CommentsCount"              BIGINT,
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
                                "HasAttachments"    BOOLEAN,
                                "QuestionsAnswered" TEXT,
                                "StageId"                    UUID,
                                "StageName"                  TEXT,
                                "ReviewStatus"                 TEXT,
                                "StageChangedAt"             TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"                   TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"                UUID[],
                                "Severity"                   TEXT
                            )
                AS
                $$
                DECLARE
                    callerMonitoringNgoId UUID;
                    defaultStageId UUID;
                BEGIN
                    -- Review columns are the caller's NGO's: its own review stage and reviewers, never another NGO's.
                    SELECT MN."Id" INTO callerMonitoringNgoId FROM "MonitoringNgos" MN WHERE MN."ElectionRoundId" = electionRoundId AND MN."NgoId" = ngoId;
                    SELECT RS."Id" INTO defaultStageId FROM "ReviewStages" RS WHERE RS."MonitoringNgoId" = callerMonitoringNgoId AND RS."IsDefault";

                    RETURN QUERY
                        SELECT psi."Id"                                              AS "SubmissionId",
                               psi."ElectionRoundId",
                               'PSI'::text                                           AS "FormType",
                               'PSI'::text                                           AS "FormCode",
                               'Published'::text                                     AS "FormStatus",
                               psif."Id"                                             AS "FormId",
                               psi."PollingStationId",
                               psi."MonitoringObserverId",
                               psi."NumberOfQuestionsAnswered",
                               psi."NumberOfFlaggedAnswers",
                               0::bigint                                             AS "MediaFilesCount",
                               0::bigint                                             AS "NotesCount",
                               0::bigint                                             AS "CommentsCount",
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
                               ps."Level1"::text,
                               ps."Level2"::text,
                               ps."Level3"::text,
                               ps."Level4"::text,
                               ps."Level5"::text,
                               ps."Number"::text,
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
                               FALSE                                                 AS "HasAttachments",
                               CASE
                                 WHEN psi."NumberOfQuestionsAnswered" = 0 THEN 'None'::text
                                 WHEN psi."NumberOfQuestionsAnswered" = psif."NumberOfQuestions" THEN 'All'::text
                               ELSE 'Some'::text
                END AS "QuestionsAnswered",
                               RST."Id" AS "StageId",
                               RST."Name"::text AS "StageName",
                               RST."Status"::text AS "ReviewStatus",
                               RWF."StageChangedAt",
                               RWF."ReviewedAt",
                               RAS."Value" AS "ReviewerIds",
                               COALESCE(RSV."Severity", 'None')::text AS "Severity"
                        FROM "PollingStationInformation" psi
                                 LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                                     AND RWF."RecordType" = 'FormSubmission' AND RWF."RecordId" = psi."Id"
                                 LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                                 CROSS JOIN LATERAL (
                                     SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                                     FROM "Reviewers" X
                                     WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'FormSubmission' AND X."RecordId" = psi."Id"
                                 ) RAS
                                 LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = callerMonitoringNgoId
                                     AND RSV."RecordType" = 'FormSubmission' AND RSV."RecordId" = psi."Id"
                                 INNER JOIN "PollingStationInformationForms" psif
                                            ON psif."Id" = psi."PollingStationInformationFormId"
                                 INNER JOIN "PollingStations" ps ON ps."Id" = psi."PollingStationId"
                                 INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) mo
                                            ON mo."MonitoringObserverId" = psi."MonitoringObserverId"
                                 INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) af
                                            ON af."FormId" = psi."PollingStationInformationFormId"
                        WHERE psi."ElectionRoundId" = electionRoundId
                          AND (recordId IS NULL OR psi."Id" = recordId)
                        UNION ALL
                        SELECT fs."Id"                                               AS "SubmissionId",
                               fs."ElectionRoundId",
                               f."FormType",
                               f."Code"::text                                        AS "FormCode",
                               f."Status"::text                                      AS "FormStatus",
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
                               (SELECT COUNT(1)
                                FROM "FormSubmissionComments" c
                                WHERE c."SubmissionId" = fs."Id"
                                  AND EXISTS (
                                      SELECT 1 FROM "NgoAdmins" na 
                                      WHERE na."ApplicationUserId" = c."CreatedBy" AND na."NgoId" = ngoId
                                      UNION
                                      SELECT 1 FROM "NgoStaff" ns 
                                      WHERE ns."ApplicationUserId" = c."CreatedBy" AND ns."NgoId" = ngoId
                                  )) AS "CommentsCount",
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
                               ps."Level1"::text,
                               ps."Level2"::text,
                               ps."Level3"::text,
                               ps."Level4"::text,
                               ps."Level5"::text,
                               ps."Number"::text,
                               mo."NgoId",
                               mo."DisplayName",
                               mo."Email",
                               mo."PhoneNumber",
                               mo."Tags",
                               mo."Status"                                           AS "MonitoringObserverStatus",
                               mo."NgoName",
                               mo."IsOwnObserver",
                               (SELECT COUNT(1)
                                FROM "FormSubmissionComments" c
                                WHERE c."SubmissionId" = fs."Id"
                                  AND EXISTS (
                                      SELECT 1 FROM "NgoAdmins" na 
                                      WHERE na."ApplicationUserId" = c."CreatedBy" AND na."NgoId" = ngoId
                                      UNION
                                      SELECT 1 FROM "NgoStaff" ns 
                                      WHERE ns."ApplicationUserId" = c."CreatedBy" AND ns."NgoId" = ngoId
                                  )) > 0              AS "HasComments",
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
                                  AND a."IsCompleted" = true) > 0              AS "HasAttachments",
                               CASE
                                 WHEN fs."NumberOfQuestionsAnswered" = 0 THEN 'None'::text
                                 WHEN fs."NumberOfQuestionsAnswered" = f."NumberOfQuestions" THEN 'All'::text
                               ELSE 'Some'::text
                END AS "QuestionsAnswered",
                               RST."Id" AS "StageId",
                               RST."Name"::text AS "StageName",
                               RST."Status"::text AS "ReviewStatus",
                               RWF."StageChangedAt",
                               RWF."ReviewedAt",
                               RAS."Value" AS "ReviewerIds",
                               COALESCE(RSV."Severity", 'None')::text AS "Severity"
                        FROM "FormSubmissions" fs
                                 LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                                     AND RWF."RecordType" = 'FormSubmission' AND RWF."RecordId" = fs."Id"
                                 LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                                 CROSS JOIN LATERAL (
                                     SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                                     FROM "Reviewers" X
                                     WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'FormSubmission' AND X."RecordId" = fs."Id"
                                 ) RAS
                                 LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = callerMonitoringNgoId
                                     AND RSV."RecordType" = 'FormSubmission' AND RSV."RecordId" = fs."Id"
                                 INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                                 INNER JOIN "PollingStations" ps ON ps."Id" = fs."PollingStationId"
                                 INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) mo
                                            ON mo."MonitoringObserverId" = fs."MonitoringObserverId"
                                 INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) af
                                            ON af."FormId" = fs."FormId"
                        WHERE fs."ElectionRoundId" = electionRoundId
                          AND (recordId IS NULL OR fs."Id" = recordId);
                END;
                $$ LANGUAGE plpgsql;
                """);
        }
    }
}
