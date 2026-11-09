using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordLabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Record labels (ADR 23 in the web app): each monitoring NGO's free-form labels on its records, kept in the
            // review history (AddedLabels / RemovedLabels) and returned by the report lists.
            migrationBuilder.AddColumn<string[]>(
                name: "AddedLabels",
                table: "ReviewActivities",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "RemovedLabels",
                table: "ReviewActivities",
                type: "text[]",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecordLabels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElectionRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoringNgoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordType = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordLabels_ElectionRounds_ElectionRoundId",
                        column: x => x.ElectionRoundId,
                        principalTable: "ElectionRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordLabels_MonitoringNgos_MonitoringNgoId",
                        column: x => x.MonitoringNgoId,
                        principalTable: "MonitoringNgos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordLabels_ElectionRoundId_MonitoringNgoId",
                table: "RecordLabels",
                columns: new[] { "ElectionRoundId", "MonitoringNgoId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecordLabels_MonitoringNgoId",
                table: "RecordLabels",
                column: "MonitoringNgoId");

            // A record's labels are a set, compared case-insensitively (EF can't model an expression index).
            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX "IX_RecordLabels_MonitoringNgoId_RecordType_RecordId_Label"
                    ON "RecordLabels" ("MonitoringNgoId", "RecordType", "RecordId", lower("Label"));
                """);

            migrationBuilder.Sql(
                """
                -- The NGO's labels on a record, A to Z (ignoring case); an empty array when it has none. Always one value.
                CREATE OR REPLACE FUNCTION "GetRecordLabels"(
                    electionRoundId UUID,
                    ngoId UUID,
                    recordType TEXT,
                    recordId UUID
                )
                    RETURNS TEXT[]
                AS
                $$
                SELECT COALESCE((SELECT array_agg(RL."Label"::text ORDER BY lower(RL."Label"), RL."Label")
                                 FROM "RecordLabels" RL
                                     INNER JOIN "MonitoringNgos" LMN ON LMN."Id" = RL."MonitoringNgoId"
                                 WHERE LMN."ElectionRoundId" = electionRoundId
                                   AND LMN."NgoId" = ngoId
                                   AND RL."RecordType" = recordType
                                   AND RL."RecordId" = recordId), '{}'::text[])
                $$ LANGUAGE sql STABLE;
                """);

            // The report-list functions return the caller NGO's labels as a new "Labels" column (after "Severity");
            // everything else is unchanged from their latest definitions (AddSubmissionNumberToFormSubmissionEntries,
            // AddRecordIdToReportEntries). New result columns need the functions dropped first.

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
                                "Labels"                     TEXT[],
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
                               RLB."Value" AS "Labels",
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
                                 CROSS JOIN LATERAL (
                                     SELECT COALESCE(array_agg(RLX."Label"::text ORDER BY lower(RLX."Label"), RLX."Label"), '{}'::text[]) AS "Value"
                                     FROM "RecordLabels" RLX
                                     WHERE RLX."MonitoringNgoId" = callerMonitoringNgoId AND RLX."RecordType" = 'FormSubmission' AND RLX."RecordId" = psi."Id"
                                 ) RLB
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
                               RLB."Value" AS "Labels",
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
                                 CROSS JOIN LATERAL (
                                     SELECT COALESCE(array_agg(RLX."Label"::text ORDER BY lower(RLX."Label"), RLX."Label"), '{}'::text[]) AS "Value"
                                     FROM "RecordLabels" RLX
                                     WHERE RLX."MonitoringNgoId" = callerMonitoringNgoId AND RLX."RecordType" = 'FormSubmission' AND RLX."RecordId" = fs."Id"
                                 ) RLB
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

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportEntries"(uuid, uuid, uuid);""");

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetCitizenReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    recordId UUID DEFAULT NULL
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
                                "ReviewerIds"                UUID[],
                                "Severity"                   TEXT,
                                "Labels"                     TEXT[]
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
                        RAS."Value" AS "ReviewerIds",
                        COALESCE(RSV."Severity", 'None')::text AS "Severity",
                        RLB."Value" AS "Labels"
                    FROM "CitizenReports" CR
                        LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                            AND RWF."RecordType" = 'CitizenReport' AND RWF."RecordId" = CR."Id"
                        LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                            FROM "Reviewers" X
                            WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'CitizenReport' AND X."RecordId" = CR."Id"
                        ) RAS
                        LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = callerMonitoringNgoId
                            AND RSV."RecordType" = 'CitizenReport' AND RSV."RecordId" = CR."Id"
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(RLX."Label"::text ORDER BY lower(RLX."Label"), RLX."Label"), '{}'::text[]) AS "Value"
                            FROM "RecordLabels" RLX
                            WHERE RLX."MonitoringNgoId" = callerMonitoringNgoId AND RLX."RecordType" = 'CitizenReport' AND RLX."RecordId" = CR."Id"
                        ) RLB
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
                      AND (recordId IS NULL OR CR."Id" = recordId)
                      AND MN."NgoId" = ngoId
                      AND ER."CitizenReportingEnabled" = TRUE;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportEntries"(uuid, uuid, text, uuid);""");

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetIncidentReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    recordId UUID DEFAULT NULL
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
                                "ReviewerIds"                UUID[],
                                "Severity"                   TEXT,
                                "Labels"                     TEXT[]
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
                        RAS."Value" AS "ReviewerIds",
                        COALESCE(RSV."Severity", 'None')::text AS "Severity",
                        RLB."Value" AS "Labels"
                    FROM "IncidentReports" IR
                        LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                            AND RWF."RecordType" = 'IncidentReport' AND RWF."RecordId" = IR."Id"
                        LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                            FROM "Reviewers" X
                            WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'IncidentReport' AND X."RecordId" = IR."Id"
                        ) RAS
                        LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = callerMonitoringNgoId
                            AND RSV."RecordType" = 'IncidentReport' AND RSV."RecordId" = IR."Id"
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(RLX."Label"::text ORDER BY lower(RLX."Label"), RLX."Label"), '{}'::text[]) AS "Value"
                            FROM "RecordLabels" RLX
                            WHERE RLX."MonitoringNgoId" = callerMonitoringNgoId AND RLX."RecordType" = 'IncidentReport' AND RLX."RecordId" = IR."Id"
                        ) RLB
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
                    WHERE IR."ElectionRoundId" = electionRoundId
                      AND (recordId IS NULL OR IR."Id" = recordId);
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportEntries"(uuid, uuid, text, uuid);""");

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetQuickReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    recordId UUID DEFAULT NULL
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
                                "ReviewerIds"                UUID[],
                                "Severity"                   TEXT,
                                "Labels"                     TEXT[]
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
                        RAS."Value" AS "ReviewerIds",
                        COALESCE(RSV."Severity", 'None')::text AS "Severity",
                        RLB."Value" AS "Labels"
                    FROM "QuickReports" QR
                        LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                            AND RWF."RecordType" = 'QuickReport' AND RWF."RecordId" = QR."Id"
                        LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                            FROM "Reviewers" X
                            WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'QuickReport' AND X."RecordId" = QR."Id"
                        ) RAS
                        LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = callerMonitoringNgoId
                            AND RSV."RecordType" = 'QuickReport' AND RSV."RecordId" = QR."Id"
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(RLX."Label"::text ORDER BY lower(RLX."Label"), RLX."Label"), '{}'::text[]) AS "Value"
                            FROM "RecordLabels" RLX
                            WHERE RLX."MonitoringNgoId" = callerMonitoringNgoId AND RLX."RecordType" = 'QuickReport' AND RLX."RecordId" = QR."Id"
                        ) RLB
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
                    WHERE QR."ElectionRoundId" = electionRoundId
                      AND (recordId IS NULL OR QR."Id" = recordId);
                END;
                $$ LANGUAGE plpgsql;
                """);

            // The incident reports list (by entry, V1) takes an optional labels filter (any of, ignoring case) and returns
            // each report's "Labels"; otherwise unchanged from MoveEndpointQueriesToFunctions.

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportListEntriesCount"(uuid, uuid, uuid, text, text, text, text, text, text, text, uuid, boolean, text, text[], text, uuid, text, text, text, boolean, boolean, boolean, text, text, uuid, text, uuid, boolean, text[]);""");

            migrationBuilder.Sql(
                """
                -- Counts the NGO's incident reports matching the incident reports list (by entry) filters.
                CREATE OR REPLACE FUNCTION "GetIncidentReportListEntriesCount"(
                    electionRoundId UUID,
                    ngoId UUID,
                    monitoringObserverId UUID,
                    searchText TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    pollingStationId UUID,
                    hasFlaggedAnswers BOOLEAN,
                    followUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    formStatus TEXT,
                    questionsAnswered TEXT,
                    locationType TEXT,
                    hasAttachments BOOLEAN,
                    hasNotes BOOLEAN,
                    hasComments BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    stageId UUID,
                    reviewStatus TEXT,
                    reviewerId UUID,
                    hasReviewer BOOLEAN,
                    severities TEXT[],
                    labels TEXT[] DEFAULT NULL
                )
                    RETURNS TABLE
                            (
                                "Count" BIGINT
                            )
                AS
                $$
                SELECT
                    COUNT(*)
                FROM
                    "IncidentReports" IR
                        INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                        INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                        LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                LEFT JOIN LATERAL "GetRecordReviewWorkflow"(electionRoundId, ngoId, 'IncidentReport', IR."Id") WF ON TRUE
                LEFT JOIN LATERAL "GetRecordSeverity"(electionRoundId, ngoId, 'IncidentReport', IR."Id") SV ON TRUE
                WHERE
                    MN."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                  AND (monitoringObserverId IS NULL OR MO."Id" = monitoringObserverId)
                  AND (searchText IS NULL
                        OR searchText = ''
                        OR U."DisplayName" ILIKE searchText
                        OR U."Email" ILIKE searchText
                        OR U."PhoneNumber" ILIKE searchText
                        OR MO."Id"::TEXT ILIKE searchText
                    )
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                  AND (pollingStationNumber IS NULL OR PS."Number" = pollingStationNumber)
                  AND (pollingStationId IS NULL OR IR."PollingStationId" = pollingStationId)
                  AND (hasFlaggedAnswers IS NULL
                        OR (IR."NumberOfFlaggedAnswers" = 0 AND hasFlaggedAnswers = FALSE)
                        OR ("NumberOfFlaggedAnswers" > 0 AND hasFlaggedAnswers = TRUE)
                    )
                  AND (followUpStatus IS NULL OR IR."FollowUpStatus" = followUpStatus)
                  AND (tagsFilter IS NULL OR CARDINALITY(tagsFilter) = 0 OR MO."Tags" && tagsFilter)
                  AND (monitoringObserverStatus IS NULL OR MO."Status" = monitoringObserverStatus)
                  AND (formId IS NULL OR IR."FormId" = formId)
                  AND (formStatus IS NULL OR F."Status" = formStatus)
                  AND (questionsAnswered IS NULL
                        OR (questionsAnswered = 'All' AND F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered")
                        OR (questionsAnswered = 'Some' AND F."NumberOfQuestions" <> IR."NumberOfQuestionsAnswered")
                        OR (questionsAnswered = 'None' AND IR."NumberOfQuestionsAnswered" = 0)
                    )
                  AND (locationType IS NULL OR IR."LocationType" = locationType)
                  AND (hasAttachments IS NULL
                        OR (hasAttachments = FALSE AND
                        (
                            SELECT
                                COUNT(1)
                            FROM
                                "IncidentReportAttachments" A
                            WHERE
                                A."IncidentReportId" = IR."Id"
                              AND IR."PollingStationId" = "PollingStationId"
                              AND A."IsDeleted" = FALSE
                              AND A."IsCompleted" = TRUE
                        ) = 0)
                        OR (
                        (hasAttachments = TRUE AND(
                            SELECT
                                COUNT(1)
                            FROM
                                "IncidentReportAttachments" A
                            WHERE
                                A."IncidentReportId" = IR."Id"
                              AND A."IsDeleted" = FALSE
                              AND A."IsCompleted" = TRUE
                        ) > 0)
                    )
                  AND (hasNotes IS NULL
                        OR (hasNotes = FALSE AND (SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") = 0)
                        OR (hasNotes = TRUE AND (SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") > 0 )
                    ))
                  AND (hasComments IS NULL
                        OR (hasComments = FALSE AND (SELECT COUNT(1) FROM "IncidentReportComments" ICC INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = ICC."CreatedBy" AND NA."NgoId" = ngoId WHERE ICC."IncidentReportId" = IR."Id") = 0)
                        OR (hasComments = TRUE AND (SELECT COUNT(1) FROM "IncidentReportComments" ICC INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = ICC."CreatedBy" AND NA."NgoId" = ngoId WHERE ICC."IncidentReportId" = IR."Id") > 0)
                    )
                  AND (fromDate is NULL OR IR."LastUpdatedAt" >= fromDate::timestamp)
                  AND (stageId IS NULL OR WF."StageId" = stageId)
                  AND (reviewStatus IS NULL OR WF."ReviewStatus" = reviewStatus)
                  AND (reviewerId IS NULL OR reviewerId = ANY(WF."ReviewerIds"))
                  AND (hasReviewer IS NULL OR (cardinality(WF."ReviewerIds") > 0) = hasReviewer)
                  AND (severities IS NULL OR cardinality(severities) = 0 OR SV."Severity" = ANY(severities))
                  AND (labels IS NULL OR cardinality(labels) = 0 OR EXISTS (SELECT 1 FROM "RecordLabels" RLX INNER JOIN "MonitoringNgos" LMN ON LMN."Id" = RLX."MonitoringNgoId" WHERE LMN."ElectionRoundId" = electionRoundId AND LMN."NgoId" = ngoId AND RLX."RecordType" = 'IncidentReport' AND RLX."RecordId" = IR."Id" AND lower(RLX."Label") = ANY (SELECT lower(x) FROM unnest(labels) x)))
                  AND (toDate is NULL OR IR."LastUpdatedAt" <= toDate::timestamp)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportListEntries"(uuid, uuid, uuid, text, text, text, text, text, text, text, uuid, boolean, text, text[], text, uuid, text, text, text, boolean, boolean, boolean, text, text, uuid, text, uuid, boolean, text[], text, integer, integer);""");

            migrationBuilder.Sql(
                """
                -- One page of the NGO's incident reports for the incident reports list (by entry), with each report's review
                -- workflow and severity, filtered and ordered by sortExpression ('<Column> ASC|DESC').
                CREATE OR REPLACE FUNCTION "GetIncidentReportListEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    monitoringObserverId UUID,
                    searchText TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    pollingStationId UUID,
                    hasFlaggedAnswers BOOLEAN,
                    followUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    formStatus TEXT,
                    questionsAnswered TEXT,
                    locationType TEXT,
                    hasAttachments BOOLEAN,
                    hasNotes BOOLEAN,
                    hasComments BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    stageId UUID,
                    reviewStatus TEXT,
                    reviewerId UUID,
                    hasReviewer BOOLEAN,
                    severities TEXT[],
                    sortExpression TEXT,
                    offsetRows INTEGER,
                    pageSize INTEGER,
                    labels TEXT[] DEFAULT NULL
                )
                    RETURNS TABLE
                            (
                                "IncidentReportId"          UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormCode"                  TEXT,
                                "FormStatus"                TEXT,
                                "FormName"                  JSONB,
                                "FormDefaultLanguage"       TEXT,
                                "LocationType"              TEXT,
                                "LocationDescription"       TEXT,
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "PollingStationNumber"      TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "Email"                     TEXT,
                                "PhoneNumber"               TEXT,
                                "Status"                    TEXT,
                                "Tags"                      TEXT[],
                                "NumberOfQuestionsAnswered" INTEGER,
                                "NumberOfFlaggedAnswers"    INTEGER,
                                "MediaFilesCount"           BIGINT,
                                "NotesCount"                BIGINT,
                                "CommentsCount"             BIGINT,
                                "FollowUpStatus"            TEXT,
                                "IsCompleted"               BOOLEAN,
                                "StageId"                   UUID,
                                "StageName"                 TEXT,
                                "ReviewStatus"              TEXT,
                                "StageChangedAt"            TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"                TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"               UUID[],
                                "Severity"                  TEXT,
                                "Labels"                    TEXT[]
                            )
                AS
                $$
                WITH
                    INCIDENT_REPORTS AS (
                        SELECT
                            IR."Id" AS "IncidentReportId",
                            F."Code" AS "FormCode",
                            F."Status" AS "FormStatus",
                            F."Name" AS "FormName",
                            F."DefaultLanguage" "FormDefaultLanguage",
                            IR."LocationType",
                            IR."LocationDescription",
                            IR."PollingStationId",
                            IR."MonitoringObserverId",
                            IR."NumberOfQuestionsAnswered",
                            IR."NumberOfFlaggedAnswers",
                            (
                                SELECT
                                    COUNT(1)
                                FROM
                                    "IncidentReportAttachments" A
                                WHERE
                                    A."IncidentReportId" = IR."Id"
                                  AND A."IsDeleted" = FALSE
                                  AND A."IsCompleted" = TRUE
                            ) AS "MediaFilesCount",
                            ( SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") AS "NotesCount",
                            ( SELECT COUNT(1)
                              FROM "IncidentReportComments" ICC
                                  INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = ICC."CreatedBy"
                                      AND NA."NgoId" = ngoId
                              WHERE ICC."IncidentReportId" = IR."Id") AS "CommentsCount",
                            IR."LastUpdatedAt" AS "TimeSubmitted",
                            IR."FollowUpStatus",
                            IR."IsCompleted",
                            WF."StageId", WF."StageName", WF."ReviewStatus", WF."StageChangedAt", WF."ReviewedAt", WF."ReviewerIds", SV."Severity"
                        FROM
                            "IncidentReports" IR
                                INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                                INNER JOIN "MonitoringObservers" MO ON IR."MonitoringObserverId" = MO."Id"
                                INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        LEFT JOIN LATERAL "GetRecordReviewWorkflow"(electionRoundId, ngoId, 'IncidentReport', IR."Id") WF ON TRUE
                        LEFT JOIN LATERAL "GetRecordSeverity"(electionRoundId, ngoId, 'IncidentReport', IR."Id") SV ON TRUE
                        WHERE
                          MN."ElectionRoundId" = electionRoundId
                          AND MN."NgoId" = ngoId
                          AND (monitoringObserverId IS NULL OR MO."Id" = monitoringObserverId)
                          AND (monitoringObserverStatus IS NULL OR MO."Status" = monitoringObserverStatus)
                          AND (formId IS NULL OR IR."FormId" = formId)
                          AND (formStatus IS NULL OR F."Status" = formStatus)
                          AND (locationType IS NULL OR IR."LocationType" = locationType)
                          AND (questionsAnswered IS NULL
                              OR (questionsAnswered = 'All' AND F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered")
                              OR (questionsAnswered = 'Some' AND F."NumberOfQuestions" <> IR."NumberOfQuestionsAnswered")
                              OR (questionsAnswered = 'None' AND IR."NumberOfQuestionsAnswered" = 0)
                          )
                          AND (fromDate is NULL OR IR."LastUpdatedAt" >= fromDate::timestamp)
                          AND (stageId IS NULL OR WF."StageId" = stageId)
                          AND (reviewStatus IS NULL OR WF."ReviewStatus" = reviewStatus)
                          AND (reviewerId IS NULL OR reviewerId = ANY(WF."ReviewerIds"))
                          AND (hasReviewer IS NULL OR (cardinality(WF."ReviewerIds") > 0) = hasReviewer)
                          AND (severities IS NULL OR cardinality(severities) = 0 OR SV."Severity" = ANY(severities))
                          AND (labels IS NULL OR cardinality(labels) = 0 OR EXISTS (SELECT 1 FROM "RecordLabels" RLX INNER JOIN "MonitoringNgos" LMN ON LMN."Id" = RLX."MonitoringNgoId" WHERE LMN."ElectionRoundId" = electionRoundId AND LMN."NgoId" = ngoId AND RLX."RecordType" = 'IncidentReport' AND RLX."RecordId" = IR."Id" AND lower(RLX."Label") = ANY (SELECT lower(x) FROM unnest(labels) x)))
                          AND (toDate is NULL OR IR."LastUpdatedAt" <= toDate::timestamp)
                    )
                SELECT
                    IR."IncidentReportId",
                    IR."TimeSubmitted",
                    IR."FormCode"::text,
                    IR."FormStatus",
                    IR."FormName",
                    IR."FormDefaultLanguage"::text,
                    IR."LocationType",
                    IR."LocationDescription"::text,
                    PS."Id" AS "PollingStationId",
                    PS."Level1"::text,
                    PS."Level2"::text,
                    PS."Level3"::text,
                    PS."Level4"::text,
                    PS."Level5"::text,
                    PS."Number"::text "PollingStationNumber",
                    IR."MonitoringObserverId",
                    U."DisplayName" AS "ObserverName",
                    U."Email"::text,
                    U."PhoneNumber",
                    MO."Status",
                    MO."Tags",
                    IR."NumberOfQuestionsAnswered",
                    IR."NumberOfFlaggedAnswers",
                    IR."MediaFilesCount",
                    IR."NotesCount",
                    IR."CommentsCount",
                    IR."FollowUpStatus",
                    IR."IsCompleted",
                    IR."StageId", IR."StageName", IR."ReviewStatus", IR."StageChangedAt", IR."ReviewedAt", IR."ReviewerIds", IR."Severity",
                    "GetRecordLabels"(electionRoundId, ngoId, 'IncidentReport', IR."IncidentReportId") AS "Labels"
                FROM
                    INCIDENT_REPORTS IR
                        INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                        LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                WHERE
                    MN."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                  AND (monitoringObserverId IS NULL OR MO."Id" = monitoringObserverId)
                  AND (
                    searchText IS NULL
                        OR searchText = ''
                        OR U."DisplayName" ILIKE searchText
                        OR U."Email" ILIKE searchText
                        OR U."PhoneNumber" ILIKE searchText
                        OR MO."Id"::TEXT ILIKE searchText
                    )
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                  AND (pollingStationNumber IS NULL OR PS."Number" = pollingStationNumber)
                  AND (pollingStationId IS NULL OR IR."PollingStationId" = pollingStationId)
                  AND (hasFlaggedAnswers IS NULL
                    OR (IR."NumberOfFlaggedAnswers" = 0 AND hasFlaggedAnswers = FALSE)
                    OR (IR."NumberOfFlaggedAnswers" > 0 AND hasFlaggedAnswers = TRUE)
                    )
                  AND (followUpStatus IS NULL OR IR."FollowUpStatus" = followUpStatus)
                  AND (tagsFilter IS NULL OR CARDINALITY(tagsFilter) = 0 OR MO."Tags" && tagsFilter)
                  AND (hasNotes IS NULL
                    OR (IR."NotesCount" = 0 AND hasNotes = FALSE)
                    OR (IR."NotesCount" > 0 AND hasNotes = TRUE)
                    )
                  AND (hasAttachments IS NULL
                    OR (IR."MediaFilesCount" = 0 AND hasAttachments = FALSE)
                    OR (IR."MediaFilesCount" > 0 AND hasAttachments = TRUE)
                    )
                  AND (hasComments IS NULL
                    OR (IR."CommentsCount" = 0 AND hasComments = FALSE)
                    OR (IR."CommentsCount" > 0 AND hasComments = TRUE)
                    )
                ORDER BY
                    CASE WHEN sortExpression = 'Severity ASC' THEN "SeverityRank"(IR."Severity") END ASC,
                    CASE WHEN sortExpression = 'Severity DESC' THEN "SeverityRank"(IR."Severity") END DESC,
                    CASE WHEN sortExpression LIKE 'Severity %' THEN IR."TimeSubmitted" END DESC,
                    CASE WHEN sortExpression = 'TimeSubmitted ASC' THEN IR."TimeSubmitted" END ASC,
                    CASE WHEN sortExpression = 'TimeSubmitted DESC' THEN IR."TimeSubmitted" END DESC,
                    CASE WHEN sortExpression = 'FormCode ASC' THEN IR."FormCode" END ASC,
                    CASE WHEN sortExpression = 'FormCode DESC' THEN IR."FormCode" END DESC,
                    CASE WHEN sortExpression = 'NumberOfQuestionsAnswered ASC' THEN IR."NumberOfQuestionsAnswered" END ASC,
                    CASE WHEN sortExpression = 'NumberOfQuestionsAnswered DESC' THEN IR."NumberOfQuestionsAnswered" END DESC,
                    CASE WHEN sortExpression = 'NumberOfFlaggedAnswers ASC' THEN IR."NumberOfFlaggedAnswers" END ASC,
                    CASE WHEN sortExpression = 'NumberOfFlaggedAnswers DESC' THEN IR."NumberOfFlaggedAnswers" END DESC,
                    CASE WHEN sortExpression = 'MediaFilesCount ASC' THEN IR."MediaFilesCount" END ASC,
                    CASE WHEN sortExpression = 'MediaFilesCount DESC' THEN IR."MediaFilesCount" END DESC,
                    CASE WHEN sortExpression = 'NotesCount ASC' THEN IR."NotesCount" END ASC,
                    CASE WHEN sortExpression = 'NotesCount DESC' THEN IR."NotesCount" END DESC,
                    CASE WHEN sortExpression = 'Level1 ASC' THEN PS."Level1" END ASC,
                    CASE WHEN sortExpression = 'Level1 DESC' THEN PS."Level1" END DESC,
                    CASE WHEN sortExpression = 'Level2 ASC' THEN PS."Level2" END ASC,
                    CASE WHEN sortExpression = 'Level2 DESC' THEN PS."Level2" END DESC,
                    CASE WHEN sortExpression = 'Level3 ASC' THEN PS."Level3" END ASC,
                    CASE WHEN sortExpression = 'Level3 DESC' THEN PS."Level3" END DESC,
                    CASE WHEN sortExpression = 'Level4 ASC' THEN PS."Level4" END ASC,
                    CASE WHEN sortExpression = 'Level4 DESC' THEN PS."Level4" END DESC,
                    CASE WHEN sortExpression = 'Level5 ASC' THEN PS."Level5" END ASC,
                    CASE WHEN sortExpression = 'Level5 DESC' THEN PS."Level5" END DESC,
                    CASE WHEN sortExpression = 'PollingStationNumber ASC' THEN PS."Number" END ASC,
                    CASE WHEN sortExpression = 'PollingStationNumber DESC' THEN PS."Number" END DESC,
                    CASE WHEN sortExpression = 'ObserverName ASC' THEN U."DisplayName" END ASC,
                    CASE WHEN sortExpression = 'ObserverName DESC' THEN U."DisplayName" END DESC
                OFFSET offsetRows ROWS
                FETCH NEXT pageSize ROWS ONLY
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- Counts the records the report lists show the NGO for the data source (the same records as "GetReviewMetrics"), per
                -- record type and label: a record with several labels counts once under each. The records without a label count in one
                -- row per record type with a null label. Citizen reports count only when the NGO does citizen reporting in the round.
                CREATE OR REPLACE FUNCTION "GetLabelMetrics"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT
                )
                    RETURNS TABLE
                            (
                                "RecordType" TEXT,
                                "Label"      TEXT,
                                "Count"      INTEGER
                            )
                AS
                $$
                WITH MN AS (
                    SELECT M."Id" AS "MonitoringNgoId"
                    FROM "MonitoringNgos" M
                    WHERE M."ElectionRoundId" = electionRoundId AND M."NgoId" = ngoId
                ), R AS (
                    SELECT 'FormSubmission' AS "RecordType", PSI."Id" AS "RecordId"
                    FROM "PollingStationInformation" PSI
                        INNER JOIN "PollingStationInformationForms" PSIF ON PSIF."Id" = PSI."PollingStationInformationFormId"
                        INNER JOIN "PollingStations" PS ON PS."Id" = PSI."PollingStationId"
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = PSI."MonitoringObserverId"
                        INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF
                            ON AF."FormId" = PSI."PollingStationInformationFormId"
                    WHERE PSI."ElectionRoundId" = electionRoundId
                    UNION ALL
                    SELECT 'FormSubmission', FS."Id"
                    FROM "FormSubmissions" FS
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                        INNER JOIN "PollingStations" PS ON PS."Id" = FS."PollingStationId"
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = FS."MonitoringObserverId"
                        INNER JOIN "GetAvailableForms"(electionRoundId, ngoId, dataSource) AF
                            ON AF."FormId" = FS."FormId"
                    WHERE FS."ElectionRoundId" = electionRoundId
                    UNION ALL
                    SELECT 'QuickReport', QR."Id"
                    FROM "QuickReports" QR
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = QR."MonitoringObserverId"
                    WHERE QR."ElectionRoundId" = electionRoundId
                    UNION ALL
                    SELECT 'IncidentReport', IR."Id"
                    FROM "IncidentReports" IR
                        INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                        INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO
                            ON AMO."MonitoringObserverId" = IR."MonitoringObserverId"
                    WHERE IR."ElectionRoundId" = electionRoundId
                    UNION ALL
                    SELECT 'CitizenReport', CR."Id"
                    FROM "CitizenReports" CR
                        INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                        INNER JOIN "Locations" L ON L."Id" = CR."LocationId"
                        INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                        INNER JOIN "MonitoringNgos" CMN ON CMN."Id" = ER."MonitoringNgoForCitizenReportingId"
                    WHERE CR."ElectionRoundId" = electionRoundId
                      AND CMN."NgoId" = ngoId
                      AND ER."CitizenReportingEnabled" = TRUE
                )
                SELECT R."RecordType", RL."Label"::text, COUNT(DISTINCT R."RecordId")::int
                FROM R
                    CROSS JOIN MN
                    LEFT JOIN "RecordLabels" RL ON RL."MonitoringNgoId" = MN."MonitoringNgoId"
                        AND RL."RecordType" = R."RecordType" AND RL."RecordId" = R."RecordId"
                GROUP BY R."RecordType", RL."Label"
                $$ LANGUAGE sql STABLE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetLabelMetrics"(uuid, uuid, text);""");

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

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportEntries"(uuid, uuid, uuid);""");

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetCitizenReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    recordId UUID DEFAULT NULL
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
                        RAS."Value" AS "ReviewerIds",
                        COALESCE(RSV."Severity", 'None')::text AS "Severity"
                    FROM "CitizenReports" CR
                        LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                            AND RWF."RecordType" = 'CitizenReport' AND RWF."RecordId" = CR."Id"
                        LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                            FROM "Reviewers" X
                            WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'CitizenReport' AND X."RecordId" = CR."Id"
                        ) RAS
                        LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = callerMonitoringNgoId
                            AND RSV."RecordType" = 'CitizenReport' AND RSV."RecordId" = CR."Id"
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
                      AND (recordId IS NULL OR CR."Id" = recordId)
                      AND MN."NgoId" = ngoId
                      AND ER."CitizenReportingEnabled" = TRUE;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportEntries"(uuid, uuid, text, uuid);""");

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetIncidentReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    recordId UUID DEFAULT NULL
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
                        RAS."Value" AS "ReviewerIds",
                        COALESCE(RSV."Severity", 'None')::text AS "Severity"
                    FROM "IncidentReports" IR
                        LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                            AND RWF."RecordType" = 'IncidentReport' AND RWF."RecordId" = IR."Id"
                        LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                            FROM "Reviewers" X
                            WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'IncidentReport' AND X."RecordId" = IR."Id"
                        ) RAS
                        LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = callerMonitoringNgoId
                            AND RSV."RecordType" = 'IncidentReport' AND RSV."RecordId" = IR."Id"
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
                    WHERE IR."ElectionRoundId" = electionRoundId
                      AND (recordId IS NULL OR IR."Id" = recordId);
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportEntries"(uuid, uuid, text, uuid);""");

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION "GetQuickReportEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    dataSource TEXT,
                    recordId UUID DEFAULT NULL
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
                        RAS."Value" AS "ReviewerIds",
                        COALESCE(RSV."Severity", 'None')::text AS "Severity"
                    FROM "QuickReports" QR
                        LEFT JOIN "ReviewWorkflows" RWF ON RWF."MonitoringNgoId" = callerMonitoringNgoId
                            AND RWF."RecordType" = 'QuickReport' AND RWF."RecordId" = QR."Id"
                        LEFT JOIN "ReviewStages" RST ON RST."Id" = COALESCE(RWF."StageId", defaultStageId)
                        CROSS JOIN LATERAL (
                            SELECT COALESCE(array_agg(X."UserId" ORDER BY X."AssignedAt"), '{}'::uuid[]) AS "Value"
                            FROM "Reviewers" X
                            WHERE X."MonitoringNgoId" = callerMonitoringNgoId AND X."RecordType" = 'QuickReport' AND X."RecordId" = QR."Id"
                        ) RAS
                        LEFT JOIN "RecordSeverities" RSV ON RSV."MonitoringNgoId" = callerMonitoringNgoId
                            AND RSV."RecordType" = 'QuickReport' AND RSV."RecordId" = QR."Id"
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
                    WHERE QR."ElectionRoundId" = electionRoundId
                      AND (recordId IS NULL OR QR."Id" = recordId);
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportListEntriesCount"(uuid, uuid, uuid, text, text, text, text, text, text, text, uuid, boolean, text, text[], text, uuid, text, text, text, boolean, boolean, boolean, text, text, uuid, text, uuid, boolean, text[], text[]);""");

            migrationBuilder.Sql(
                """
                -- Counts the NGO's incident reports matching the incident reports list (by entry) filters.
                CREATE OR REPLACE FUNCTION "GetIncidentReportListEntriesCount"(
                    electionRoundId UUID,
                    ngoId UUID,
                    monitoringObserverId UUID,
                    searchText TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    pollingStationId UUID,
                    hasFlaggedAnswers BOOLEAN,
                    followUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    formStatus TEXT,
                    questionsAnswered TEXT,
                    locationType TEXT,
                    hasAttachments BOOLEAN,
                    hasNotes BOOLEAN,
                    hasComments BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    stageId UUID,
                    reviewStatus TEXT,
                    reviewerId UUID,
                    hasReviewer BOOLEAN,
                    severities TEXT[]
                )
                    RETURNS TABLE
                            (
                                "Count" BIGINT
                            )
                AS
                $$
                SELECT
                    COUNT(*)
                FROM
                    "IncidentReports" IR
                        INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                        INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                        LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                LEFT JOIN LATERAL "GetRecordReviewWorkflow"(electionRoundId, ngoId, 'IncidentReport', IR."Id") WF ON TRUE
                LEFT JOIN LATERAL "GetRecordSeverity"(electionRoundId, ngoId, 'IncidentReport', IR."Id") SV ON TRUE
                WHERE
                    MN."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                  AND (monitoringObserverId IS NULL OR MO."Id" = monitoringObserverId)
                  AND (searchText IS NULL
                        OR searchText = ''
                        OR U."DisplayName" ILIKE searchText
                        OR U."Email" ILIKE searchText
                        OR U."PhoneNumber" ILIKE searchText
                        OR MO."Id"::TEXT ILIKE searchText
                    )
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                  AND (pollingStationNumber IS NULL OR PS."Number" = pollingStationNumber)
                  AND (pollingStationId IS NULL OR IR."PollingStationId" = pollingStationId)
                  AND (hasFlaggedAnswers IS NULL
                        OR (IR."NumberOfFlaggedAnswers" = 0 AND hasFlaggedAnswers = FALSE)
                        OR ("NumberOfFlaggedAnswers" > 0 AND hasFlaggedAnswers = TRUE)
                    )
                  AND (followUpStatus IS NULL OR IR."FollowUpStatus" = followUpStatus)
                  AND (tagsFilter IS NULL OR CARDINALITY(tagsFilter) = 0 OR MO."Tags" && tagsFilter)
                  AND (monitoringObserverStatus IS NULL OR MO."Status" = monitoringObserverStatus)
                  AND (formId IS NULL OR IR."FormId" = formId)
                  AND (formStatus IS NULL OR F."Status" = formStatus)
                  AND (questionsAnswered IS NULL
                        OR (questionsAnswered = 'All' AND F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered")
                        OR (questionsAnswered = 'Some' AND F."NumberOfQuestions" <> IR."NumberOfQuestionsAnswered")
                        OR (questionsAnswered = 'None' AND IR."NumberOfQuestionsAnswered" = 0)
                    )
                  AND (locationType IS NULL OR IR."LocationType" = locationType)
                  AND (hasAttachments IS NULL
                        OR (hasAttachments = FALSE AND
                        (
                            SELECT
                                COUNT(1)
                            FROM
                                "IncidentReportAttachments" A
                            WHERE
                                A."IncidentReportId" = IR."Id"
                              AND IR."PollingStationId" = "PollingStationId"
                              AND A."IsDeleted" = FALSE
                              AND A."IsCompleted" = TRUE
                        ) = 0)
                        OR (
                        (hasAttachments = TRUE AND(
                            SELECT
                                COUNT(1)
                            FROM
                                "IncidentReportAttachments" A
                            WHERE
                                A."IncidentReportId" = IR."Id"
                              AND A."IsDeleted" = FALSE
                              AND A."IsCompleted" = TRUE
                        ) > 0)
                    )
                  AND (hasNotes IS NULL
                        OR (hasNotes = FALSE AND (SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") = 0)
                        OR (hasNotes = TRUE AND (SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") > 0 )
                    ))
                  AND (hasComments IS NULL
                        OR (hasComments = FALSE AND (SELECT COUNT(1) FROM "IncidentReportComments" ICC INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = ICC."CreatedBy" AND NA."NgoId" = ngoId WHERE ICC."IncidentReportId" = IR."Id") = 0)
                        OR (hasComments = TRUE AND (SELECT COUNT(1) FROM "IncidentReportComments" ICC INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = ICC."CreatedBy" AND NA."NgoId" = ngoId WHERE ICC."IncidentReportId" = IR."Id") > 0)
                    )
                  AND (fromDate is NULL OR IR."LastUpdatedAt" >= fromDate::timestamp)
                  AND (stageId IS NULL OR WF."StageId" = stageId)
                  AND (reviewStatus IS NULL OR WF."ReviewStatus" = reviewStatus)
                  AND (reviewerId IS NULL OR reviewerId = ANY(WF."ReviewerIds"))
                  AND (hasReviewer IS NULL OR (cardinality(WF."ReviewerIds") > 0) = hasReviewer)
                  AND (severities IS NULL OR cardinality(severities) = 0 OR SV."Severity" = ANY(severities))
                  AND (toDate is NULL OR IR."LastUpdatedAt" <= toDate::timestamp)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportListEntries"(uuid, uuid, uuid, text, text, text, text, text, text, text, uuid, boolean, text, text[], text, uuid, text, text, text, boolean, boolean, boolean, text, text, uuid, text, uuid, boolean, text[], text, integer, integer, text[]);""");

            migrationBuilder.Sql(
                """
                -- One page of the NGO's incident reports for the incident reports list (by entry), with each report's review
                -- workflow and severity, filtered and ordered by sortExpression ('<Column> ASC|DESC').
                CREATE OR REPLACE FUNCTION "GetIncidentReportListEntries"(
                    electionRoundId UUID,
                    ngoId UUID,
                    monitoringObserverId UUID,
                    searchText TEXT,
                    level1 TEXT,
                    level2 TEXT,
                    level3 TEXT,
                    level4 TEXT,
                    level5 TEXT,
                    pollingStationNumber TEXT,
                    pollingStationId UUID,
                    hasFlaggedAnswers BOOLEAN,
                    followUpStatus TEXT,
                    tagsFilter TEXT[],
                    monitoringObserverStatus TEXT,
                    formId UUID,
                    formStatus TEXT,
                    questionsAnswered TEXT,
                    locationType TEXT,
                    hasAttachments BOOLEAN,
                    hasNotes BOOLEAN,
                    hasComments BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    stageId UUID,
                    reviewStatus TEXT,
                    reviewerId UUID,
                    hasReviewer BOOLEAN,
                    severities TEXT[],
                    sortExpression TEXT,
                    offsetRows INTEGER,
                    pageSize INTEGER
                )
                    RETURNS TABLE
                            (
                                "IncidentReportId"          UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormCode"                  TEXT,
                                "FormStatus"                TEXT,
                                "FormName"                  JSONB,
                                "FormDefaultLanguage"       TEXT,
                                "LocationType"              TEXT,
                                "LocationDescription"       TEXT,
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "PollingStationNumber"      TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "Email"                     TEXT,
                                "PhoneNumber"               TEXT,
                                "Status"                    TEXT,
                                "Tags"                      TEXT[],
                                "NumberOfQuestionsAnswered" INTEGER,
                                "NumberOfFlaggedAnswers"    INTEGER,
                                "MediaFilesCount"           BIGINT,
                                "NotesCount"                BIGINT,
                                "CommentsCount"             BIGINT,
                                "FollowUpStatus"            TEXT,
                                "IsCompleted"               BOOLEAN,
                                "StageId"                   UUID,
                                "StageName"                 TEXT,
                                "ReviewStatus"              TEXT,
                                "StageChangedAt"            TIMESTAMP WITH TIME ZONE,
                                "ReviewedAt"                TIMESTAMP WITH TIME ZONE,
                                "ReviewerIds"               UUID[],
                                "Severity"                  TEXT
                            )
                AS
                $$
                WITH
                    INCIDENT_REPORTS AS (
                        SELECT
                            IR."Id" AS "IncidentReportId",
                            F."Code" AS "FormCode",
                            F."Status" AS "FormStatus",
                            F."Name" AS "FormName",
                            F."DefaultLanguage" "FormDefaultLanguage",
                            IR."LocationType",
                            IR."LocationDescription",
                            IR."PollingStationId",
                            IR."MonitoringObserverId",
                            IR."NumberOfQuestionsAnswered",
                            IR."NumberOfFlaggedAnswers",
                            (
                                SELECT
                                    COUNT(1)
                                FROM
                                    "IncidentReportAttachments" A
                                WHERE
                                    A."IncidentReportId" = IR."Id"
                                  AND A."IsDeleted" = FALSE
                                  AND A."IsCompleted" = TRUE
                            ) AS "MediaFilesCount",
                            ( SELECT COUNT(1) FROM "IncidentReportNotes" N WHERE N."IncidentReportId" = IR."Id") AS "NotesCount",
                            ( SELECT COUNT(1)
                              FROM "IncidentReportComments" ICC
                                  INNER JOIN "NgoAdmins" NA ON NA."ApplicationUserId" = ICC."CreatedBy"
                                      AND NA."NgoId" = ngoId
                              WHERE ICC."IncidentReportId" = IR."Id") AS "CommentsCount",
                            IR."LastUpdatedAt" AS "TimeSubmitted",
                            IR."FollowUpStatus",
                            IR."IsCompleted",
                            WF."StageId", WF."StageName", WF."ReviewStatus", WF."StageChangedAt", WF."ReviewedAt", WF."ReviewerIds", SV."Severity"
                        FROM
                            "IncidentReports" IR
                                INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                                INNER JOIN "MonitoringObservers" MO ON IR."MonitoringObserverId" = MO."Id"
                                INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        LEFT JOIN LATERAL "GetRecordReviewWorkflow"(electionRoundId, ngoId, 'IncidentReport', IR."Id") WF ON TRUE
                        LEFT JOIN LATERAL "GetRecordSeverity"(electionRoundId, ngoId, 'IncidentReport', IR."Id") SV ON TRUE
                        WHERE
                          MN."ElectionRoundId" = electionRoundId
                          AND MN."NgoId" = ngoId
                          AND (monitoringObserverId IS NULL OR MO."Id" = monitoringObserverId)
                          AND (monitoringObserverStatus IS NULL OR MO."Status" = monitoringObserverStatus)
                          AND (formId IS NULL OR IR."FormId" = formId)
                          AND (formStatus IS NULL OR F."Status" = formStatus)
                          AND (locationType IS NULL OR IR."LocationType" = locationType)
                          AND (questionsAnswered IS NULL
                              OR (questionsAnswered = 'All' AND F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered")
                              OR (questionsAnswered = 'Some' AND F."NumberOfQuestions" <> IR."NumberOfQuestionsAnswered")
                              OR (questionsAnswered = 'None' AND IR."NumberOfQuestionsAnswered" = 0)
                          )
                          AND (fromDate is NULL OR IR."LastUpdatedAt" >= fromDate::timestamp)
                          AND (stageId IS NULL OR WF."StageId" = stageId)
                          AND (reviewStatus IS NULL OR WF."ReviewStatus" = reviewStatus)
                          AND (reviewerId IS NULL OR reviewerId = ANY(WF."ReviewerIds"))
                          AND (hasReviewer IS NULL OR (cardinality(WF."ReviewerIds") > 0) = hasReviewer)
                          AND (severities IS NULL OR cardinality(severities) = 0 OR SV."Severity" = ANY(severities))
                          AND (toDate is NULL OR IR."LastUpdatedAt" <= toDate::timestamp)
                    )
                SELECT
                    IR."IncidentReportId",
                    IR."TimeSubmitted",
                    IR."FormCode"::text,
                    IR."FormStatus",
                    IR."FormName",
                    IR."FormDefaultLanguage"::text,
                    IR."LocationType",
                    IR."LocationDescription"::text,
                    PS."Id" AS "PollingStationId",
                    PS."Level1"::text,
                    PS."Level2"::text,
                    PS."Level3"::text,
                    PS."Level4"::text,
                    PS."Level5"::text,
                    PS."Number"::text "PollingStationNumber",
                    IR."MonitoringObserverId",
                    U."DisplayName" AS "ObserverName",
                    U."Email"::text,
                    U."PhoneNumber",
                    MO."Status",
                    MO."Tags",
                    IR."NumberOfQuestionsAnswered",
                    IR."NumberOfFlaggedAnswers",
                    IR."MediaFilesCount",
                    IR."NotesCount",
                    IR."CommentsCount",
                    IR."FollowUpStatus",
                    IR."IsCompleted",
                    IR."StageId", IR."StageName", IR."ReviewStatus", IR."StageChangedAt", IR."ReviewedAt", IR."ReviewerIds", IR."Severity"
                FROM
                    INCIDENT_REPORTS IR
                        INNER JOIN "MonitoringObservers" MO ON MO."Id" = IR."MonitoringObserverId"
                        INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                        INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                        LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                        LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
                WHERE
                    MN."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId
                  AND (monitoringObserverId IS NULL OR MO."Id" = monitoringObserverId)
                  AND (
                    searchText IS NULL
                        OR searchText = ''
                        OR U."DisplayName" ILIKE searchText
                        OR U."Email" ILIKE searchText
                        OR U."PhoneNumber" ILIKE searchText
                        OR MO."Id"::TEXT ILIKE searchText
                    )
                  AND (level1 IS NULL OR PS."Level1" = level1)
                  AND (level2 IS NULL OR PS."Level2" = level2)
                  AND (level3 IS NULL OR PS."Level3" = level3)
                  AND (level4 IS NULL OR PS."Level4" = level4)
                  AND (level5 IS NULL OR PS."Level5" = level5)
                  AND (pollingStationNumber IS NULL OR PS."Number" = pollingStationNumber)
                  AND (pollingStationId IS NULL OR IR."PollingStationId" = pollingStationId)
                  AND (hasFlaggedAnswers IS NULL
                    OR (IR."NumberOfFlaggedAnswers" = 0 AND hasFlaggedAnswers = FALSE)
                    OR (IR."NumberOfFlaggedAnswers" > 0 AND hasFlaggedAnswers = TRUE)
                    )
                  AND (followUpStatus IS NULL OR IR."FollowUpStatus" = followUpStatus)
                  AND (tagsFilter IS NULL OR CARDINALITY(tagsFilter) = 0 OR MO."Tags" && tagsFilter)
                  AND (hasNotes IS NULL
                    OR (IR."NotesCount" = 0 AND hasNotes = FALSE)
                    OR (IR."NotesCount" > 0 AND hasNotes = TRUE)
                    )
                  AND (hasAttachments IS NULL
                    OR (IR."MediaFilesCount" = 0 AND hasAttachments = FALSE)
                    OR (IR."MediaFilesCount" > 0 AND hasAttachments = TRUE)
                    )
                  AND (hasComments IS NULL
                    OR (IR."CommentsCount" = 0 AND hasComments = FALSE)
                    OR (IR."CommentsCount" > 0 AND hasComments = TRUE)
                    )
                ORDER BY
                    CASE WHEN sortExpression = 'Severity ASC' THEN "SeverityRank"(IR."Severity") END ASC,
                    CASE WHEN sortExpression = 'Severity DESC' THEN "SeverityRank"(IR."Severity") END DESC,
                    CASE WHEN sortExpression LIKE 'Severity %' THEN IR."TimeSubmitted" END DESC,
                    CASE WHEN sortExpression = 'TimeSubmitted ASC' THEN IR."TimeSubmitted" END ASC,
                    CASE WHEN sortExpression = 'TimeSubmitted DESC' THEN IR."TimeSubmitted" END DESC,
                    CASE WHEN sortExpression = 'FormCode ASC' THEN IR."FormCode" END ASC,
                    CASE WHEN sortExpression = 'FormCode DESC' THEN IR."FormCode" END DESC,
                    CASE WHEN sortExpression = 'NumberOfQuestionsAnswered ASC' THEN IR."NumberOfQuestionsAnswered" END ASC,
                    CASE WHEN sortExpression = 'NumberOfQuestionsAnswered DESC' THEN IR."NumberOfQuestionsAnswered" END DESC,
                    CASE WHEN sortExpression = 'NumberOfFlaggedAnswers ASC' THEN IR."NumberOfFlaggedAnswers" END ASC,
                    CASE WHEN sortExpression = 'NumberOfFlaggedAnswers DESC' THEN IR."NumberOfFlaggedAnswers" END DESC,
                    CASE WHEN sortExpression = 'MediaFilesCount ASC' THEN IR."MediaFilesCount" END ASC,
                    CASE WHEN sortExpression = 'MediaFilesCount DESC' THEN IR."MediaFilesCount" END DESC,
                    CASE WHEN sortExpression = 'NotesCount ASC' THEN IR."NotesCount" END ASC,
                    CASE WHEN sortExpression = 'NotesCount DESC' THEN IR."NotesCount" END DESC,
                    CASE WHEN sortExpression = 'Level1 ASC' THEN PS."Level1" END ASC,
                    CASE WHEN sortExpression = 'Level1 DESC' THEN PS."Level1" END DESC,
                    CASE WHEN sortExpression = 'Level2 ASC' THEN PS."Level2" END ASC,
                    CASE WHEN sortExpression = 'Level2 DESC' THEN PS."Level2" END DESC,
                    CASE WHEN sortExpression = 'Level3 ASC' THEN PS."Level3" END ASC,
                    CASE WHEN sortExpression = 'Level3 DESC' THEN PS."Level3" END DESC,
                    CASE WHEN sortExpression = 'Level4 ASC' THEN PS."Level4" END ASC,
                    CASE WHEN sortExpression = 'Level4 DESC' THEN PS."Level4" END DESC,
                    CASE WHEN sortExpression = 'Level5 ASC' THEN PS."Level5" END ASC,
                    CASE WHEN sortExpression = 'Level5 DESC' THEN PS."Level5" END DESC,
                    CASE WHEN sortExpression = 'PollingStationNumber ASC' THEN PS."Number" END ASC,
                    CASE WHEN sortExpression = 'PollingStationNumber DESC' THEN PS."Number" END DESC,
                    CASE WHEN sortExpression = 'ObserverName ASC' THEN U."DisplayName" END ASC,
                    CASE WHEN sortExpression = 'ObserverName DESC' THEN U."DisplayName" END DESC
                OFFSET offsetRows ROWS
                FETCH NEXT pageSize ROWS ONLY
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetRecordLabels"(uuid, uuid, text, uuid);""");

            migrationBuilder.DropTable(
                name: "RecordLabels");

            migrationBuilder.DropColumn(
                name: "AddedLabels",
                table: "ReviewActivities");

            migrationBuilder.DropColumn(
                name: "RemovedLabels",
                table: "ReviewActivities");
        }
    }
}
