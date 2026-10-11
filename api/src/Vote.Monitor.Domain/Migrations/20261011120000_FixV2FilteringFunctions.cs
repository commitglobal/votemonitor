using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations;

/// <summary>
/// Fixes the v2 entry functions (missing MonitoringNgos join, varchar/text mismatches in RETURN QUERY)
/// and adds the columns the v2 listings need, so they can read everything from a single function call.
/// </summary>
[DbContext(typeof(VoteMonitorContext))]
[Migration("20261011120000_FixV2FilteringFunctions")]
public partial class FixV2FilteringFunctions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportEntries"(uuid, uuid);""");
        migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportEntries"(uuid, uuid, text);""");
        migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportEntries"(uuid, uuid, text);""");

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

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // The previous definitions (20261010120000_AddV2FilteringViews) failed at runtime, so they are not restored.
        migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportEntries"(uuid, uuid, text);""");
        migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportEntries"(uuid, uuid, text);""");
        migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportEntries"(uuid, uuid);""");
    }
}
