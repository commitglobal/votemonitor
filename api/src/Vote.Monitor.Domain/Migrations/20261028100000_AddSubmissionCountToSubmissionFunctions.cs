using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionCountToSubmissionFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Every view of a form submission shows "2 of 3" when the observer sent that form more than once from the same
            // polling station: the platform lists get the count beside their number, a submission's page both.
            // New result columns need each function dropped first.
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCorrectableRecords"(UUID, TEXT[], UUID, UUID, TEXT, TEXT, BOOLEAN, TEXT[], TEXT[], TEXT[], TEXT[], TEXT[]);""");

            migrationBuilder.Sql(
                """
                -- The records platform admins can move to another polling station: every form submission and incident report at a
                -- polling station and every quick report from a visited polling station (never citizen reports), with their observer,
                -- NGO, polling station and correction history (jsonb array, oldest first). Null arguments don't filter; recordTypes and
                -- level1..level5 keep records matching any of their values (null or empty = no filter); searchText and
                -- pollingStationNumber are ILIKE patterns; correctedOnly keeps only records that were (true) or weren't (false) corrected.
                -- SubmissionCount is how many such submissions there are. SubmissionNumber (form submissions only, null for quick and incident reports) is which of the observer's submissions of
                -- that form at that polling station the record is, by creation (ties by Id; always 1 for PSI forms). It counts all of the
                -- observer's submissions, so filters and pages never change it.
                CREATE FUNCTION "GetCorrectableRecords"(
                    electionRoundId UUID,
                    recordTypes TEXT[],
                    recordId UUID,
                    monitoringNgoId UUID,
                    searchText TEXT,
                    pollingStationNumber TEXT,
                    correctedOnly BOOLEAN,
                    level1 TEXT[],
                    level2 TEXT[],
                    level3 TEXT[],
                    level4 TEXT[],
                    level5 TEXT[]
                )
                    RETURNS TABLE
                            (
                                "Id"               UUID,
                                "RecordType"       TEXT,
                                "ObserverName"     TEXT,
                                "ObserverEmail"    TEXT,
                                "NgoName"          TEXT,
                                "FormCode"         TEXT,
                                "FormName"         TEXT,
                                "SubmittedAt"      TIMESTAMP WITH TIME ZONE,
                                "SubmissionNumber" INTEGER,
                                "SubmissionCount"  INTEGER,
                                "PollingStationId" UUID,
                                "Number"           TEXT,
                                "Level1"           TEXT,
                                "Level2"           TEXT,
                                "Level3"           TEXT,
                                "Level4"           TEXT,
                                "Level5"           TEXT,
                                "Address"          TEXT,
                                "Corrections"      JSONB
                            )
                AS
                $$
                SELECT R."Id",
                       R."RecordType",
                       U."DisplayName",
                       U."Email"::text,
                       N."Name"::text,
                       R."FormCode"::text,
                       R."FormName",
                       R."SubmittedAt",
                       R."SubmissionNumber",
                       R."SubmissionCount",
                       PS."Id",
                       PS."Number"::text,
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Address"::text,
                       COALESCE((SELECT jsonb_agg(jsonb_build_object(
                                     'CorrectedAt', to_char(PSC."CorrectedAt" AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS.US"Z"'),
                                     'From', jsonb_build_object('Id', FPS."Id", 'Number', FPS."Number", 'Level1', FPS."Level1", 'Level2', FPS."Level2", 'Level3', FPS."Level3", 'Level4', FPS."Level4", 'Level5', FPS."Level5", 'Address', FPS."Address"),
                                     'To', jsonb_build_object('Id', TPS."Id", 'Number', TPS."Number", 'Level1', TPS."Level1", 'Level2', TPS."Level2", 'Level3', TPS."Level3", 'Level4', TPS."Level4", 'Level5', TPS."Level5", 'Address', TPS."Address")
                                   ) ORDER BY PSC."CorrectedAt")
                                 FROM "PollingStationCorrections" PSC
                                     INNER JOIN "PollingStations" FPS ON FPS."Id" = PSC."FromPollingStationId"
                                     INNER JOIN "PollingStations" TPS ON TPS."Id" = PSC."ToPollingStationId"
                                 WHERE PSC."RecordType" = R."RecordType" AND PSC."RecordId" = R."Id"), '[]'::jsonb)
                FROM (SELECT 'FormSubmission' AS "RecordType",
                             FS."Id",
                             FS."MonitoringObserverId",
                             FS."PollingStationId",
                             F."Code" AS "FormCode",
                             F."Name" ->> F."DefaultLanguage" AS "FormName",
                             FS."LastUpdatedAt" AS "SubmittedAt",
                             CASE
                                 WHEN F."FormType" = 'PSI' THEN 1
                                 ELSE (SELECT COUNT(*)::int
                                       FROM "FormSubmissions" S
                                       WHERE S."ElectionRoundId" = FS."ElectionRoundId"
                                         AND S."FormId" = FS."FormId"
                                         AND S."PollingStationId" = FS."PollingStationId"
                                         AND S."MonitoringObserverId" = FS."MonitoringObserverId"
                                         AND (S."CreatedAt", S."Id") <= (FS."CreatedAt", FS."Id"))
                             END AS "SubmissionNumber",
                             CASE
                                 WHEN F."FormType" = 'PSI' THEN 1
                                 ELSE (SELECT COUNT(*)::int
                                       FROM "FormSubmissions" S
                                       WHERE S."ElectionRoundId" = FS."ElectionRoundId"
                                         AND S."FormId" = FS."FormId"
                                         AND S."PollingStationId" = FS."PollingStationId"
                                         AND S."MonitoringObserverId" = FS."MonitoringObserverId")
                             END AS "SubmissionCount"
                      FROM "FormSubmissions" FS
                          INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                      WHERE FS."ElectionRoundId" = electionRoundId
                        AND (recordTypes IS NULL OR CARDINALITY(recordTypes) = 0 OR 'FormSubmission' = ANY(recordTypes))
                        AND (recordId IS NULL OR FS."Id" = recordId)
                      UNION ALL
                      SELECT 'QuickReport' AS "RecordType",
                             QR."Id",
                             QR."MonitoringObserverId",
                             QR."PollingStationId",
                             NULL AS "FormCode",
                             NULL AS "FormName",
                             QR."LastUpdatedAt" AS "SubmittedAt",
                             NULL::int AS "SubmissionNumber",
                             NULL::int AS "SubmissionCount"
                      FROM "QuickReports" QR
                      WHERE QR."ElectionRoundId" = electionRoundId
                        AND QR."QuickReportLocationType" = 'VisitedPollingStation'
                        AND QR."PollingStationId" IS NOT NULL
                        AND (recordTypes IS NULL OR CARDINALITY(recordTypes) = 0 OR 'QuickReport' = ANY(recordTypes))
                        AND (recordId IS NULL OR QR."Id" = recordId)
                      UNION ALL
                      SELECT 'IncidentReport' AS "RecordType",
                             IR."Id",
                             IR."MonitoringObserverId",
                             IR."PollingStationId",
                             F."Code" AS "FormCode",
                             F."Name" ->> F."DefaultLanguage" AS "FormName",
                             IR."LastUpdatedAt" AS "SubmittedAt",
                             NULL::int AS "SubmissionNumber",
                             NULL::int AS "SubmissionCount"
                      FROM "IncidentReports" IR
                          INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                      WHERE IR."ElectionRoundId" = electionRoundId
                        AND IR."PollingStationId" IS NOT NULL
                        AND (recordTypes IS NULL OR CARDINALITY(recordTypes) = 0 OR 'IncidentReport' = ANY(recordTypes))
                        AND (recordId IS NULL OR IR."Id" = recordId)) R
                    INNER JOIN "PollingStations" PS ON PS."Id" = R."PollingStationId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = R."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE (monitoringNgoId IS NULL OR MN."Id" = monitoringNgoId)
                  AND (searchText IS NULL OR U."DisplayName" ILIKE searchText OR N."Name" ILIKE searchText)
                  AND (pollingStationNumber IS NULL OR PS."Number" ILIKE pollingStationNumber)
                  AND (correctedOnly IS NULL OR correctedOnly = EXISTS (SELECT 1
                                                                        FROM "PollingStationCorrections" PSC
                                                                        WHERE PSC."ElectionRoundId" = electionRoundId
                                                                          AND PSC."RecordType" = R."RecordType"
                                                                          AND PSC."RecordId" = R."Id"))
                  AND (level1 IS NULL OR CARDINALITY(level1) = 0 OR PS."Level1" = ANY(level1))
                  AND (level2 IS NULL OR CARDINALITY(level2) = 0 OR PS."Level2" = ANY(level2))
                  AND (level3 IS NULL OR CARDINALITY(level3) = 0 OR PS."Level3" = ANY(level3))
                  AND (level4 IS NULL OR CARDINALITY(level4) = 0 OR PS."Level4" = ANY(level4))
                  AND (level5 IS NULL OR CARDINALITY(level5) = 0 OR PS."Level5" = ANY(level5))
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetElectionRoundFormSubmissionRows"(UUID, TEXT[], TEXT[], TEXT[], TEXT[], TEXT[], UUID, UUID, BOOLEAN, TEXT, TEXT, TEXT);""");

            migrationBuilder.Sql(
                """
                -- Every form submission in a round (any NGO) with its form, polling station, NGO and observer, matching the filters
                -- (platform admin listing). Null filters match everything; level1..level5 keep submissions at any of their values (null or
                -- empty = no filter); fromDate/toDate are ISO 8601 strings compared as timestamps. SubmissionNumber is which of the
                -- observer's submissions of that form at that polling station it is, by creation (ties by Id; always 1 for PSI forms). It
                -- counts all of the observer's submissions, so filters and pages never change it.
                CREATE FUNCTION "GetElectionRoundFormSubmissionRows"(
                    electionRoundId UUID,
                    level1 TEXT[],
                    level2 TEXT[],
                    level3 TEXT[],
                    level4 TEXT[],
                    level5 TEXT[],
                    monitoringNgoId UUID,
                    formId UUID,
                    isCompleted BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    searchText TEXT
                )
                    RETURNS TABLE
                            (
                                "SubmissionId"              UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormId"                    UUID,
                                "FormCode"                  TEXT,
                                "FormType"                  TEXT,
                                "FormName"                  JSONB,
                                "DefaultLanguage"           TEXT,
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "Number"                    TEXT,
                                "Address"                   TEXT,
                                "MonitoringNgoId"           UUID,
                                "NgoName"                   TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "ObserverEmail"             TEXT,
                                "ObserverPhoneNumber"       TEXT,
                                "NumberOfQuestionsAnswered" INTEGER,
                                "NumberOfFlaggedAnswers"    INTEGER,
                                "FollowUpStatus"            TEXT,
                                "IsCompleted"               BOOLEAN,
                                "SubmissionNumber"          INTEGER,
                                "SubmissionCount"           INTEGER
                            )
                AS
                $$
                SELECT FS."Id" AS "SubmissionId",
                       FS."LastUpdatedAt" AS "TimeSubmitted",
                       F."Id" AS "FormId",
                       F."Code"::text AS "FormCode",
                       F."FormType",
                       F."Name" AS "FormName",
                       F."DefaultLanguage"::text AS "DefaultLanguage",
                       PS."Id" AS "PollingStationId",
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Number"::text,
                       PS."Address"::text,
                       MN."Id" AS "MonitoringNgoId",
                       N."Name"::text AS "NgoName",
                       MO."Id" AS "MonitoringObserverId",
                       U."DisplayName" AS "ObserverName",
                       U."Email"::text AS "ObserverEmail",
                       U."PhoneNumber" AS "ObserverPhoneNumber",
                       FS."NumberOfQuestionsAnswered",
                       FS."NumberOfFlaggedAnswers",
                       FS."FollowUpStatus",
                       FS."IsCompleted",
                       CASE
                           WHEN F."FormType" = 'PSI' THEN 1
                           ELSE (SELECT COUNT(*)::int
                                 FROM "FormSubmissions" S
                                 WHERE S."ElectionRoundId" = FS."ElectionRoundId"
                                   AND S."FormId" = FS."FormId"
                                   AND S."PollingStationId" = FS."PollingStationId"
                                   AND S."MonitoringObserverId" = FS."MonitoringObserverId"
                                   AND (S."CreatedAt", S."Id") <= (FS."CreatedAt", FS."Id"))
                       END AS "SubmissionNumber",
                       CASE
                           WHEN F."FormType" = 'PSI' THEN 1
                           ELSE (SELECT COUNT(*)::int
                                 FROM "FormSubmissions" S
                                 WHERE S."ElectionRoundId" = FS."ElectionRoundId"
                                   AND S."FormId" = FS."FormId"
                                   AND S."PollingStationId" = FS."PollingStationId"
                                   AND S."MonitoringObserverId" = FS."MonitoringObserverId")
                       END AS "SubmissionCount"
                FROM "FormSubmissions" FS
                    INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    INNER JOIN "PollingStations" PS ON PS."Id" = FS."PollingStationId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE FS."ElectionRoundId" = electionRoundId
                  AND (level1 IS NULL OR CARDINALITY(level1) = 0 OR PS."Level1" = ANY(level1))
                  AND (level2 IS NULL OR CARDINALITY(level2) = 0 OR PS."Level2" = ANY(level2))
                  AND (level3 IS NULL OR CARDINALITY(level3) = 0 OR PS."Level3" = ANY(level3))
                  AND (level4 IS NULL OR CARDINALITY(level4) = 0 OR PS."Level4" = ANY(level4))
                  AND (level5 IS NULL OR CARDINALITY(level5) = 0 OR PS."Level5" = ANY(level5))
                  AND (monitoringNgoId IS NULL OR MN."Id" = monitoringNgoId)
                  AND (formId IS NULL OR FS."FormId" = formId)
                  AND (isCompleted IS NULL OR FS."IsCompleted" = isCompleted)
                  AND (fromDate IS NULL OR FS."LastUpdatedAt" >= fromDate::timestamp)
                  AND (toDate IS NULL OR FS."LastUpdatedAt" <= toDate::timestamp)
                  AND (searchText IS NULL OR PS."Number" ILIKE searchText OR PS."Address" ILIKE searchText OR F."Code" ILIKE searchText)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionViewV2"(UUID, UUID, UUID);""");

            migrationBuilder.Sql(
                """
                -- A form submission or polling station information visible to the NGO (Coalition data source) in a round, with its
                -- form, polling station, observer, answers, notes, attachments and polling station corrections (oldest first). No row
                -- when not found.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionViewV2"(
                    electionRoundId UUID,
                    ngoId UUID,
                    submissionId UUID
                )
                    RETURNS TABLE
                            (
                                "SubmissionId"              UUID,
                                "FormId"                    UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormCode"                  TEXT,
                                "FormName"                  JSONB,
                                "FormType"                  TEXT,
                                "DefaultLanguage"           TEXT,
                                "Languages"                 TEXT[],
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "Number"                    TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "Email"                     TEXT,
                                "PhoneNumber"               TEXT,
                                "Tags"                      TEXT[],
                                "NgoName"                   TEXT,
                                "IsOwnObserver"             BOOLEAN,
                                "Attachments"               JSONB,
                                "Notes"                     JSONB,
                                "Answers"                   JSONB,
                                "Questions"                 JSONB,
                                "FollowUpStatus"            TEXT,
                                "ArrivalTime"               TIMESTAMP WITH TIME ZONE,
                                "DepartureTime"             TIMESTAMP WITH TIME ZONE,
                                "Breaks"                    JSONB,
                                "IsCompleted"               BOOLEAN,
                                "PollingStationCorrections" JSONB,
                                -- Which of the observer's submissions of this form at this polling station this is (by creation
                                -- time, then id) and how many there are; polling station information is always 1 of 1.
                                "SubmissionNumber"          INTEGER,
                                "SubmissionCount"           INTEGER
                            )
                AS
                $$
                WITH submissions AS
                         (SELECT psi."Id" AS "SubmissionId",
                                 'PSI' AS "FormType",
                                 'PSI' AS "FormCode",
                                 psif."Name" AS "FormName",
                                 psi."PollingStationId",
                                 psi."MonitoringObserverId",
                                 psi."Answers",
                                 psif."Questions" AS "Questions",
                                 psif."DefaultLanguage" AS "DefaultLanguage",
                                 psif."Languages" AS "Languages",
                                 psif."Id" AS "FormId",
                                 psi."FollowUpStatus" AS "FollowUpStatus",
                                 '[]'::jsonb AS "Attachments",
                                 '[]'::jsonb AS "Notes",
                                 "LastUpdatedAt" AS "TimeSubmitted",
                                 psi."ArrivalTime",
                                 psi."DepartureTime",
                                 psi."Breaks",
                                 psi."IsCompleted",
                                 '[]'::jsonb AS "PollingStationCorrections"
                          FROM "PollingStationInformation" psi
                              INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = psi."MonitoringObserverId"
                              INNER JOIN "PollingStationInformationForms" psif ON psi."ElectionRoundId" = psif."ElectionRoundId"
                          WHERE psi."Id" = submissionId AND psi."ElectionRoundId" = electionRoundId
                          UNION ALL
                          SELECT fs."Id" AS "SubmissionId",
                                 f."FormType" AS "FormType",
                                 f."Code" AS "FormCode",
                                 f."Name" AS "FormName",
                                 fs."PollingStationId",
                                 fs."MonitoringObserverId",
                                 fs."Answers",
                                 f."Questions",
                                 f."DefaultLanguage",
                                 f."Languages",
                                 f."Id" AS "FormId",
                                 fs."FollowUpStatus",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'FileName', "FileName", 'MimeType', "MimeType", 'FilePath', "FilePath", 'UploadedFileName', "UploadedFileName", 'TimeSubmitted', "LastUpdatedAt"))
                                           FROM "Attachments" a
                                           WHERE ((a."FormId" = fs."FormId" AND fs."PollingStationId" = a."PollingStationId") -- backwards compatibility
                                                  OR a."SubmissionId" = fs."Id")
                                             AND a."MonitoringObserverId" = fs."MonitoringObserverId"
                                             AND a."IsDeleted" = false
                                             AND a."IsCompleted" = true), '[]'::JSONB) AS "Attachments",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'Text', "Text", 'TimeSubmitted', "LastUpdatedAt"))
                                           FROM "Notes" n
                                           WHERE ((n."FormId" = fs."FormId" AND fs."PollingStationId" = n."PollingStationId") -- backwards compatibility
                                                  OR n."SubmissionId" = fs."Id")
                                             AND n."MonitoringObserverId" = fs."MonitoringObserverId"), '[]'::JSONB) AS "Notes",
                                 "LastUpdatedAt" AS "TimeSubmitted",
                                 NULL AS "ArrivalTime",
                                 NULL AS "DepartureTime",
                                 '[]'::jsonb AS "Breaks",
                                 fs."IsCompleted",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object(
                                                    'CorrectedAt', to_char(PSC."CorrectedAt" AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS.US"Z"'),
                                                    'From', jsonb_build_object('Id', FPS."Id", 'Number', FPS."Number", 'Level1', FPS."Level1", 'Level2', FPS."Level2", 'Level3', FPS."Level3", 'Level4', FPS."Level4", 'Level5', FPS."Level5", 'Address', FPS."Address"),
                                                    'To', jsonb_build_object('Id', TPS."Id", 'Number', TPS."Number", 'Level1', TPS."Level1", 'Level2', TPS."Level2", 'Level3', TPS."Level3", 'Level4', TPS."Level4", 'Level5', TPS."Level5", 'Address', TPS."Address")
                                                  ) ORDER BY PSC."CorrectedAt")
                                           FROM "PollingStationCorrections" PSC
                                               INNER JOIN "PollingStations" FPS ON FPS."Id" = PSC."FromPollingStationId"
                                               INNER JOIN "PollingStations" TPS ON TPS."Id" = PSC."ToPollingStationId"
                                           WHERE PSC."RecordType" = 'FormSubmission' AND PSC."RecordId" = fs."Id"), '[]'::jsonb) AS "PollingStationCorrections"
                          FROM "FormSubmissions" fs
                              INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = fs."MonitoringObserverId"
                              INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                          WHERE fs."Id" = submissionId AND fs."ElectionRoundId" = electionRoundId)
                SELECT s."SubmissionId",
                       s."FormId",
                       s."TimeSubmitted",
                       s."FormCode"::text,
                       s."FormName",
                       s."FormType",
                       s."DefaultLanguage"::text,
                       s."Languages",
                       ps."Id" AS "PollingStationId",
                       ps."Level1"::text,
                       ps."Level2"::text,
                       ps."Level3"::text,
                       ps."Level4"::text,
                       ps."Level5"::text,
                       ps."Number"::text,
                       s."MonitoringObserverId",
                       AMO."DisplayName" AS "ObserverName",
                       AMO."Email",
                       AMO."PhoneNumber",
                       AMO."Tags",
                       AMO."NgoName"::text,
                       AMO."IsOwnObserver",
                       s."Attachments",
                       s."Notes",
                       s."Answers",
                       s."Questions",
                       s."FollowUpStatus",
                       s."ArrivalTime",
                       s."DepartureTime",
                       s."Breaks",
                       s."IsCompleted",
                       s."PollingStationCorrections",
                       CASE
                           WHEN s."FormType" = 'PSI' THEN 1
                           ELSE (SELECT COUNT(*)::int
                                 FROM "FormSubmissions" S2
                                 WHERE S2."FormId" = s."FormId"
                                   AND S2."PollingStationId" = s."PollingStationId"
                                   AND S2."MonitoringObserverId" = s."MonitoringObserverId"
                                   AND (S2."CreatedAt", S2."Id") <= (SELECT X."CreatedAt", X."Id" FROM "FormSubmissions" X WHERE X."Id" = s."SubmissionId"))
                       END AS "SubmissionNumber",
                       CASE
                           WHEN s."FormType" = 'PSI' THEN 1
                           ELSE (SELECT COUNT(*)::int
                                 FROM "FormSubmissions" S2
                                 WHERE S2."FormId" = s."FormId"
                                   AND S2."PollingStationId" = s."PollingStationId"
                                   AND S2."MonitoringObserverId" = s."MonitoringObserverId")
                       END AS "SubmissionCount"
                FROM submissions s
                    INNER JOIN "PollingStations" ps ON ps."Id" = s."PollingStationId"
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = s."MonitoringObserverId"
                $$ LANGUAGE sql STABLE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCorrectableRecords"(UUID, TEXT[], UUID, UUID, TEXT, TEXT, BOOLEAN, TEXT[], TEXT[], TEXT[], TEXT[], TEXT[]);""");

            migrationBuilder.Sql(
                """
                -- The records platform admins can move to another polling station: every form submission and incident report at a
                -- polling station and every quick report from a visited polling station (never citizen reports), with their observer,
                -- NGO, polling station and correction history (jsonb array, oldest first). Null arguments don't filter; recordTypes and
                -- level1..level5 keep records matching any of their values (null or empty = no filter); searchText and
                -- pollingStationNumber are ILIKE patterns; correctedOnly keeps only records that were (true) or weren't (false) corrected.
                -- SubmissionNumber (form submissions only, null for quick and incident reports) is which of the observer's submissions of
                -- that form at that polling station the record is, by creation (ties by Id; always 1 for PSI forms). It counts all of the
                -- observer's submissions, so filters and pages never change it.
                CREATE FUNCTION "GetCorrectableRecords"(
                    electionRoundId UUID,
                    recordTypes TEXT[],
                    recordId UUID,
                    monitoringNgoId UUID,
                    searchText TEXT,
                    pollingStationNumber TEXT,
                    correctedOnly BOOLEAN,
                    level1 TEXT[],
                    level2 TEXT[],
                    level3 TEXT[],
                    level4 TEXT[],
                    level5 TEXT[]
                )
                    RETURNS TABLE
                            (
                                "Id"               UUID,
                                "RecordType"       TEXT,
                                "ObserverName"     TEXT,
                                "ObserverEmail"    TEXT,
                                "NgoName"          TEXT,
                                "FormCode"         TEXT,
                                "FormName"         TEXT,
                                "SubmittedAt"      TIMESTAMP WITH TIME ZONE,
                                "SubmissionNumber" INTEGER,
                                "PollingStationId" UUID,
                                "Number"           TEXT,
                                "Level1"           TEXT,
                                "Level2"           TEXT,
                                "Level3"           TEXT,
                                "Level4"           TEXT,
                                "Level5"           TEXT,
                                "Address"          TEXT,
                                "Corrections"      JSONB
                            )
                AS
                $$
                SELECT R."Id",
                       R."RecordType",
                       U."DisplayName",
                       U."Email"::text,
                       N."Name"::text,
                       R."FormCode"::text,
                       R."FormName",
                       R."SubmittedAt",
                       R."SubmissionNumber",
                       PS."Id",
                       PS."Number"::text,
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Address"::text,
                       COALESCE((SELECT jsonb_agg(jsonb_build_object(
                                     'CorrectedAt', to_char(PSC."CorrectedAt" AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS.US"Z"'),
                                     'From', jsonb_build_object('Id', FPS."Id", 'Number', FPS."Number", 'Level1', FPS."Level1", 'Level2', FPS."Level2", 'Level3', FPS."Level3", 'Level4', FPS."Level4", 'Level5', FPS."Level5", 'Address', FPS."Address"),
                                     'To', jsonb_build_object('Id', TPS."Id", 'Number', TPS."Number", 'Level1', TPS."Level1", 'Level2', TPS."Level2", 'Level3', TPS."Level3", 'Level4', TPS."Level4", 'Level5', TPS."Level5", 'Address', TPS."Address")
                                   ) ORDER BY PSC."CorrectedAt")
                                 FROM "PollingStationCorrections" PSC
                                     INNER JOIN "PollingStations" FPS ON FPS."Id" = PSC."FromPollingStationId"
                                     INNER JOIN "PollingStations" TPS ON TPS."Id" = PSC."ToPollingStationId"
                                 WHERE PSC."RecordType" = R."RecordType" AND PSC."RecordId" = R."Id"), '[]'::jsonb)
                FROM (SELECT 'FormSubmission' AS "RecordType",
                             FS."Id",
                             FS."MonitoringObserverId",
                             FS."PollingStationId",
                             F."Code" AS "FormCode",
                             F."Name" ->> F."DefaultLanguage" AS "FormName",
                             FS."LastUpdatedAt" AS "SubmittedAt",
                             CASE
                                 WHEN F."FormType" = 'PSI' THEN 1
                                 ELSE (SELECT COUNT(*)::int
                                       FROM "FormSubmissions" S
                                       WHERE S."ElectionRoundId" = FS."ElectionRoundId"
                                         AND S."FormId" = FS."FormId"
                                         AND S."PollingStationId" = FS."PollingStationId"
                                         AND S."MonitoringObserverId" = FS."MonitoringObserverId"
                                         AND (S."CreatedAt", S."Id") <= (FS."CreatedAt", FS."Id"))
                             END AS "SubmissionNumber"
                      FROM "FormSubmissions" FS
                          INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                      WHERE FS."ElectionRoundId" = electionRoundId
                        AND (recordTypes IS NULL OR CARDINALITY(recordTypes) = 0 OR 'FormSubmission' = ANY(recordTypes))
                        AND (recordId IS NULL OR FS."Id" = recordId)
                      UNION ALL
                      SELECT 'QuickReport' AS "RecordType",
                             QR."Id",
                             QR."MonitoringObserverId",
                             QR."PollingStationId",
                             NULL AS "FormCode",
                             NULL AS "FormName",
                             QR."LastUpdatedAt" AS "SubmittedAt",
                             NULL::int AS "SubmissionNumber"
                      FROM "QuickReports" QR
                      WHERE QR."ElectionRoundId" = electionRoundId
                        AND QR."QuickReportLocationType" = 'VisitedPollingStation'
                        AND QR."PollingStationId" IS NOT NULL
                        AND (recordTypes IS NULL OR CARDINALITY(recordTypes) = 0 OR 'QuickReport' = ANY(recordTypes))
                        AND (recordId IS NULL OR QR."Id" = recordId)
                      UNION ALL
                      SELECT 'IncidentReport' AS "RecordType",
                             IR."Id",
                             IR."MonitoringObserverId",
                             IR."PollingStationId",
                             F."Code" AS "FormCode",
                             F."Name" ->> F."DefaultLanguage" AS "FormName",
                             IR."LastUpdatedAt" AS "SubmittedAt",
                             NULL::int AS "SubmissionNumber"
                      FROM "IncidentReports" IR
                          INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                      WHERE IR."ElectionRoundId" = electionRoundId
                        AND IR."PollingStationId" IS NOT NULL
                        AND (recordTypes IS NULL OR CARDINALITY(recordTypes) = 0 OR 'IncidentReport' = ANY(recordTypes))
                        AND (recordId IS NULL OR IR."Id" = recordId)) R
                    INNER JOIN "PollingStations" PS ON PS."Id" = R."PollingStationId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = R."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE (monitoringNgoId IS NULL OR MN."Id" = monitoringNgoId)
                  AND (searchText IS NULL OR U."DisplayName" ILIKE searchText OR N."Name" ILIKE searchText)
                  AND (pollingStationNumber IS NULL OR PS."Number" ILIKE pollingStationNumber)
                  AND (correctedOnly IS NULL OR correctedOnly = EXISTS (SELECT 1
                                                                        FROM "PollingStationCorrections" PSC
                                                                        WHERE PSC."ElectionRoundId" = electionRoundId
                                                                          AND PSC."RecordType" = R."RecordType"
                                                                          AND PSC."RecordId" = R."Id"))
                  AND (level1 IS NULL OR CARDINALITY(level1) = 0 OR PS."Level1" = ANY(level1))
                  AND (level2 IS NULL OR CARDINALITY(level2) = 0 OR PS."Level2" = ANY(level2))
                  AND (level3 IS NULL OR CARDINALITY(level3) = 0 OR PS."Level3" = ANY(level3))
                  AND (level4 IS NULL OR CARDINALITY(level4) = 0 OR PS."Level4" = ANY(level4))
                  AND (level5 IS NULL OR CARDINALITY(level5) = 0 OR PS."Level5" = ANY(level5))
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetElectionRoundFormSubmissionRows"(UUID, TEXT[], TEXT[], TEXT[], TEXT[], TEXT[], UUID, UUID, BOOLEAN, TEXT, TEXT, TEXT);""");

            migrationBuilder.Sql(
                """
                -- Every form submission in a round (any NGO) with its form, polling station, NGO and observer, matching the filters
                -- (platform admin listing). Null filters match everything; level1..level5 keep submissions at any of their values (null or
                -- empty = no filter); fromDate/toDate are ISO 8601 strings compared as timestamps. SubmissionNumber is which of the
                -- observer's submissions of that form at that polling station it is, by creation (ties by Id; always 1 for PSI forms). It
                -- counts all of the observer's submissions, so filters and pages never change it.
                CREATE FUNCTION "GetElectionRoundFormSubmissionRows"(
                    electionRoundId UUID,
                    level1 TEXT[],
                    level2 TEXT[],
                    level3 TEXT[],
                    level4 TEXT[],
                    level5 TEXT[],
                    monitoringNgoId UUID,
                    formId UUID,
                    isCompleted BOOLEAN,
                    fromDate TEXT,
                    toDate TEXT,
                    searchText TEXT
                )
                    RETURNS TABLE
                            (
                                "SubmissionId"              UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormId"                    UUID,
                                "FormCode"                  TEXT,
                                "FormType"                  TEXT,
                                "FormName"                  JSONB,
                                "DefaultLanguage"           TEXT,
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "Number"                    TEXT,
                                "Address"                   TEXT,
                                "MonitoringNgoId"           UUID,
                                "NgoName"                   TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "ObserverEmail"             TEXT,
                                "ObserverPhoneNumber"       TEXT,
                                "NumberOfQuestionsAnswered" INTEGER,
                                "NumberOfFlaggedAnswers"    INTEGER,
                                "FollowUpStatus"            TEXT,
                                "IsCompleted"               BOOLEAN,
                                "SubmissionNumber"          INTEGER
                            )
                AS
                $$
                SELECT FS."Id" AS "SubmissionId",
                       FS."LastUpdatedAt" AS "TimeSubmitted",
                       F."Id" AS "FormId",
                       F."Code"::text AS "FormCode",
                       F."FormType",
                       F."Name" AS "FormName",
                       F."DefaultLanguage"::text AS "DefaultLanguage",
                       PS."Id" AS "PollingStationId",
                       PS."Level1"::text,
                       PS."Level2"::text,
                       PS."Level3"::text,
                       PS."Level4"::text,
                       PS."Level5"::text,
                       PS."Number"::text,
                       PS."Address"::text,
                       MN."Id" AS "MonitoringNgoId",
                       N."Name"::text AS "NgoName",
                       MO."Id" AS "MonitoringObserverId",
                       U."DisplayName" AS "ObserverName",
                       U."Email"::text AS "ObserverEmail",
                       U."PhoneNumber" AS "ObserverPhoneNumber",
                       FS."NumberOfQuestionsAnswered",
                       FS."NumberOfFlaggedAnswers",
                       FS."FollowUpStatus",
                       FS."IsCompleted",
                       CASE
                           WHEN F."FormType" = 'PSI' THEN 1
                           ELSE (SELECT COUNT(*)::int
                                 FROM "FormSubmissions" S
                                 WHERE S."ElectionRoundId" = FS."ElectionRoundId"
                                   AND S."FormId" = FS."FormId"
                                   AND S."PollingStationId" = FS."PollingStationId"
                                   AND S."MonitoringObserverId" = FS."MonitoringObserverId"
                                   AND (S."CreatedAt", S."Id") <= (FS."CreatedAt", FS."Id"))
                       END AS "SubmissionNumber"
                FROM "FormSubmissions" FS
                    INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    INNER JOIN "PollingStations" PS ON PS."Id" = FS."PollingStationId"
                    INNER JOIN "MonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                    INNER JOIN "Ngos" N ON N."Id" = MN."NgoId"
                    INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                    LEFT JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                WHERE FS."ElectionRoundId" = electionRoundId
                  AND (level1 IS NULL OR CARDINALITY(level1) = 0 OR PS."Level1" = ANY(level1))
                  AND (level2 IS NULL OR CARDINALITY(level2) = 0 OR PS."Level2" = ANY(level2))
                  AND (level3 IS NULL OR CARDINALITY(level3) = 0 OR PS."Level3" = ANY(level3))
                  AND (level4 IS NULL OR CARDINALITY(level4) = 0 OR PS."Level4" = ANY(level4))
                  AND (level5 IS NULL OR CARDINALITY(level5) = 0 OR PS."Level5" = ANY(level5))
                  AND (monitoringNgoId IS NULL OR MN."Id" = monitoringNgoId)
                  AND (formId IS NULL OR FS."FormId" = formId)
                  AND (isCompleted IS NULL OR FS."IsCompleted" = isCompleted)
                  AND (fromDate IS NULL OR FS."LastUpdatedAt" >= fromDate::timestamp)
                  AND (toDate IS NULL OR FS."LastUpdatedAt" <= toDate::timestamp)
                  AND (searchText IS NULL OR PS."Number" ILIKE searchText OR PS."Address" ILIKE searchText OR F."Code" ILIKE searchText)
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetFormSubmissionViewV2"(UUID, UUID, UUID);""");

            migrationBuilder.Sql(
                """
                -- A form submission or polling station information visible to the NGO (Coalition data source) in a round, with its
                -- form, polling station, observer, answers, notes, attachments and polling station corrections (oldest first). No row
                -- when not found.
                CREATE OR REPLACE FUNCTION "GetFormSubmissionViewV2"(
                    electionRoundId UUID,
                    ngoId UUID,
                    submissionId UUID
                )
                    RETURNS TABLE
                            (
                                "SubmissionId"              UUID,
                                "FormId"                    UUID,
                                "TimeSubmitted"             TIMESTAMP WITH TIME ZONE,
                                "FormCode"                  TEXT,
                                "FormName"                  JSONB,
                                "FormType"                  TEXT,
                                "DefaultLanguage"           TEXT,
                                "Languages"                 TEXT[],
                                "PollingStationId"          UUID,
                                "Level1"                    TEXT,
                                "Level2"                    TEXT,
                                "Level3"                    TEXT,
                                "Level4"                    TEXT,
                                "Level5"                    TEXT,
                                "Number"                    TEXT,
                                "MonitoringObserverId"      UUID,
                                "ObserverName"              TEXT,
                                "Email"                     TEXT,
                                "PhoneNumber"               TEXT,
                                "Tags"                      TEXT[],
                                "NgoName"                   TEXT,
                                "IsOwnObserver"             BOOLEAN,
                                "Attachments"               JSONB,
                                "Notes"                     JSONB,
                                "Answers"                   JSONB,
                                "Questions"                 JSONB,
                                "FollowUpStatus"            TEXT,
                                "ArrivalTime"               TIMESTAMP WITH TIME ZONE,
                                "DepartureTime"             TIMESTAMP WITH TIME ZONE,
                                "Breaks"                    JSONB,
                                "IsCompleted"               BOOLEAN,
                                "PollingStationCorrections" JSONB
                            )
                AS
                $$
                WITH submissions AS
                         (SELECT psi."Id" AS "SubmissionId",
                                 'PSI' AS "FormType",
                                 'PSI' AS "FormCode",
                                 psif."Name" AS "FormName",
                                 psi."PollingStationId",
                                 psi."MonitoringObserverId",
                                 psi."Answers",
                                 psif."Questions" AS "Questions",
                                 psif."DefaultLanguage" AS "DefaultLanguage",
                                 psif."Languages" AS "Languages",
                                 psif."Id" AS "FormId",
                                 psi."FollowUpStatus" AS "FollowUpStatus",
                                 '[]'::jsonb AS "Attachments",
                                 '[]'::jsonb AS "Notes",
                                 "LastUpdatedAt" AS "TimeSubmitted",
                                 psi."ArrivalTime",
                                 psi."DepartureTime",
                                 psi."Breaks",
                                 psi."IsCompleted",
                                 '[]'::jsonb AS "PollingStationCorrections"
                          FROM "PollingStationInformation" psi
                              INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = psi."MonitoringObserverId"
                              INNER JOIN "PollingStationInformationForms" psif ON psi."ElectionRoundId" = psif."ElectionRoundId"
                          WHERE psi."Id" = submissionId AND psi."ElectionRoundId" = electionRoundId
                          UNION ALL
                          SELECT fs."Id" AS "SubmissionId",
                                 f."FormType" AS "FormType",
                                 f."Code" AS "FormCode",
                                 f."Name" AS "FormName",
                                 fs."PollingStationId",
                                 fs."MonitoringObserverId",
                                 fs."Answers",
                                 f."Questions",
                                 f."DefaultLanguage",
                                 f."Languages",
                                 f."Id" AS "FormId",
                                 fs."FollowUpStatus",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'FileName', "FileName", 'MimeType', "MimeType", 'FilePath', "FilePath", 'UploadedFileName', "UploadedFileName", 'TimeSubmitted', "LastUpdatedAt"))
                                           FROM "Attachments" a
                                           WHERE ((a."FormId" = fs."FormId" AND fs."PollingStationId" = a."PollingStationId") -- backwards compatibility
                                                  OR a."SubmissionId" = fs."Id")
                                             AND a."MonitoringObserverId" = fs."MonitoringObserverId"
                                             AND a."IsDeleted" = false
                                             AND a."IsCompleted" = true), '[]'::JSONB) AS "Attachments",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'Text', "Text", 'TimeSubmitted', "LastUpdatedAt"))
                                           FROM "Notes" n
                                           WHERE ((n."FormId" = fs."FormId" AND fs."PollingStationId" = n."PollingStationId") -- backwards compatibility
                                                  OR n."SubmissionId" = fs."Id")
                                             AND n."MonitoringObserverId" = fs."MonitoringObserverId"), '[]'::JSONB) AS "Notes",
                                 "LastUpdatedAt" AS "TimeSubmitted",
                                 NULL AS "ArrivalTime",
                                 NULL AS "DepartureTime",
                                 '[]'::jsonb AS "Breaks",
                                 fs."IsCompleted",
                                 COALESCE((SELECT jsonb_agg(jsonb_build_object(
                                                    'CorrectedAt', to_char(PSC."CorrectedAt" AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS.US"Z"'),
                                                    'From', jsonb_build_object('Id', FPS."Id", 'Number', FPS."Number", 'Level1', FPS."Level1", 'Level2', FPS."Level2", 'Level3', FPS."Level3", 'Level4', FPS."Level4", 'Level5', FPS."Level5", 'Address', FPS."Address"),
                                                    'To', jsonb_build_object('Id', TPS."Id", 'Number', TPS."Number", 'Level1', TPS."Level1", 'Level2', TPS."Level2", 'Level3', TPS."Level3", 'Level4', TPS."Level4", 'Level5', TPS."Level5", 'Address', TPS."Address")
                                                  ) ORDER BY PSC."CorrectedAt")
                                           FROM "PollingStationCorrections" PSC
                                               INNER JOIN "PollingStations" FPS ON FPS."Id" = PSC."FromPollingStationId"
                                               INNER JOIN "PollingStations" TPS ON TPS."Id" = PSC."ToPollingStationId"
                                           WHERE PSC."RecordType" = 'FormSubmission' AND PSC."RecordId" = fs."Id"), '[]'::jsonb) AS "PollingStationCorrections"
                          FROM "FormSubmissions" fs
                              INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = fs."MonitoringObserverId"
                              INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                          WHERE fs."Id" = submissionId AND fs."ElectionRoundId" = electionRoundId)
                SELECT s."SubmissionId",
                       s."FormId",
                       s."TimeSubmitted",
                       s."FormCode"::text,
                       s."FormName",
                       s."FormType",
                       s."DefaultLanguage"::text,
                       s."Languages",
                       ps."Id" AS "PollingStationId",
                       ps."Level1"::text,
                       ps."Level2"::text,
                       ps."Level3"::text,
                       ps."Level4"::text,
                       ps."Level5"::text,
                       ps."Number"::text,
                       s."MonitoringObserverId",
                       AMO."DisplayName" AS "ObserverName",
                       AMO."Email",
                       AMO."PhoneNumber",
                       AMO."Tags",
                       AMO."NgoName"::text,
                       AMO."IsOwnObserver",
                       s."Attachments",
                       s."Notes",
                       s."Answers",
                       s."Questions",
                       s."FollowUpStatus",
                       s."ArrivalTime",
                       s."DepartureTime",
                       s."Breaks",
                       s."IsCompleted",
                       s."PollingStationCorrections"
                FROM submissions s
                    INNER JOIN "PollingStations" ps ON ps."Id" = s."PollingStationId"
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, 'Coalition') AMO ON AMO."MonitoringObserverId" = s."MonitoringObserverId"
                $$ LANGUAGE sql STABLE;
                """);
        }
    }
}
