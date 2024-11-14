using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListEntries;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<PagedResponse<FormSubmissionEntry>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byEntry");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x => { x.Summary = "Lists form submissions by entry in our system"; });
    }

    public override async Task<Results<Ok<PagedResponse<FormSubmissionEntry>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  WITH "MonitoringNgoDetails" AS (SELECT MN."ElectionRoundId",
                                                         MN."Id"                               AS "MonitoringNgoId",
                                                         -- Check if MonitoringNgo is a coalition leader
                                                         EXISTS (SELECT 1
                                                                 FROM "CoalitionMemberships" CM
                                                                          JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                                                                 WHERE CM."MonitoringNgoId" = MN."Id"
                                                                   AND CM."ElectionRoundId" = MN."ElectionRoundId"
                                                                   AND C."LeaderId" = MN."Id") AS "IsCoalitionLeader",
                                                         -- Get coalition id
                                                         (SELECT C."Id"
                                                          FROM "CoalitionMemberships" CM
                                                                   JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                                                          WHERE CM."MonitoringNgoId" = MN."Id"
                                                            AND CM."ElectionRoundId" = MN."ElectionRoundId"
                                                          LIMIT 1)                             AS "CoalitionId"
                                                  FROM "MonitoringNgos" MN
                                                  WHERE MN."ElectionRoundId" = @electionRoundId
                                                    AND MN."NgoId" = @ngoId
                                                  LIMIT 1),
                       -- if ngo is coalition leader they need to see all the responses
                       "AvailableMonitoringObservers" AS (SELECT MO."Id",
                                                                 CASE
                                                                     WHEN (
                                                                         (SELECT "IsCoalitionLeader"
                                                                          FROM "MonitoringNgoDetails")
                                                                             AND MN."NgoId" <> @ngoId
                                                                         ) THEN MO."Id"::TEXT
                                                                     ELSE U."DisplayName"
                                                                     END     AS "DisplayName",
                                                                 CASE
                                                                     WHEN (
                                                                         (SELECT "IsCoalitionLeader"
                                                                          FROM "MonitoringNgoDetails")
                                                                             AND MN."NgoId" <> @ngoId
                                                                         ) THEN MO."Id"::TEXT
                                                                     ELSE U."Email"
                                                                     END     AS "Email",
                                                                 CASE
                                                                     WHEN (
                                                                         (SELECT "IsCoalitionLeader"
                                                                          FROM "MonitoringNgoDetails")
                                                                             AND MN."NgoId" <> @ngoId
                                                                         ) THEN MO."Id"::TEXT
                                                                     ELSE U."PhoneNumber"
                                                                     END     AS "PhoneNumber",
                                                                 CASE
                                                                     WHEN (
                                                                         (SELECT "IsCoalitionLeader"
                                                                          FROM "MonitoringNgoDetails")
                                                                             AND MN."NgoId" <> @ngoId
                                                                         ) THEN '{}'::TEXT[]
                                                                     ELSE MO."Tags"
                                                                     END     AS "Tags",
                                                                 MO."Status" AS "Status"
                                                          FROM "Coalitions" C
                                                                   INNER JOIN "MonitoringNgoDetails" MND ON MND."CoalitionId" = C."Id"
                                                                   INNER JOIN "CoalitionMemberships" CM ON C."Id" = CM."CoalitionId"
                                                                   INNER JOIN "MonitoringObservers" MO ON MO."MonitoringNgoId" = CM."MonitoringNgoId"
                                                                   INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                                                                   INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                                                          WHERE MND."CoalitionId" IS NOT NULL
                                                            -- Case 1: `@dataSource` is "Ngo" or "Coalition" and they are not in a coalition
                                                            AND (
                                                              (
                                                                  (
                                                                      @dataSource = 'Ngo'
                                                                          OR @dataSource = 'Coalition'
                                                                      )
                                                                      AND NOT EXISTS (SELECT 1
                                                                                      FROM "MonitoringNgoDetails")
                                                                      AND MN."NgoId" = @ngoId
                                                                  )
                                                                  -- Case 2: `@dataSource` is "Coalition" and they are a coalition leader
                                                                  OR (
                                                                  @dataSource = 'Coalition'
                                                                      AND EXISTS (SELECT 1
                                                                                  FROM "MonitoringNgoDetails"
                                                                                  WHERE "IsCoalitionLeader")
                                                                  )
                                                                  -- Case 3: `@dataSource` is "Ngo" and they are a coalition leader, apply `MN."NgoId" = @ngoId`
                                                                  OR (
                                                                  @dataSource = 'Ngo'
                                                                      AND EXISTS (SELECT 1
                                                                                  FROM "MonitoringNgoDetails"
                                                                                  WHERE "IsCoalitionLeader")
                                                                      AND MN."NgoId" = @ngoId
                                                                  )
                                                                  -- Case 4: For all other cases, apply `MN."NgoId" = @ngoId`
                                                                  OR MN."NgoId" = @ngoId
                                                              )
                                                          UNION
                                                          SELECT MO."Id",
                                                                 U."DisplayName" AS "DisplayName",
                                                                 U."Email"       AS "Email",
                                                                 U."PhoneNumber" AS "PhoneNumber",
                                                                 MO."Tags"       AS "Tags",
                                                                 MO."Status"     AS "Status"
                                                          FROM "MonitoringObservers" MO
                                                                   INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                                                                   INNER JOIN "MonitoringNgoDetails" MND
                                                                              ON MND."MonitoringNgoId" = MO."MonitoringNgoId"
                                                                   INNER JOIN "MonitoringNgos" MN on mn."Id" = mo."MonitoringNgoId"
                                                          WHERE MND."CoalitionId" IS NULL
                                                            and mn."NgoId" = @ngoId),
                       "AvailableForms" AS (SELECT F."Id"
                                            FROM "CoalitionFormAccess" CFA
                                                     INNER JOIN "Coalitions" C ON CFA."CoalitionId" = C."Id"
                                                     inner join "MonitoringNgoDetails" mnd on mnd."CoalitionId" = c."Id"
                                                     Inner join "MonitoringNgos" mn on mnd."MonitoringNgoId" = mn."Id"
                                                     INNER JOIN "Forms" F ON CFA."FormId" = F."Id"
                                            WHERE MND."CoalitionId" IS NOT NULL
                                              -- Case 1: `@dataSource` is "Ngo" or "Coalition" and they are not in a coalition
                                              AND (
                                                (
                                                    (@dataSource = 'Ngo' OR @dataSource = 'Coalition')
                                                        AND NOT EXISTS (SELECT 1
                                                                        FROM "MonitoringNgoDetails")
                                                        AND MN."NgoId" = @ngoId
                                                    )
                                                    -- Case 2: `@dataSource` is "Coalition" and they are a coalition leader
                                                    OR (@dataSource = 'Coalition' AND EXISTS (SELECT 1
                                                                                              FROM "MonitoringNgoDetails"
                                                                                              WHERE "IsCoalitionLeader")
                                                    )
                                                    -- Case 3: `@dataSource` is "Ngo" and they are a coalition leader, apply `MN."NgoId" = @ngoId`
                                                    OR (
                                                    @dataSource = 'Ngo'
                                                        AND EXISTS (SELECT 1
                                                                    FROM "MonitoringNgoDetails"
                                                                    WHERE "IsCoalitionLeader")
                                                        AND MN."NgoId" = @ngoId
                                                    )
                                                    -- Case 4: For all other cases, apply `MN."NgoId" = @ngoId`
                                                    OR MN."NgoId" = @ngoId
                                                )
                                            UNION
                                            SELECT F."Id"
                                            FROM "Forms" F
                                                     INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
                                            WHERE f."ElectionRoundId" = @electionRoundId
                                              AND MN."NgoId" = @ngoId)
                  SELECT SUM(count)
                  FROM (SELECT count(*) AS count
                        FROM "PollingStationInformation" psi
                                 INNER JOIN "PollingStationInformationForms" psif ON psif."Id" = psi."PollingStationInformationFormId"
                                 INNER JOIN "PollingStations" ps ON ps."Id" = psi."PollingStationId"
                                 INNER JOIN "AvailableMonitoringObservers" mo ON mo."Id" = psi."MonitoringObserverId"
                        WHERE psi."ElectionRoundId" = @electionRoundId
                          AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                          AND (@searchText IS NULL
                            OR @searchText = ''
                            OR mo."DisplayName" ILIKE @searchText
                            OR mo."Email" ILIKE @searchText
                            OR mo."PhoneNumber" ILIKE @searchText
                            OR mo."Id"::TEXT ILIKE @searchText)
                          AND (@formType IS NULL OR 'PSI' = @formType)
                          AND (@level1 IS NULL OR ps."Level1" = @level1)
                          AND (@level2 IS NULL OR ps."Level2" = @level2)
                          AND (@level3 IS NULL OR ps."Level3" = @level3)
                          AND (@level4 IS NULL OR ps."Level4" = @level4)
                          AND (@level5 IS NULL OR ps."Level5" = @level5)
                          AND (@pollingStationNumber IS NULL OR ps."Number" = @pollingStationNumber)
                          AND (@hasFlaggedAnswers is NULL OR @hasFlaggedAnswers = false OR 1 = 2)
                          AND (@followUpStatus is NULL OR psi."FollowUpStatus" = @followUpStatus)
                          AND (@monitoringObserverStatus IS NULL OR mo."Status" = @monitoringObserverStatus)
                          AND (@formId IS NULL OR psi."PollingStationInformationFormId" = @formId)
                          AND (@questionsAnswered is null
                            OR (@questionsAnswered = 'All' AND psif."NumberOfQuestions" = psi."NumberOfQuestionsAnswered")
                            OR (@questionsAnswered = 'Some' AND psif."NumberOfQuestions" <> psi."NumberOfQuestionsAnswered")
                            OR (@questionsAnswered = 'None' AND psi."NumberOfQuestionsAnswered" = 0))
                          AND (@hasNotes is NULL OR (TRUE AND @hasNotes = false) OR (FALSE AND @hasNotes = true))
                          AND (@hasAttachments is NULL OR (TRUE AND @hasAttachments = false) OR (FALSE AND @hasAttachments = true))
                          AND (@fromDate is NULL OR COALESCE(PSI."LastModifiedOn", PSI."CreatedOn") >= @fromDate::timestamp)
                          AND (@toDate is NULL OR COALESCE(PSI."LastModifiedOn", PSI."CreatedOn") <= @toDate::timestamp)
                        UNION ALL
                        SELECT count(*) AS count
                        FROM "FormSubmissions" fs
                                 INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                                 INNER JOIN "AvailableForms" AF ON AF."Id" = fs."FormId"
                                 INNER JOIN "PollingStations" ps ON ps."Id" = fs."PollingStationId"
                                 INNER JOIN "AvailableMonitoringObservers" mo ON mo."Id" = fs."MonitoringObserverId"
                        WHERE fs."ElectionRoundId" = @electionRoundId
                          AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                          AND (@searchText IS NULL
                            OR @searchText = ''
                            OR mo."DisplayName" ILIKE @searchText
                            OR mo."Email" ILIKE @searchText
                            OR mo."PhoneNumber" ILIKE @searchText
                            OR mo."Id"::TEXT ILIKE @searchText)
                          AND (@formType IS NULL OR f."FormType" = @formType)
                          AND (@level1 IS NULL OR ps."Level1" = @level1)
                          AND (@level2 IS NULL OR ps."Level2" = @level2)
                          AND (@level3 IS NULL OR ps."Level3" = @level3)
                          AND (@level4 IS NULL OR ps."Level4" = @level4)
                          AND (@level5 IS NULL OR ps."Level5" = @level5)
                          AND (@pollingStationNumber IS NULL OR ps."Number" = @pollingStationNumber)
                          AND (@hasFlaggedAnswers is NULL OR (fs."NumberOfFlaggedAnswers" = 0 AND @hasFlaggedAnswers = false) OR
                               ("NumberOfFlaggedAnswers" > 0 AND @hasFlaggedAnswers = true))
                          AND (@followUpStatus is NULL OR fs."FollowUpStatus" = @followUpStatus)
                          AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo."Tags" && @tagsFilter)
                          AND (@monitoringObserverStatus IS NULL OR mo."Status" = @monitoringObserverStatus)
                          AND (@formId IS NULL OR fs."FormId" = @formId)
                          AND (@questionsAnswered is null
                            OR (@questionsAnswered = 'All' AND f."NumberOfQuestions" = fs."NumberOfQuestionsAnswered")
                            OR (@questionsAnswered = 'Some' AND f."NumberOfQuestions" <> fs."NumberOfQuestionsAnswered")
                            OR (@questionsAnswered = 'None' AND fs."NumberOfQuestionsAnswered" = 0))
                          AND (@hasAttachments is NULL
                            OR ((SELECT COUNT(1)
                                 FROM "Attachments" A
                                 WHERE A."FormId" = fs."FormId"
                                   AND A."MonitoringObserverId" = fs."MonitoringObserverId"
                                   AND fs."PollingStationId" = A."PollingStationId"
                                   AND A."IsDeleted" = false
                                   AND A."IsCompleted" = true) = 0 AND @hasAttachments = false)
                            OR ((SELECT COUNT(1)
                                 FROM "Attachments" A
                                 WHERE A."FormId" = fs."FormId"
                                   AND A."MonitoringObserverId" = fs."MonitoringObserverId"
                                   AND fs."PollingStationId" = A."PollingStationId"
                                   AND A."IsDeleted" = false
                                   AND A."IsCompleted" = true) > 0 AND @hasAttachments = true))
                          AND (@hasNotes is NULL
                            OR ((SELECT COUNT(1)
                                 FROM "Notes" N
                                 WHERE N."FormId" = fs."FormId"
                                   AND N."MonitoringObserverId" = fs."MonitoringObserverId"
                                   AND fs."PollingStationId" = N."PollingStationId") = 0 AND @hasNotes = false)
                            OR ((SELECT COUNT(1)
                                 FROM "Notes" N
                                 WHERE N."FormId" = fs."FormId"
                                   AND N."MonitoringObserverId" = fs."MonitoringObserverId"
                                   AND fs."PollingStationId" = N."PollingStationId") > 0 AND @hasNotes = true))
                          AND (@fromDate is NULL OR COALESCE(FS."LastModifiedOn", FS."CreatedOn") >= @fromDate::timestamp)
                          AND (@toDate is NULL OR COALESCE(FS."LastModifiedOn", FS."CreatedOn") <= @toDate::timestamp)) c;
                  
                  WITH "MonitoringNgoDetails" AS (SELECT MN."ElectionRoundId",
                                                         MN."Id"                               AS "MonitoringNgoId",
                                                         -- Check if MonitoringNgo is a coalition leader
                                                         EXISTS (SELECT 1
                                                                 FROM "CoalitionMemberships" CM
                                                                          JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                                                                 WHERE CM."MonitoringNgoId" = MN."Id"
                                                                   AND CM."ElectionRoundId" = MN."ElectionRoundId"
                                                                   AND C."LeaderId" = MN."Id") AS "IsCoalitionLeader",
                                                         -- Get coalition id
                                                         (SELECT C."Id"
                                                          FROM "CoalitionMemberships" CM
                                                                   JOIN "Coalitions" C ON CM."CoalitionId" = C."Id"
                                                          WHERE CM."MonitoringNgoId" = MN."Id"
                                                            AND CM."ElectionRoundId" = MN."ElectionRoundId"
                                                          LIMIT 1)                             AS "CoalitionId"
                                                  FROM "MonitoringNgos" MN
                                                  WHERE MN."ElectionRoundId" = @electionRoundId
                                                    AND MN."NgoId" = @ngoId
                                                  LIMIT 1),
                       -- if ngo is coalition leader they need to see all the responses
                       "AvailableMonitoringObservers" AS (SELECT MO."Id",
                                                                 CASE
                                                                     WHEN (
                                                                         (SELECT "IsCoalitionLeader"
                                                                          FROM "MonitoringNgoDetails")
                                                                             AND MN."NgoId" <> @ngoId
                                                                         ) THEN MO."Id"::TEXT
                                                                     ELSE U."DisplayName"
                                                                     END     AS "DisplayName",
                                                                 CASE
                                                                     WHEN (
                                                                         (SELECT "IsCoalitionLeader"
                                                                          FROM "MonitoringNgoDetails")
                                                                             AND MN."NgoId" <> @ngoId
                                                                         ) THEN MO."Id"::TEXT
                                                                     ELSE U."Email"
                                                                     END     AS "Email",
                                                                 CASE
                                                                     WHEN (
                                                                         (SELECT "IsCoalitionLeader"
                                                                          FROM "MonitoringNgoDetails")
                                                                             AND MN."NgoId" <> @ngoId
                                                                         ) THEN MO."Id"::TEXT
                                                                     ELSE U."PhoneNumber"
                                                                     END     AS "PhoneNumber",
                                                                 CASE
                                                                     WHEN (
                                                                         (SELECT "IsCoalitionLeader"
                                                                          FROM "MonitoringNgoDetails")
                                                                             AND MN."NgoId" <> @ngoId
                                                                         ) THEN '{}'::TEXT[]
                                                                     ELSE MO."Tags"
                                                                     END     AS "Tags",
                                                                 MO."Status" AS "Status"
                                                          FROM "Coalitions" C
                                                                   INNER JOIN "MonitoringNgoDetails" MND ON MND."CoalitionId" = C."Id"
                                                                   INNER JOIN "CoalitionMemberships" CM ON C."Id" = CM."CoalitionId"
                                                                   INNER JOIN "MonitoringObservers" MO ON MO."MonitoringNgoId" = CM."MonitoringNgoId"
                                                                   INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                                                                   INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                                                          WHERE MND."CoalitionId" IS NOT NULL
                                                            -- Case 1: `@dataSource` is "Ngo" or "Coalition" and they are not in a coalition
                                                            AND (
                                                              (
                                                                  (
                                                                      @dataSource = 'Ngo'
                                                                          OR @dataSource = 'Coalition'
                                                                      )
                                                                      AND NOT EXISTS (SELECT 1
                                                                                      FROM "MonitoringNgoDetails")
                                                                      AND MN."NgoId" = @ngoId
                                                                  )
                                                                  -- Case 2: `@dataSource` is "Coalition" and they are a coalition leader
                                                                  OR (
                                                                  @dataSource = 'Coalition'
                                                                      AND EXISTS (SELECT 1
                                                                                  FROM "MonitoringNgoDetails"
                                                                                  WHERE "IsCoalitionLeader")
                                                                  )
                                                                  -- Case 3: `@dataSource` is "Ngo" and they are a coalition leader, apply `MN."NgoId" = @ngoId`
                                                                  OR (
                                                                  @dataSource = 'Ngo'
                                                                      AND EXISTS (SELECT 1
                                                                                  FROM "MonitoringNgoDetails"
                                                                                  WHERE "IsCoalitionLeader")
                                                                      AND MN."NgoId" = @ngoId
                                                                  )
                                                                  -- Case 4: For all other cases, apply `MN."NgoId" = @ngoId`
                                                                  OR MN."NgoId" = @ngoId
                                                              )
                                                          UNION
                                                          SELECT MO."Id",
                                                                 U."DisplayName" AS "DisplayName",
                                                                 U."Email"       AS "Email",
                                                                 U."PhoneNumber" AS "PhoneNumber",
                                                                 MO."Tags"       AS "Tags",
                                                                 MO."Status"     AS "Status"
                                                          FROM "MonitoringObservers" MO
                                                                   INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                                                                   INNER JOIN "MonitoringNgoDetails" MND
                                                                              ON MND."MonitoringNgoId" = MO."MonitoringNgoId"
                                                                   INNER JOIN "MonitoringNgos" MN on mn."Id" = mo."MonitoringNgoId"
                                                          WHERE MND."CoalitionId" IS NULL
                                                            and mn."NgoId" = @ngoId),
                       "AvailableForms" AS (SELECT F."Id"
                                            FROM "CoalitionFormAccess" CFA
                                                     INNER JOIN "Coalitions" C ON CFA."CoalitionId" = C."Id"
                                                     inner join "MonitoringNgoDetails" mnd on mnd."CoalitionId" = c."Id"
                                                     INNER JOIN "Forms" F ON CFA."FormId" = F."Id"
                                                     inner join "MonitoringNgos" mn on mn."Id" = mnd."MonitoringNgoId"
                                            WHERE MND."CoalitionId" IS NOT NULL
                                              -- Case 1: `@dataSource` is "Ngo" or "Coalition" and they are not in a coalition
                                              AND (
                                                (
                                                    (@dataSource = 'Ngo' OR @dataSource = 'Coalition')
                                                        AND NOT EXISTS (SELECT 1
                                                                        FROM "MonitoringNgoDetails")
                                                        AND MN."NgoId" = @ngoId
                                                    )
                                                    -- Case 2: `@dataSource` is "Coalition" and they are a coalition leader
                                                    OR (@dataSource = 'Coalition' AND EXISTS (SELECT 1
                                                                                              FROM "MonitoringNgoDetails"
                                                                                              WHERE "IsCoalitionLeader")
                                                    )
                                                    -- Case 3: `@dataSource` is "Ngo" and they are a coalition leader, apply `MN."NgoId" = @ngoId`
                                                    OR (
                                                    @dataSource = 'Ngo'
                                                        AND EXISTS (SELECT 1
                                                                    FROM "MonitoringNgoDetails"
                                                                    WHERE "IsCoalitionLeader")
                                                        AND MN."NgoId" = @ngoId
                                                    )
                                                    -- Case 4: For all other cases, apply `MN."NgoId" = @ngoId`
                                                    OR MN."NgoId" = @ngoId
                                                )
                                            UNION
                                            SELECT F."Id"
                                            FROM "Forms" F
                                                     INNER JOIN "MonitoringNgos" MN ON MN."Id" = F."MonitoringNgoId"
                                            WHERE f."ElectionRoundId" = @electionRoundId
                                              AND MN."NgoId" = @ngoId),
                       polling_station_submissions AS (SELECT psi."Id" AS                                     "SubmissionId",
                                                              'PSI'    AS                                     "FormType",
                                                              'PSI'    AS                                     "FormCode",
                                                              psi."PollingStationId",
                                                              psi."MonitoringObserverId",
                                                              psi."NumberOfQuestionsAnswered",
                                                              psi."NumberOfFlaggedAnswers",
                                                              0        AS                                     "MediaFilesCount",
                                                              0        AS                                     "NotesCount",
                                                              COALESCE(psi."LastModifiedOn", psi."CreatedOn") "TimeSubmitted",
                                                              psi."FollowUpStatus",
                                                              psif."DefaultLanguage",
                                                              psif."Name",
                                                              psi."IsCompleted"
                                                       FROM "PollingStationInformation" psi
                                                                INNER JOIN "PollingStationInformationForms" psif
                                                                           ON psif."Id" = psi."PollingStationInformationFormId"
                                                                INNER JOIN "AvailableMonitoringObservers" mo
                                                                           ON mo."Id" = psi."MonitoringObserverId"
                                                       WHERE psi."ElectionRoundId" = @electionRoundId
                                                         AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                                                         AND (@searchText IS NULL OR @searchText = ''
                                                           OR mo."DisplayName" ILIKE @searchText
                                                           OR mo."Email" ILIKE @searchText
                                                           OR mo."PhoneNumber" ILIKE @searchText
                                                           OR mo."Id"::TEXT ILIKE @searchText)
                                                         AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                                                         AND (@monitoringObserverStatus IS NULL OR
                                                              mo."Status" = @monitoringObserverStatus)
                                                         AND (@formId IS NULL OR psi."PollingStationInformationFormId" = @formId)
                                                         AND (@fromDate is NULL OR
                                                              COALESCE(PSI."LastModifiedOn", PSI."CreatedOn") >= @fromDate::timestamp)
                                                         AND (@toDate is NULL OR
                                                              COALESCE(PSI."LastModifiedOn", PSI."CreatedOn") <= @toDate::timestamp)
                                                         AND (@questionsAnswered IS NULL
                                                           OR (@questionsAnswered = 'All' AND
                                                               psif."NumberOfQuestions" = psi."NumberOfQuestionsAnswered")
                                                           OR (@questionsAnswered = 'Some' AND
                                                               psif."NumberOfQuestions" <> psi."NumberOfQuestionsAnswered")
                                                           OR (@questionsAnswered = 'None' AND psi."NumberOfQuestionsAnswered" = 0))),
                       form_submissions AS (SELECT fs."Id"                                              AS "SubmissionId",
                                                   f."FormType",
                                                   f."Code"                                             AS "FormCode",
                                                   fs."PollingStationId",
                                                   fs."MonitoringObserverId",
                                                   fs."NumberOfQuestionsAnswered",
                                                   fs."NumberOfFlaggedAnswers",
                                                   (SELECT COUNT(1)
                                                    FROM "Attachments" A
                                                    WHERE A."FormId" = fs."FormId"
                                                      AND a."MonitoringObserverId" = fs."MonitoringObserverId"
                                                      AND fs."PollingStationId" = A."PollingStationId"
                                                      AND A."IsDeleted" = false
                                                      AND A."IsCompleted" = true)                       AS "MediaFilesCount",
                                                   (SELECT COUNT(1)
                                                    FROM "Notes" N
                                                    WHERE N."FormId" = fs."FormId"
                                                      AND N."MonitoringObserverId" = fs."MonitoringObserverId"
                                                      AND fs."PollingStationId" = N."PollingStationId") AS "NotesCount",
                                                   COALESCE(fs."LastModifiedOn", fs."CreatedOn")        AS "TimeSubmitted",
                                                   fs."FollowUpStatus",
                                                   f."DefaultLanguage",
                                                   f."Name",
                                                   fs."IsCompleted"
                                            FROM "FormSubmissions" fs
                                                     INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                                                     INNER JOIN "AvailableMonitoringObservers" mo ON fs."MonitoringObserverId" = mo."Id"
                                                     INNER JOIN "AvailableForms" AF ON AF."Id" = fs."FormId"
                                            WHERE fs."ElectionRoundId" = @electionRoundId
                                              AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                                              AND (@searchText IS NULL OR @searchText = ''
                                                OR mo."DisplayName" ILIKE @searchText
                                                OR mo."Email" ILIKE @searchText
                                                OR mo."PhoneNumber" ILIKE @searchText
                                                OR mo."Id"::TEXT ILIKE @searchText)
                                              AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                                              AND (@monitoringObserverStatus IS NULL OR mo."Status" = @monitoringObserverStatus)
                                              AND (@formId IS NULL OR fs."FormId" = @formId)
                                              AND (@fromDate is NULL OR
                                                   COALESCE(FS."LastModifiedOn", FS."CreatedOn") >= @fromDate::timestamp)
                                              AND (@toDate is NULL OR COALESCE(FS."LastModifiedOn", FS."CreatedOn") <= @toDate::timestamp)
                                              AND (@questionsAnswered IS NULL
                                                OR (@questionsAnswered = 'All' AND f."NumberOfQuestions" = fs."NumberOfQuestionsAnswered")
                                                OR (@questionsAnswered = 'Some' AND
                                                    f."NumberOfQuestions" <> fs."NumberOfQuestionsAnswered")
                                                OR (@questionsAnswered = 'None' AND fs."NumberOfQuestionsAnswered" = 0)))
                  SELECT s."SubmissionId",
                         s."TimeSubmitted",
                         s."FormCode",
                         s."FormType",
                         s."DefaultLanguage",
                         s."Name"         as "FormName",
                         ps."Id"          AS "PollingStationId",
                         ps."Level1",
                         ps."Level2",
                         ps."Level3",
                         ps."Level4",
                         ps."Level5",
                         ps."Number",
                         s."MonitoringObserverId",
                         mo."DisplayName" AS "ObserverName",
                         mo."Email",
                         mo."PhoneNumber",
                         mo."Status",
                         mo."Tags",
                         s."NumberOfQuestionsAnswered",
                         s."NumberOfFlaggedAnswers",
                         s."MediaFilesCount",
                         s."NotesCount",
                         s."FollowUpStatus",
                         s."IsCompleted",
                         mo."Status"         "MonitoringObserverStatus"
                  FROM (SELECT *
                        FROM polling_station_submissions
                        UNION ALL
                        SELECT *
                        FROM form_submissions) s
                           INNER JOIN "PollingStations" ps ON ps."Id" = s."PollingStationId"
                           INNER JOIN "AvailableMonitoringObservers" mo ON mo."Id" = s."MonitoringObserverId"
                  WHERE (@formType IS NULL OR s."FormType" = @formType)
                    AND (@level1 IS NULL OR ps."Level1" = @level1)
                    AND (@level2 IS NULL OR ps."Level2" = @level2)
                    AND (@level3 IS NULL OR ps."Level3" = @level3)
                    AND (@level4 IS NULL OR ps."Level4" = @level4)
                    AND (@level5 IS NULL OR ps."Level5" = @level5)
                    AND (@pollingStationNumber IS NULL OR ps."Number" = @pollingStationNumber)
                    AND (@hasFlaggedAnswers IS NULL
                      OR (s."NumberOfFlaggedAnswers" = 0 AND @hasFlaggedAnswers = false)
                      OR (s."NumberOfFlaggedAnswers" > 0 AND @hasFlaggedAnswers = true))
                    AND (@followUpStatus IS NULL OR s."FollowUpStatus" = @followUpStatus)
                    AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo."Tags" && @tagsFilter)
                    AND (@hasNotes IS NULL
                      OR (s."NotesCount" = 0 AND @hasNotes = false)
                      OR (s."NotesCount" > 0 AND @hasNotes = true))
                    AND (@hasAttachments IS NULL
                      OR (s."MediaFilesCount" = 0 AND @hasAttachments = false)
                      OR (s."MediaFilesCount" > 0 AND @hasAttachments = true))
                  ORDER BY CASE WHEN @sortExpression = 'TimeSubmitted ASC' THEN s."TimeSubmitted" END ASC,
                           CASE WHEN @sortExpression = 'TimeSubmitted DESC' THEN s."TimeSubmitted" END DESC,
                           CASE WHEN @sortExpression = 'FormCode ASC' THEN s."FormCode" END ASC,
                           CASE WHEN @sortExpression = 'FormCode DESC' THEN s."FormCode" END DESC,
                           CASE WHEN @sortExpression = 'FormType ASC' THEN s."FormType" END ASC,
                           CASE WHEN @sortExpression = 'FormType DESC' THEN s."FormType" END DESC,
                           CASE WHEN @sortExpression = 'Level1 ASC' THEN ps."Level1" END ASC,
                           CASE WHEN @sortExpression = 'Level1 DESC' THEN ps."Level1" END DESC,
                           CASE WHEN @sortExpression = 'Level2 ASC' THEN ps."Level2" END ASC,
                           CASE WHEN @sortExpression = 'Level2 DESC' THEN ps."Level2" END DESC,
                           CASE WHEN @sortExpression = 'Level3 ASC' THEN ps."Level3" END ASC,
                           CASE WHEN @sortExpression = 'Level3 DESC' THEN ps."Level3" END DESC,
                           CASE WHEN @sortExpression = 'Level4 ASC' THEN ps."Level4" END ASC,
                           CASE WHEN @sortExpression = 'Level4 DESC' THEN ps."Level4" END DESC,
                           CASE WHEN @sortExpression = 'Level5 ASC' THEN ps."Level5" END ASC,
                           CASE WHEN @sortExpression = 'Level5 DESC' THEN ps."Level5" END DESC,
                           CASE WHEN @sortExpression = 'Number ASC' THEN ps."Number" END ASC,
                           CASE WHEN @sortExpression = 'Number DESC' THEN ps."Number" END DESC,
                           CASE WHEN @sortExpression = 'ObserverName ASC' THEN mo."DisplayName" END ASC,
                           CASE WHEN @sortExpression = 'ObserverName DESC' THEN mo."DisplayName" END DESC,
                           CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers ASC' THEN s."NumberOfFlaggedAnswers" END ASC,
                           CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers DESC' THEN s."NumberOfFlaggedAnswers" END DESC,
                           CASE WHEN @sortExpression = 'NumberOfQuestionsAnswered ASC' THEN s."NumberOfQuestionsAnswered" END ASC,
                           CASE WHEN @sortExpression = 'NumberOfQuestionsAnswered DESC' THEN s."NumberOfQuestionsAnswered" END DESC,
                           CASE WHEN @sortExpression = 'MediaFilesCount ASC' THEN s."MediaFilesCount" END ASC,
                           CASE WHEN @sortExpression = 'MediaFilesCount DESC' THEN s."MediaFilesCount" END DESC,
                           CASE WHEN @sortExpression = 'NotesCount ASC' THEN s."NotesCount" END ASC,
                           CASE WHEN @sortExpression = 'NotesCount DESC' THEN s."NotesCount" END DESC,
                           CASE WHEN @sortExpression = 'MonitoringObserverStatus ASC' THEN mo."Status" END ASC,
                           CASE WHEN @sortExpression = 'MonitoringObserverStatus DESC' THEN mo."Status" END DESC
                  OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            monitoringObserverId = req.MonitoringObserverId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            formType = req.FormTypeFilter?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            followUpStatus = req.FollowUpStatus?.ToString(),
            tagsFilter = req.TagsFilter ?? [],
            monitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
            formId = req.FormId,
            hasNotes = req.HasNotes,
            hasAttachments = req.HasAttachments,
            questionsAnswered = req.QuestionsAnswered?.ToString(),
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),
            dataSource = req.DataSource?.ToString(),
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<FormSubmissionEntry> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<FormSubmissionEntry>().ToList();
        }

        return TypedResults.Ok(
            new PagedResponse<FormSubmissionEntry>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(FormSubmissionEntry.TimeSubmitted)} DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.FormCode)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.FormType),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.FormType)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level1),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level1)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level2),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level2)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level3),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level3)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level4),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level4)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level5),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level5)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Number),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Number)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.ObserverName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Tags),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Tags)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NumberOfQuestionsAnswered),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.NumberOfQuestionsAnswered)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.NumberOfFlaggedAnswers)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.MediaFilesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.MediaFilesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NotesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.NotesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.TimeSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.TimeSubmitted)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.MonitoringObserverStatus),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.MonitoringObserverStatus)} {sortOrder}";
        }

        return $"{nameof(FormSubmissionEntry.TimeSubmitted)} DESC";
    }
}
