using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class SearchFormsInEveryLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // searching forms and form templates matches their name and description in every language, not only the default one
            migrationBuilder.Sql(
                """
                -- The forms the NGO sees in an election round (its own forms and the forms its coalition shares with it), matching the
                -- optional search (an ILIKE pattern on the code, id, or the name or description in any language; NULL or '' matches
                -- all), form type and status filters. FormAccess lists the NGOs a form is shared with, only for forms the NGO owns.
                -- Unordered.
                CREATE OR REPLACE FUNCTION "GetNgoFormsList"(
                    electionRoundId UUID,
                    ngoId UUID,
                    searchText TEXT,
                    formType TEXT,
                    formStatus TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "Code"                       TEXT,
                                "Name"                       JSONB,
                                "Description"                JSONB,
                                "DefaultLanguage"            TEXT,
                                "Languages"                  TEXT[],
                                "Status"                     TEXT,
                                "FormType"                   TEXT,
                                "NumberOfQuestions"          INTEGER,
                                "LanguagesTranslationStatus" JSONB,
                                "Icon"                       TEXT,
                                "LastModifiedOn"             TIMESTAMP WITH TIME ZONE,
                                "LastModifiedBy"             TEXT,
                                "IsFormOwner"                BOOLEAN,
                                "FormAccess"                 JSONB
                            )
                AS
                $$
                WITH
                    "MonitoringNgoData" AS (
                        SELECT
                            MN."ElectionRoundId",
                            MN."Id" AS "MonitoringNgoId",
                            -- Check if MonitoringNgo is a coalition leader
                            EXISTS (
                                SELECT
                                    1
                                FROM
                                    "CoalitionMemberships" CM
                                        JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                                WHERE
                                    CM."MonitoringNgoId" = MN."Id"
                                  AND CM."ElectionRoundId" = MN."ElectionRoundId"
                                  AND C."LeaderId" = MN."Id"
                            ) AS "IsCoalitionLeader",
                            -- Check if MonitoringNgo is in a coalition
                            (
                                SELECT
                                    COUNT(1)
                                FROM
                                    "CoalitionMemberships" CM
                                WHERE
                                    CM."MonitoringNgoId" = MN."Id"
                                  AND CM."ElectionRoundId" = MN."ElectionRoundId"
                            ) > 0 AS "IsInACoalition"
                        FROM
                            "MonitoringNgos" MN
                        WHERE
                            MN."ElectionRoundId" = electionRoundId
                          AND MN."NgoId" = ngoId
                        LIMIT
                            1
                    )
                SELECT
                    F."Id",
                    F."Code"::text,
                    F."Name",
                    F."Description",
                    F."DefaultLanguage"::text,
                    F."Languages",
                    F."Status",
                    F."FormType",
                    F."NumberOfQuestions",
                    F."LanguagesTranslationStatus",
                    F."Icon",
                    F."LastModifiedOn",
                    F."LastModifiedBy",
                    F."IsFormOwner",
                    CASE WHEN F."IsFormOwner" THEN COALESCE(
                            (SELECT JSONB_AGG(
                                            JSONB_BUILD_OBJECT(
                                                    'NgoId',
                                                    N."Id",
                                                    'Name',
                                                    N."Name"
                                            )
                                    )
                             FROM "CoalitionFormAccess" CFA
                                      INNER JOIN "Coalitions" C ON C."Id" = CFA."CoalitionId"
                                      INNER JOIN "MonitoringNgos" MN ON CFA."MonitoringNgoId" = MN."Id"
                                      INNER JOIN "Ngos" N ON MN."NgoId" = N."Id"
                             WHERE C."ElectionRoundId" = electionRoundId
                               AND CFA."FormId" = F."Id"),
                            '[]'::JSONB) ELSE '[]'::JSONB END
                FROM
                    (
                        SELECT
                            F."Id",
                            F."Code",
                            F."Name",
                            F."Description",
                            F."DefaultLanguage",
                            F."Languages",
                            F."Status",
                            F."FormType",
                            F."NumberOfQuestions",
                            F."LanguagesTranslationStatus",
                            F."Icon",
                            COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                            COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                            EXISTS (
                                SELECT 1
                                FROM "MonitoringNgoData"
                                WHERE "MonitoringNgoId" = F."MonitoringNgoId"
                            ) AS "IsFormOwner"
                        FROM
                            "CoalitionFormAccess" CFA
                                INNER JOIN "Coalitions" C ON CFA."CoalitionId" = C."Id"
                                INNER JOIN "Forms" F ON CFA."FormId" = F."Id"
                                LEFT JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                        WHERE
                            CFA."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                          AND C."ElectionRoundId" = electionRoundId
                          AND (
                            (SELECT "IsInACoalition" FROM "MonitoringNgoData")
                            OR (SELECT "IsCoalitionLeader" FROM "MonitoringNgoData")
                            )
                          AND (
                            searchText IS NULL
                                OR searchText = ''
                                OR F."Code" ILIKE searchText
                                OR EXISTS (SELECT 1 FROM JSONB_EACH_TEXT(F."Name") T WHERE T.value ILIKE searchText)
                                OR EXISTS (SELECT 1 FROM JSONB_EACH_TEXT(F."Description") T WHERE T.value ILIKE searchText)
                                OR F."Id"::TEXT ILIKE searchText
                            )
                          AND (formType IS NULL OR F."FormType" = formType)
                          AND (formStatus IS NULL OR F."Status" = formStatus)
                        UNION
                        SELECT
                            F."Id",
                            F."Code",
                            F."Name",
                            F."Description",
                            F."DefaultLanguage",
                            F."Languages",
                            F."Status",
                            F."FormType",
                            F."NumberOfQuestions",
                            F."LanguagesTranslationStatus",
                            F."Icon",
                            COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                            COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                            TRUE AS "IsFormOwner"
                        FROM
                            "Forms" F
                                LEFT JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                        WHERE
                            F."ElectionRoundId" = electionRoundId
                          AND F."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                          AND (
                            searchText IS NULL
                                OR searchText = ''
                                OR F."Code" ILIKE searchText
                                OR EXISTS (SELECT 1 FROM JSONB_EACH_TEXT(F."Name") T WHERE T.value ILIKE searchText)
                                OR EXISTS (SELECT 1 FROM JSONB_EACH_TEXT(F."Description") T WHERE T.value ILIKE searchText)
                                OR F."Id"::TEXT ILIKE searchText
                            )
                          AND (formType IS NULL OR F."FormType" = formType)
                          AND (formStatus IS NULL OR F."Status" = formStatus)
                    ) F
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The platform's form templates matching the optional search (an ILIKE pattern on the code, id, or the
                -- name or description in any language; NULL or '' matches all), form type and
                -- status filters, with who last modified them. Unordered.
                CREATE OR REPLACE FUNCTION "GetFormTemplatesList"(
                    searchText TEXT,
                    formType TEXT,
                    formStatus TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "Code"                       TEXT,
                                "Name"                       JSONB,
                                "Description"                JSONB,
                                "DefaultLanguage"            TEXT,
                                "Languages"                  TEXT[],
                                "Status"                     TEXT,
                                "FormType"                   TEXT,
                                "NumberOfQuestions"          INTEGER,
                                "Icon"                       TEXT,
                                "LanguagesTranslationStatus" JSONB,
                                "LastModifiedOn"             TIMESTAMP WITH TIME ZONE,
                                "LastModifiedBy"             TEXT
                            )
                AS
                $$
                SELECT FT."Id",
                       FT."Code"::text,
                       FT."Name",
                       FT."Description",
                       FT."DefaultLanguage"::text,
                       FT."Languages",
                       FT."Status",
                       FT."FormType",
                       FT."NumberOfQuestions",
                       FT."Icon",
                       FT."LanguagesTranslationStatus",
                       COALESCE(FT."LastModifiedOn", FT."CreatedOn"),
                       COALESCE(UPDATER."DisplayName", CREATOR."DisplayName")
                FROM "FormTemplates" FT
                         LEFT JOIN "AspNetUsers" CREATOR ON FT."CreatedBy" = CREATOR."Id"
                         LEFT JOIN "AspNetUsers" UPDATER ON FT."LastModifiedBy" = UPDATER."Id"
                WHERE (
                    searchText IS NULL
                        OR searchText = ''
                        OR FT."Code" ILIKE searchText
                        OR EXISTS (SELECT 1 FROM JSONB_EACH_TEXT(FT."Name") T WHERE T.value ILIKE searchText)
                        OR EXISTS (SELECT 1 FROM JSONB_EACH_TEXT(FT."Description") T WHERE T.value ILIKE searchText)
                        OR FT."Id"::TEXT ILIKE searchText
                    )
                  AND (formType IS NULL OR FT."FormType" = formType)
                  AND (formStatus IS NULL OR FT."Status" = formStatus)
                $$ LANGUAGE sql STABLE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                -- The forms the NGO sees in an election round (its own forms and the forms its coalition shares with it), matching the
                -- optional search (an ILIKE pattern; NULL or '' matches all), form type and status filters. FormAccess lists the NGOs a
                -- form is shared with, only for forms the NGO owns. Unordered.
                CREATE OR REPLACE FUNCTION "GetNgoFormsList"(
                    electionRoundId UUID,
                    ngoId UUID,
                    searchText TEXT,
                    formType TEXT,
                    formStatus TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "Code"                       TEXT,
                                "Name"                       JSONB,
                                "Description"                JSONB,
                                "DefaultLanguage"            TEXT,
                                "Languages"                  TEXT[],
                                "Status"                     TEXT,
                                "FormType"                   TEXT,
                                "NumberOfQuestions"          INTEGER,
                                "LanguagesTranslationStatus" JSONB,
                                "Icon"                       TEXT,
                                "LastModifiedOn"             TIMESTAMP WITH TIME ZONE,
                                "LastModifiedBy"             TEXT,
                                "IsFormOwner"                BOOLEAN,
                                "FormAccess"                 JSONB
                            )
                AS
                $$
                WITH
                    "MonitoringNgoData" AS (
                        SELECT
                            MN."ElectionRoundId",
                            MN."Id" AS "MonitoringNgoId",
                            -- Check if MonitoringNgo is a coalition leader
                            EXISTS (
                                SELECT
                                    1
                                FROM
                                    "CoalitionMemberships" CM
                                        JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                                WHERE
                                    CM."MonitoringNgoId" = MN."Id"
                                  AND CM."ElectionRoundId" = MN."ElectionRoundId"
                                  AND C."LeaderId" = MN."Id"
                            ) AS "IsCoalitionLeader",
                            -- Check if MonitoringNgo is in a coalition
                            (
                                SELECT
                                    COUNT(1)
                                FROM
                                    "CoalitionMemberships" CM
                                WHERE
                                    CM."MonitoringNgoId" = MN."Id"
                                  AND CM."ElectionRoundId" = MN."ElectionRoundId"
                            ) > 0 AS "IsInACoalition"
                        FROM
                            "MonitoringNgos" MN
                        WHERE
                            MN."ElectionRoundId" = electionRoundId
                          AND MN."NgoId" = ngoId
                        LIMIT
                            1
                    )
                SELECT
                    F."Id",
                    F."Code"::text,
                    F."Name",
                    F."Description",
                    F."DefaultLanguage"::text,
                    F."Languages",
                    F."Status",
                    F."FormType",
                    F."NumberOfQuestions",
                    F."LanguagesTranslationStatus",
                    F."Icon",
                    F."LastModifiedOn",
                    F."LastModifiedBy",
                    F."IsFormOwner",
                    CASE WHEN F."IsFormOwner" THEN COALESCE(
                            (SELECT JSONB_AGG(
                                            JSONB_BUILD_OBJECT(
                                                    'NgoId',
                                                    N."Id",
                                                    'Name',
                                                    N."Name"
                                            )
                                    )
                             FROM "CoalitionFormAccess" CFA
                                      INNER JOIN "Coalitions" C ON C."Id" = CFA."CoalitionId"
                                      INNER JOIN "MonitoringNgos" MN ON CFA."MonitoringNgoId" = MN."Id"
                                      INNER JOIN "Ngos" N ON MN."NgoId" = N."Id"
                             WHERE C."ElectionRoundId" = electionRoundId
                               AND CFA."FormId" = F."Id"),
                            '[]'::JSONB) ELSE '[]'::JSONB END
                FROM
                    (
                        SELECT
                            F."Id",
                            F."Code",
                            F."Name",
                            F."Description",
                            F."DefaultLanguage",
                            F."Languages",
                            F."Status",
                            F."FormType",
                            F."NumberOfQuestions",
                            F."LanguagesTranslationStatus",
                            F."Icon",
                            COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                            COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                            EXISTS (
                                SELECT 1
                                FROM "MonitoringNgoData"
                                WHERE "MonitoringNgoId" = F."MonitoringNgoId"
                            ) AS "IsFormOwner"
                        FROM
                            "CoalitionFormAccess" CFA
                                INNER JOIN "Coalitions" C ON CFA."CoalitionId" = C."Id"
                                INNER JOIN "Forms" F ON CFA."FormId" = F."Id"
                                LEFT JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                        WHERE
                            CFA."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                          AND C."ElectionRoundId" = electionRoundId
                          AND (
                            (SELECT "IsInACoalition" FROM "MonitoringNgoData")
                            OR (SELECT "IsCoalitionLeader" FROM "MonitoringNgoData")
                            )
                          AND (
                            searchText IS NULL
                                OR searchText = ''
                                OR F."Code" ILIKE searchText
                                OR F."Name" ->> F."DefaultLanguage" ILIKE searchText
                                OR F."Description" ->> F."DefaultLanguage" ILIKE searchText
                                OR F."Id"::TEXT ILIKE searchText
                            )
                          AND (formType IS NULL OR F."FormType" = formType)
                          AND (formStatus IS NULL OR F."Status" = formStatus)
                        UNION
                        SELECT
                            F."Id",
                            F."Code",
                            F."Name",
                            F."Description",
                            F."DefaultLanguage",
                            F."Languages",
                            F."Status",
                            F."FormType",
                            F."NumberOfQuestions",
                            F."LanguagesTranslationStatus",
                            F."Icon",
                            COALESCE(F."LastModifiedOn", F."CreatedOn") AS "LastModifiedOn",
                            COALESCE(UPDATER."DisplayName", CREATOR."DisplayName") AS "LastModifiedBy",
                            TRUE AS "IsFormOwner"
                        FROM
                            "Forms" F
                                LEFT JOIN "AspNetUsers" CREATOR ON F."CreatedBy" = CREATOR."Id"
                                LEFT JOIN "AspNetUsers" UPDATER ON F."LastModifiedBy" = UPDATER."Id"
                        WHERE
                            F."ElectionRoundId" = electionRoundId
                          AND F."MonitoringNgoId" = (SELECT "MonitoringNgoId" FROM "MonitoringNgoData")
                          AND (
                            searchText IS NULL
                                OR searchText = ''
                                OR F."Code" ILIKE searchText
                                OR F."Name" ->> F."DefaultLanguage" ILIKE searchText
                                OR F."Description" ->> F."DefaultLanguage" ILIKE searchText
                                OR F."Id"::TEXT ILIKE searchText
                            )
                          AND (formType IS NULL OR F."FormType" = formType)
                          AND (formStatus IS NULL OR F."Status" = formStatus)
                    ) F
                $$ LANGUAGE sql STABLE;
                """);

            migrationBuilder.Sql(
                """
                -- The platform's form templates matching the optional search (an ILIKE pattern; NULL or '' matches all), form type and
                -- status filters, with who last modified them. Unordered.
                CREATE OR REPLACE FUNCTION "GetFormTemplatesList"(
                    searchText TEXT,
                    formType TEXT,
                    formStatus TEXT
                )
                    RETURNS TABLE
                            (
                                "Id"                         UUID,
                                "Code"                       TEXT,
                                "Name"                       JSONB,
                                "Description"                JSONB,
                                "DefaultLanguage"            TEXT,
                                "Languages"                  TEXT[],
                                "Status"                     TEXT,
                                "FormType"                   TEXT,
                                "NumberOfQuestions"          INTEGER,
                                "Icon"                       TEXT,
                                "LanguagesTranslationStatus" JSONB,
                                "LastModifiedOn"             TIMESTAMP WITH TIME ZONE,
                                "LastModifiedBy"             TEXT
                            )
                AS
                $$
                SELECT FT."Id",
                       FT."Code"::text,
                       FT."Name",
                       FT."Description",
                       FT."DefaultLanguage"::text,
                       FT."Languages",
                       FT."Status",
                       FT."FormType",
                       FT."NumberOfQuestions",
                       FT."Icon",
                       FT."LanguagesTranslationStatus",
                       COALESCE(FT."LastModifiedOn", FT."CreatedOn"),
                       COALESCE(UPDATER."DisplayName", CREATOR."DisplayName")
                FROM "FormTemplates" FT
                         LEFT JOIN "AspNetUsers" CREATOR ON FT."CreatedBy" = CREATOR."Id"
                         LEFT JOIN "AspNetUsers" UPDATER ON FT."LastModifiedBy" = UPDATER."Id"
                WHERE (
                    searchText IS NULL
                        OR searchText = ''
                        OR FT."Code" ILIKE searchText
                        OR FT."Name" ->> FT."DefaultLanguage" ILIKE searchText
                        OR FT."Description" ->> FT."DefaultLanguage" ILIKE searchText
                        OR FT."Id"::TEXT ILIKE searchText
                    )
                  AND (formType IS NULL OR FT."FormType" = formType)
                  AND (formStatus IS NULL OR FT."Status" = formStatus)
                $$ LANGUAGE sql STABLE;
                """);
        }
    }
}
