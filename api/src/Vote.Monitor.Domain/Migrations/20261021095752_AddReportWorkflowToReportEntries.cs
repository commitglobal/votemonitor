using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddReportWorkflowToReportEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionEntries"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportEntries"(uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportEntries"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportEntries"(uuid, uuid, text);""");

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
                                "ReviewerIds"                UUID[]
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
                               RAS."Value" AS "ReviewerIds"
                        FROM "PollingStationInformation" psi
                                 LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                                     AND RWF."RecordType" = 'FormSubmission' AND RWF."RecordId" = psi."Id"
                                 LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                                 CROSS JOIN LATERAL (
                                     SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                                     FROM "Reviewers" X
                                     WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'FormSubmission' AND X."RecordId" = psi."Id"
                                 ) RAS
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
                               RAS."Value" AS "ReviewerIds"
                        FROM "FormSubmissions" fs
                                 LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                                     AND RWF."RecordType" = 'FormSubmission' AND RWF."RecordId" = fs."Id"
                                 LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                                 CROSS JOIN LATERAL (
                                     SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                                     FROM "Reviewers" X
                                     WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'FormSubmission' AND X."RecordId" = fs."Id"
                                 ) RAS
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

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetCitizenReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "ElectionRoundId"            UUID,
                                "FormId"                     UUID,
                                "FormCode"                   TEXT,
                                "FormName"                   JSONB,
                                "FormDefaultLanguage"        TEXT,
                                "FormType"                   TEXT,
                                "FormStatus"                 TEXT,
                                "Answers"                    JSONB,
                                "NumberOfQuestionsAnswered"  INTEGER,
                                "NumberOfFlaggedAnswers"     INTEGER,
                                "FollowUpStatus"             TEXT,
                                "TimeSubmitted"              TIMESTAMP WITH TIME ZONE,
                                "QuestionsAnswered"          TEXT,
                                "LocationId"                 UUID,
                                "Level1"                     TEXT,
                                "Level2"                     TEXT,
                                "Level3"                     TEXT,
                                "Level4"                     TEXT,
                                "Level5"                     TEXT,
                                "NotesCount"                 BIGINT,
                                "MediaFilesCount"            BIGINT,
                                "CommentsCount"              BIGINT,
                                "HasNotes"                   BOOLEAN,
                                "HasComments"                BOOLEAN,
                                "HasAttachments"             BOOLEAN,
                                "NgoId"                      UUID,
                                "CitizenReportingEnabled"    BOOLEAN,
                                "StageId"                    UUID,
                                "StageName"                  TEXT,
                                "ReviewStatus"                 TEXT,
                                "StageChangedAt"             TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"                   TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"                UUID[]
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
                    SELECT
                        CR."Id",
                        CR."ElectionRoundId",
                        CR."FormId",
                        F."Code"::text,
                        F."Name",
                        F."DefaultLanguage"::text,
                        F."FormType"::text,
                        F."Status"::text,
                        CR."Answers",
                        CR."NumberOfQuestionsAnswered",
                        CR."NumberOfFlaggedAnswers",
                        CR."FollowUpStatus"::text,
                        COALESCE(CR."LastModifiedOn", CR."CreatedOn"),
                        CASE
                            WHEN CR."NumberOfQuestionsAnswered" = 0 THEN 'None'::text
                            WHEN F."NumberOfQuestions" = CR."NumberOfQuestionsAnswered" THEN 'All'::text
                            ELSE 'Some'::text
                        END,
                        L."Id",
                        L."Level1"::text,
                        L."Level2"::text,
                        L."Level3"::text,
                        L."Level4"::text,
                        L."Level5"::text,
                        NC."Value",
                        AC."Value",
                        CC."Value",
                        NC."Value" > 0,
                        CC."Value" > 0,
                        AC."Value" > 0,
                        MN."NgoId",
                        ER."CitizenReportingEnabled",
                        RST."Id" AS "StageId",
                        RST."Name"::text AS "StageName",
                        RST."Status"::text AS "ReviewStatus",
                        RWF."StageChangedAt",
                        RWF."ReviewedAt",
                        RAS."Value" AS "ReviewerIds"
                    FROM "CitizenReports" CR
                        LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                            AND RWF."RecordType" = 'CitizenReport' AND RWF."RecordId" = CR."Id"
                        LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                            FROM "Reviewers" X
                            WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'CitizenReport' AND X."RecordId" = CR."Id"
                        ) RAS
                        INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                        INNER JOIN "Locations" L ON L."Id" = CR."LocationId"
                        INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "CitizenReportNotes" X
                            WHERE X."CitizenReportId" = CR."Id"
                        ) NC
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "CitizenReportAttachments" X
                            WHERE X."CitizenReportId" = CR."Id"
                              AND X."IsDeleted" = FALSE
                              AND X."IsCompleted" = TRUE
                        ) AC
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "CitizenReportComments" X
                            WHERE X."CitizenReportId" = CR."Id"
                              AND (
                                  EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = X."CreatedBy" AND NA."NgoId" = ngoId)
                                  OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = X."CreatedBy" AND NS."NgoId" = ngoId)
                              )
                        ) CC
                    WHERE CR."ElectionRoundId" = electionRoundId
                      AND MN."NgoId" = ngoId
                      AND ER."CitizenReportingEnabled" = TRUE;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetIncidentReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "ElectionRoundId"            UUID,
                                "FormId"                     UUID,
                                "FormCode"                   TEXT,
                                "FormName"                   JSONB,
                                "FormDefaultLanguage"        TEXT,
                                "FormType"                   TEXT,
                                "FormStatus"                 TEXT,
                                "Answers"                    JSONB,
                                "MonitoringObserverId"       UUID,
                                "DisplayName"                TEXT,
                                "Email"                      TEXT,
                                "PhoneNumber"                TEXT,
                                "Tags"                       TEXT[],
                                "MonitoringObserverStatus"   TEXT,
                                "NgoId"                      UUID,
                                "NgoName"                    TEXT,
                                "PollingStationId"           UUID,
                                "LocationType"               TEXT,
                                "LocationDescription"        TEXT,
                                "NumberOfQuestionsAnswered"  INTEGER,
                                "NumberOfFlaggedAnswers"     INTEGER,
                                "FollowUpStatus"             TEXT,
                                "IsCompleted"                BOOLEAN,
                                "TimeSubmitted"              TIMESTAMP WITH TIME ZONE,
                                "QuestionsAnswered"          TEXT,
                                "Level1"                     TEXT,
                                "Level2"                     TEXT,
                                "Level3"                     TEXT,
                                "Level4"                     TEXT,
                                "Level5"                     TEXT,
                                "Number"                     TEXT,
                                "NotesCount"                 BIGINT,
                                "MediaFilesCount"            BIGINT,
                                "CommentsCount"              BIGINT,
                                "HasNotes"                   BOOLEAN,
                                "HasComments"                BOOLEAN,
                                "HasAttachments"             BOOLEAN,
                                "StageId"                    UUID,
                                "StageName"                  TEXT,
                                "ReviewStatus"                 TEXT,
                                "StageChangedAt"             TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"                   TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"                UUID[]
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
                    SELECT
                        IR."Id",
                        IR."ElectionRoundId",
                        IR."FormId",
                        F."Code"::text,
                        F."Name",
                        F."DefaultLanguage"::text,
                        F."FormType"::text,
                        F."Status"::text,
                        IR."Answers",
                        IR."MonitoringObserverId",
                        AMO."DisplayName",
                        AMO."Email",
                        AMO."PhoneNumber",
                        AMO."Tags",
                        AMO."Status",
                        AMO."NgoId",
                        AMO."NgoName"::text,
                        IR."PollingStationId",
                        IR."LocationType"::text,
                        IR."LocationDescription"::text,
                        IR."NumberOfQuestionsAnswered",
                        IR."NumberOfFlaggedAnswers",
                        IR."FollowUpStatus"::text,
                        IR."IsCompleted",
                        IR."LastUpdatedAt",
                        CASE
                            WHEN IR."NumberOfQuestionsAnswered" = 0 THEN 'None'::text
                            WHEN F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered" THEN 'All'::text
                            ELSE 'Some'::text
                        END,
                        PS."Level1"::text,
                        PS."Level2"::text,
                        PS."Level3"::text,
                        PS."Level4"::text,
                        PS."Level5"::text,
                        PS."Number"::text,
                        NC."Value",
                        AC."Value",
                        CC."Value",
                        NC."Value" > 0,
                        CC."Value" > 0,
                        AC."Value" > 0,
                        RST."Id" AS "StageId",
                        RST."Name"::text AS "StageName",
                        RST."Status"::text AS "ReviewStatus",
                        RWF."StageChangedAt",
                        RWF."ReviewedAt",
                        RAS."Value" AS "ReviewerIds"
                    FROM "IncidentReports" IR
                        LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                            AND RWF."RecordType" = 'IncidentReport' AND RWF."RecordId" = IR."Id"
                        LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                            FROM "Reviewers" X
                            WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'IncidentReport' AND X."RecordId" = IR."Id"
                        ) RAS
                        INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = IR."MonitoringObserverId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "IncidentReportNotes" X
                            WHERE X."IncidentReportId" = IR."Id"
                        ) NC
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "IncidentReportAttachments" X
                            WHERE X."IncidentReportId" = IR."Id"
                              AND X."IsDeleted" = FALSE
                              AND X."IsCompleted" = TRUE
                        ) AC
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "IncidentReportComments" X
                            WHERE X."IncidentReportId" = IR."Id"
                              AND (
                                  EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = X."CreatedBy" AND NA."NgoId" = ngoId)
                                  OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = X."CreatedBy" AND NS."NgoId" = ngoId)
                              )
                        ) CC
                    WHERE IR."ElectionRoundId" = electionRoundId;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetQuickReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                    UUID,
                                "ElectionRoundId"       UUID,
                                "MonitoringObserverId"  UUID,
                                "DisplayName"           TEXT,
                                "Email"                 TEXT,
                                "PhoneNumber"           TEXT,
                                "Tags"                  TEXT[],
                                "NgoId"                 UUID,
                                "NgoName"               TEXT,
                                "PollingStationId"      UUID,
                                "LocationType"          TEXT,
                                "IncidentCategory"      TEXT,
                                "FollowUpStatus"        TEXT,
                                "TimeSubmitted"         TIMESTAMP WITH TIME ZONE,
                                "Title"                 TEXT,
                                "Description"           TEXT,
                                "PollingStationDetails" TEXT,
                                "Level1"                TEXT,
                                "Level2"                TEXT,
                                "Level3"                TEXT,
                                "Level4"                TEXT,
                                "Level5"                TEXT,
                                "Number"                TEXT,
                                "Address"               TEXT,
                                "NumberOfAttachments"   BIGINT,
                                "CommentsCount"         BIGINT,
                                "HasComments"           BOOLEAN,
                                "HasAttachments"        BOOLEAN,
                                "StageId"                    UUID,
                                "StageName"                  TEXT,
                                "ReviewStatus"                 TEXT,
                                "StageChangedAt"             TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"                   TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"                UUID[]
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
                    SELECT
                        QR."Id",
                        QR."ElectionRoundId",
                        QR."MonitoringObserverId",
                        AMO."DisplayName",
                        AMO."Email",
                        AMO."PhoneNumber",
                        AMO."Tags",
                        AMO."NgoId",
                        AMO."NgoName"::text,
                        QR."PollingStationId",
                        QR."QuickReportLocationType"::text,
                        QR."IncidentCategory"::text,
                        QR."FollowUpStatus"::text,
                        QR."LastUpdatedAt",
                        QR."Title"::text,
                        QR."Description"::text,
                        QR."PollingStationDetails"::text,
                        PS."Level1"::text,
                        PS."Level2"::text,
                        PS."Level3"::text,
                        PS."Level4"::text,
                        PS."Level5"::text,
                        PS."Number"::text,
                        PS."Address"::text,
                        AC."Value",
                        CC."Value",
                        CC."Value" > 0,
                        AC."Value" > 0,
                        RST."Id" AS "StageId",
                        RST."Name"::text AS "StageName",
                        RST."Status"::text AS "ReviewStatus",
                        RWF."StageChangedAt",
                        RWF."ReviewedAt",
                        RAS."Value" AS "ReviewerIds"
                    FROM "QuickReports" QR
                        LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                            AND RWF."RecordType" = 'QuickReport' AND RWF."RecordId" = QR."Id"
                        LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                            FROM "Reviewers" X
                            WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'QuickReport' AND X."RecordId" = QR."Id"
                        ) RAS
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = QR."MonitoringObserverId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "QuickReportAttachments" X
                            WHERE X."QuickReportId" = QR."Id"
                              AND X."MonitoringObserverId" = QR."MonitoringObserverId"
                              AND X."IsDeleted" = FALSE
                              AND X."IsCompleted" = TRUE
                        ) AC
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "QuickReportComments" X
                            WHERE X."QuickReportId" = QR."Id"
                              AND (
                                  EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = X."CreatedBy" AND NA."NgoId" = ngoId)
                                  OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = X."CreatedBy" AND NS."NgoId" = ngoId)
                              )
                        ) CC
                    WHERE QR."ElectionRoundId" = electionRoundId;
                END;
                $$ LANGUAGE plpgsql;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionEntries"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportEntries"(uuid, uuid);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportEntries"(uuid, uuid, text);""");
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportEntries"(uuid, uuid, text);""");

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
                                "QuestionsAnswered" TEXT
                            )
                AS
                $$
                BEGIN
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
                END AS "QuestionsAnswered"
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
                END AS "QuestionsAnswered"
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

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetCitizenReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "ElectionRoundId"            UUID,
                                "FormId"                     UUID,
                                "FormCode"                   TEXT,
                                "FormName"                   JSONB,
                                "FormDefaultLanguage"        TEXT,
                                "FormType"                   TEXT,
                                "FormStatus"                 TEXT,
                                "Answers"                    JSONB,
                                "NumberOfQuestionsAnswered"  INTEGER,
                                "NumberOfFlaggedAnswers"     INTEGER,
                                "FollowUpStatus"             TEXT,
                                "TimeSubmitted"              TIMESTAMP WITH TIME ZONE,
                                "QuestionsAnswered"          TEXT,
                                "LocationId"                 UUID,
                                "Level1"                     TEXT,
                                "Level2"                     TEXT,
                                "Level3"                     TEXT,
                                "Level4"                     TEXT,
                                "Level5"                     TEXT,
                                "NotesCount"                 BIGINT,
                                "MediaFilesCount"            BIGINT,
                                "CommentsCount"              BIGINT,
                                "HasNotes"                   BOOLEAN,
                                "HasComments"                BOOLEAN,
                                "HasAttachments"             BOOLEAN,
                                "NgoId"                      UUID,
                                "CitizenReportingEnabled"    BOOLEAN
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY
                    SELECT
                        CR."Id",
                        CR."ElectionRoundId",
                        CR."FormId",
                        F."Code"::text,
                        F."Name",
                        F."DefaultLanguage"::text,
                        F."FormType"::text,
                        F."Status"::text,
                        CR."Answers",
                        CR."NumberOfQuestionsAnswered",
                        CR."NumberOfFlaggedAnswers",
                        CR."FollowUpStatus"::text,
                        COALESCE(CR."LastModifiedOn", CR."CreatedOn"),
                        CASE
                            WHEN CR."NumberOfQuestionsAnswered" = 0 THEN 'None'::text
                            WHEN F."NumberOfQuestions" = CR."NumberOfQuestionsAnswered" THEN 'All'::text
                            ELSE 'Some'::text
                        END,
                        L."Id",
                        L."Level1"::text,
                        L."Level2"::text,
                        L."Level3"::text,
                        L."Level4"::text,
                        L."Level5"::text,
                        NC."Value",
                        AC."Value",
                        CC."Value",
                        NC."Value" > 0,
                        CC."Value" > 0,
                        AC."Value" > 0,
                        MN."NgoId",
                        ER."CitizenReportingEnabled"
                    FROM "CitizenReports" CR
                        INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                        INNER JOIN "Locations" L ON L."Id" = CR."LocationId"
                        INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "CitizenReportNotes" X
                            WHERE X."CitizenReportId" = CR."Id"
                        ) NC
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "CitizenReportAttachments" X
                            WHERE X."CitizenReportId" = CR."Id"
                              AND X."IsDeleted" = FALSE
                              AND X."IsCompleted" = TRUE
                        ) AC
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "CitizenReportComments" X
                            WHERE X."CitizenReportId" = CR."Id"
                              AND (
                                  EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = X."CreatedBy" AND NA."NgoId" = ngoId)
                                  OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = X."CreatedBy" AND NS."NgoId" = ngoId)
                              )
                        ) CC
                    WHERE CR."ElectionRoundId" = electionRoundId
                      AND MN."NgoId" = ngoId
                      AND ER."CitizenReportingEnabled" = TRUE;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetIncidentReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "ElectionRoundId"            UUID,
                                "FormId"                     UUID,
                                "FormCode"                   TEXT,
                                "FormName"                   JSONB,
                                "FormDefaultLanguage"        TEXT,
                                "FormType"                   TEXT,
                                "FormStatus"                 TEXT,
                                "Answers"                    JSONB,
                                "MonitoringObserverId"       UUID,
                                "DisplayName"                TEXT,
                                "Email"                      TEXT,
                                "PhoneNumber"                TEXT,
                                "Tags"                       TEXT[],
                                "MonitoringObserverStatus"   TEXT,
                                "NgoId"                      UUID,
                                "NgoName"                    TEXT,
                                "PollingStationId"           UUID,
                                "LocationType"               TEXT,
                                "LocationDescription"        TEXT,
                                "NumberOfQuestionsAnswered"  INTEGER,
                                "NumberOfFlaggedAnswers"     INTEGER,
                                "FollowUpStatus"             TEXT,
                                "IsCompleted"                BOOLEAN,
                                "TimeSubmitted"              TIMESTAMP WITH TIME ZONE,
                                "QuestionsAnswered"          TEXT,
                                "Level1"                     TEXT,
                                "Level2"                     TEXT,
                                "Level3"                     TEXT,
                                "Level4"                     TEXT,
                                "Level5"                     TEXT,
                                "Number"                     TEXT,
                                "NotesCount"                 BIGINT,
                                "MediaFilesCount"            BIGINT,
                                "CommentsCount"              BIGINT,
                                "HasNotes"                   BOOLEAN,
                                "HasComments"                BOOLEAN,
                                "HasAttachments"             BOOLEAN
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY
                    SELECT
                        IR."Id",
                        IR."ElectionRoundId",
                        IR."FormId",
                        F."Code"::text,
                        F."Name",
                        F."DefaultLanguage"::text,
                        F."FormType"::text,
                        F."Status"::text,
                        IR."Answers",
                        IR."MonitoringObserverId",
                        AMO."DisplayName",
                        AMO."Email",
                        AMO."PhoneNumber",
                        AMO."Tags",
                        AMO."Status",
                        AMO."NgoId",
                        AMO."NgoName"::text,
                        IR."PollingStationId",
                        IR."LocationType"::text,
                        IR."LocationDescription"::text,
                        IR."NumberOfQuestionsAnswered",
                        IR."NumberOfFlaggedAnswers",
                        IR."FollowUpStatus"::text,
                        IR."IsCompleted",
                        IR."LastUpdatedAt",
                        CASE
                            WHEN IR."NumberOfQuestionsAnswered" = 0 THEN 'None'::text
                            WHEN F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered" THEN 'All'::text
                            ELSE 'Some'::text
                        END,
                        PS."Level1"::text,
                        PS."Level2"::text,
                        PS."Level3"::text,
                        PS."Level4"::text,
                        PS."Level5"::text,
                        PS."Number"::text,
                        NC."Value",
                        AC."Value",
                        CC."Value",
                        NC."Value" > 0,
                        CC."Value" > 0,
                        AC."Value" > 0
                    FROM "IncidentReports" IR
                        INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = IR."MonitoringObserverId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "IncidentReportNotes" X
                            WHERE X."IncidentReportId" = IR."Id"
                        ) NC
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "IncidentReportAttachments" X
                            WHERE X."IncidentReportId" = IR."Id"
                              AND X."IsDeleted" = FALSE
                              AND X."IsCompleted" = TRUE
                        ) AC
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "IncidentReportComments" X
                            WHERE X."IncidentReportId" = IR."Id"
                              AND (
                                  EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = X."CreatedBy" AND NA."NgoId" = ngoId)
                                  OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = X."CreatedBy" AND NS."NgoId" = ngoId)
                              )
                        ) CC
                    WHERE IR."ElectionRoundId" = electionRoundId;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetQuickReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                    UUID,
                                "ElectionRoundId"       UUID,
                                "MonitoringObserverId"  UUID,
                                "DisplayName"           TEXT,
                                "Email"                 TEXT,
                                "PhoneNumber"           TEXT,
                                "Tags"                  TEXT[],
                                "NgoId"                 UUID,
                                "NgoName"               TEXT,
                                "PollingStationId"      UUID,
                                "LocationType"          TEXT,
                                "IncidentCategory"      TEXT,
                                "FollowUpStatus"        TEXT,
                                "TimeSubmitted"         TIMESTAMP WITH TIME ZONE,
                                "Title"                 TEXT,
                                "Description"           TEXT,
                                "PollingStationDetails" TEXT,
                                "Level1"                TEXT,
                                "Level2"                TEXT,
                                "Level3"                TEXT,
                                "Level4"                TEXT,
                                "Level5"                TEXT,
                                "Number"                TEXT,
                                "Address"               TEXT,
                                "NumberOfAttachments"   BIGINT,
                                "CommentsCount"         BIGINT,
                                "HasComments"           BOOLEAN,
                                "HasAttachments"        BOOLEAN
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY
                    SELECT
                        QR."Id",
                        QR."ElectionRoundId",
                        QR."MonitoringObserverId",
                        AMO."DisplayName",
                        AMO."Email",
                        AMO."PhoneNumber",
                        AMO."Tags",
                        AMO."NgoId",
                        AMO."NgoName"::text,
                        QR."PollingStationId",
                        QR."QuickReportLocationType"::text,
                        QR."IncidentCategory"::text,
                        QR."FollowUpStatus"::text,
                        QR."LastUpdatedAt",
                        QR."Title"::text,
                        QR."Description"::text,
                        QR."PollingStationDetails"::text,
                        PS."Level1"::text,
                        PS."Level2"::text,
                        PS."Level3"::text,
                        PS."Level4"::text,
                        PS."Level5"::text,
                        PS."Number"::text,
                        PS."Address"::text,
                        AC."Value",
                        CC."Value",
                        CC."Value" > 0,
                        AC."Value" > 0
                    FROM "QuickReports" QR
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = QR."MonitoringObserverId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "QuickReportAttachments" X
                            WHERE X."QuickReportId" = QR."Id"
                              AND X."MonitoringObserverId" = QR."MonitoringObserverId"
                              AND X."IsDeleted" = FALSE
                              AND X."IsCompleted" = TRUE
                        ) AC
                        CROSS JOIN LATERAL (
                            SELECT COUNT(1) AS "Value"
                            FROM "QuickReportComments" X
                            WHERE X."QuickReportId" = QR."Id"
                              AND (
                                  EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = X."CreatedBy" AND NA."NgoId" = ngoId)
                                  OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = X."CreatedBy" AND NS."NgoId" = ngoId)
                              )
                        ) CC
                    WHERE QR."ElectionRoundId" = electionRoundId;
                END;
                $$ LANGUAGE plpgsql;
                """);
        }
    }
}
