using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Vote.Monitor.Domain;

#nullable disable

namespace Vote.Monitor.Domain.Migrations;

[DbContext(typeof(VoteMonitorContext))]
[Migration("20261010120000_AddV2FilteringViews")]
public partial class AddV2FilteringViews : Migration
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
                            "Answers"                    JSONB,
                            "NumberOfQuestionsAnswered"  INTEGER,
                            "NumberOfFlaggedAnswers"     INTEGER,
                            "FollowUpStatus"             TEXT,
                            "TimeSubmitted"              TIMESTAMP WITH TIME ZONE,
                            "QuestionsAnswered"          TEXT,
                            "FormType"                  TEXT,
                            "FormStatus"                TEXT,
                            "LocationId"                UUID,
                            "Level1"                    TEXT,
                            "Level2"                    TEXT,
                            "Level3"                    TEXT,
                            "Level4"                    TEXT,
                            "Level5"                    TEXT,
                            "HasNotes"                  BOOLEAN,
                            "HasComments"               BOOLEAN,
                            "HasAttachments"            BOOLEAN,
                            "NgoId"                     UUID,
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
                    CR."Answers",
                    CR."NumberOfQuestionsAnswered",
                    CR."NumberOfFlaggedAnswers",
                    CR."FollowUpStatus"::text AS "FollowUpStatus",
                    COALESCE(CR."LastModifiedOn", CR."CreatedOn") AS "TimeSubmitted",
                    CASE
                        WHEN CR."NumberOfQuestionsAnswered" = 0 THEN 'None'::text
                        WHEN F."NumberOfQuestions" = CR."NumberOfQuestionsAnswered" THEN 'All'::text
                        ELSE 'Some'::text
                    END AS "QuestionsAnswered",
                    F."FormType"::text AS "FormType",
                    F."Status"::text AS "FormStatus",
                    L."Id" AS "LocationId",
                    L."Level1"::text AS "Level1",
                    L."Level2"::text AS "Level2",
                    L."Level3"::text AS "Level3",
                    L."Level4"::text AS "Level4",
                    L."Level5"::text AS "Level5",
                    (
                        SELECT COUNT(1) > 0
                        FROM "CitizenReportNotes" N
                        WHERE N."CitizenReportId" = CR."Id"
                    ) AS "HasNotes",
                    (
                        SELECT COUNT(1) > 0
                        FROM "CitizenReportComments" C
                        WHERE C."CitizenReportId" = CR."Id"
                          AND (
                              EXISTS (
                                  SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = C."CreatedBy" AND NA."NgoId" = ngoId
                              )
                              OR EXISTS (
                                  SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = C."CreatedBy" AND NS."NgoId" = ngoId
                              )
                          )
                    ) AS "HasComments",
                    (
                        SELECT COUNT(1) > 0
                        FROM "CitizenReportAttachments" A
                        WHERE A."CitizenReportId" = CR."Id"
                          AND A."IsDeleted" = FALSE
                          AND A."IsCompleted" = TRUE
                    ) AS "HasAttachments",
                    MN."NgoId",
                    ER."CitizenReportingEnabled"
                FROM "CitizenReports" CR
                    INNER JOIN "Forms" F ON F."Id" = CR."FormId"
                    INNER JOIN "Locations" L ON L."Id" = CR."LocationId"
                    INNER JOIN "ElectionRounds" ER ON ER."Id" = CR."ElectionRoundId"
                    INNER JOIN "MonitoringNgos" MN ON MN."Id" = ER."MonitoringNgoForCitizenReportingId"
                WHERE CR."ElectionRoundId" = electionRoundId
                  AND MN."NgoId" = ngoId;
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
                            "Answers"                    JSONB,
                            "MonitoringObserverId"       UUID,
                            "PollingStationId"           UUID,
                            "LocationType"               TEXT,
                            "LocationDescription"        TEXT,
                            "NumberOfQuestionsAnswered"  INTEGER,
                            "NumberOfFlaggedAnswers"     INTEGER,
                            "FollowUpStatus"             TEXT,
                            "IsCompleted"                BOOLEAN,
                            "TimeSubmitted"              TIMESTAMP WITH TIME ZONE,
                            "QuestionsAnswered"          TEXT,
                            "FormType"                  TEXT,
                            "FormStatus"                TEXT,
                            "Level1"                    TEXT,
                            "Level2"                    TEXT,
                            "Level3"                    TEXT,
                            "Level4"                    TEXT,
                            "Level5"                    TEXT,
                            "Number"                    TEXT,
                            "HasNotes"                  BOOLEAN,
                            "HasComments"               BOOLEAN,
                            "HasAttachments"            BOOLEAN,
                            "NgoId"                     UUID
                        )
            AS
            $$
            BEGIN
                RETURN QUERY
                SELECT
                    IR."Id",
                    IR."ElectionRoundId",
                    IR."FormId",
                    IR."Answers",
                    IR."MonitoringObserverId",
                    IR."PollingStationId",
                    IR."LocationType"::text AS "LocationType",
                    IR."LocationDescription",
                    IR."NumberOfQuestionsAnswered",
                    IR."NumberOfFlaggedAnswers",
                    IR."FollowUpStatus"::text AS "FollowUpStatus",
                    IR."IsCompleted",
                    IR."LastUpdatedAt" AS "TimeSubmitted",
                    CASE
                        WHEN IR."NumberOfQuestionsAnswered" = 0 THEN 'None'::text
                        WHEN F."NumberOfQuestions" = IR."NumberOfQuestionsAnswered" THEN 'All'::text
                        ELSE 'Some'::text
                    END AS "QuestionsAnswered",
                    F."FormType"::text AS "FormType",
                    F."Status"::text AS "FormStatus",
                    PS."Level1"::text AS "Level1",
                    PS."Level2"::text AS "Level2",
                    PS."Level3"::text AS "Level3",
                    PS."Level4"::text AS "Level4",
                    PS."Level5"::text AS "Level5",
                    PS."Number"::text AS "Number",
                    (
                        SELECT COUNT(1) > 0
                        FROM "IncidentReportNotes" N
                        WHERE N."IncidentReportId" = IR."Id"
                    ) AS "HasNotes",
                    (
                        SELECT COUNT(1) > 0
                        FROM "IncidentReportComments" C
                        WHERE C."IncidentReportId" = IR."Id"
                          AND (
                              EXISTS (
                                  SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = C."CreatedBy" AND NA."NgoId" = ngoId
                              )
                              OR EXISTS (
                                  SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = C."CreatedBy" AND NS."NgoId" = ngoId
                              )
                          )
                    ) AS "HasComments",
                    (
                        SELECT COUNT(1) > 0
                        FROM "IncidentReportAttachments" A
                        WHERE A."IncidentReportId" = IR."Id"
                          AND A."IsDeleted" = FALSE
                          AND A."IsCompleted" = TRUE
                    ) AS "HasAttachments",
                    MN."NgoId"
                FROM "IncidentReports" IR
                    INNER JOIN "Forms" F ON F."Id" = IR."FormId"
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO ON AMO."MonitoringObserverId" = IR."MonitoringObserverId"
                    LEFT JOIN "PollingStations" PS ON PS."Id" = IR."PollingStationId"
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
                            "Id"                  UUID,
                            "ElectionRoundId"     UUID,
                            "MonitoringObserverId" UUID,
                            "PollingStationId"     UUID,
                            "LocationType"        TEXT,
                            "IncidentCategory"    TEXT,
                            "FollowUpStatus"      TEXT,
                            "TimeSubmitted"       TIMESTAMP WITH TIME ZONE,
                            "Title"               TEXT,
                            "Description"         TEXT,
                            "PollingStationDetails" JSONB,
                            "Level1"             TEXT,
                            "Level2"             TEXT,
                            "Level3"             TEXT,
                            "Level4"             TEXT,
                            "Level5"             TEXT,
                            "Number"             TEXT,
                            "Address"            TEXT,
                            "HasComments"        BOOLEAN,
                            "HasAttachments"     BOOLEAN
                        )
            AS
            $$
            BEGIN
                RETURN QUERY
                SELECT
                    QR."Id",
                    QR."ElectionRoundId",
                    QR."MonitoringObserverId",
                    QR."PollingStationId",
                    QR."QuickReportLocationType"::text AS "LocationType",
                    QR."IncidentCategory"::text AS "IncidentCategory",
                    QR."FollowUpStatus"::text AS "FollowUpStatus",
                    QR."LastUpdatedAt" AS "TimeSubmitted",
                    QR."Title",
                    QR."Description",
                    QR."PollingStationDetails",
                    PS."Level1"::text AS "Level1",
                    PS."Level2"::text AS "Level2",
                    PS."Level3"::text AS "Level3",
                    PS."Level4"::text AS "Level4",
                    PS."Level5"::text AS "Level5",
                    PS."Number"::text AS "Number",
                    PS."Address",
                    (
                        SELECT COUNT(1) > 0
                        FROM "QuickReportComments" C
                        WHERE C."QuickReportId" = QR."Id"
                          AND (
                              EXISTS (
                                  SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = C."CreatedBy" AND NA."NgoId" = ngoId
                              )
                              OR EXISTS (
                                  SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = C."CreatedBy" AND NS."NgoId" = ngoId
                              )
                          )
                    ) AS "HasComments",
                    (
                        SELECT COUNT(1) > 0
                        FROM "QuickReportAttachments" A
                        WHERE A."QuickReportId" = QR."Id"
                          AND A."IsDeleted" = FALSE
                          AND A."IsCompleted" = TRUE
                    ) AS "HasAttachments"
                FROM "QuickReports" QR
                    INNER JOIN "GetAvailableMonitoringObservers"(electionRoundId, ngoId, dataSource) AMO ON AMO."MonitoringObserverId" = QR."MonitoringObserverId"
                    LEFT JOIN "PollingStations" PS ON PS."Id" = QR."PollingStationId"
                WHERE QR."ElectionRoundId" = electionRoundId;
            END;
            $$ LANGUAGE plpgsql;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetQuickReportEntries"(uuid, uuid, text);""");
        migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetIncidentReportEntries"(uuid, uuid, text);""");
        migrationBuilder.Sql("""DROP FUNCTION IF EXISTS "GetCitizenReportEntries"(uuid, uuid);""");
    }
}
