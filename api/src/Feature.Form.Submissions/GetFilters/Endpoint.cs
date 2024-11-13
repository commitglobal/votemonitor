using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.Form.Submissions.GetFilters;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:filters");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s => { s.Summary = "Filter options for submissions."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  WITH "CombinedTimestamps" AS (
                      -- First subquery for FormSubmissions
                      SELECT MIN(COALESCE(FS."LastModifiedOn", FS."CreatedOn")) AS "FirstSubmissionTimestamp",
                             MAX(COALESCE(FS."LastModifiedOn", FS."CreatedOn")) AS "LastSubmissionTimestamp"
                      FROM "FormSubmissions" FS
                               INNER JOIN "MonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                               INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                      WHERE FS."ElectionRoundId" = @electionRoundId
                        AND MN."NgoId" = @ngoId
                      UNION ALL
                      -- Second subquery for PollingStationInformation
                      SELECT MIN(COALESCE(PSI."LastModifiedOn", PSI."CreatedOn")) AS "FirstSubmissionTimestamp",
                             MAX(COALESCE(PSI."LastModifiedOn", PSI."CreatedOn")) AS "LastSubmissionTimestamp"
                      FROM "PollingStationInformation" PSI
                               INNER JOIN "MonitoringObservers" MO ON MO."Id" = PSI."MonitoringObserverId"
                               INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                      WHERE PSI."ElectionRoundId" = @electionRoundId
                        AND MN."NgoId" = @ngoId)
                        
                  -- Final query to get the overall min and max
                  SELECT MIN("FirstSubmissionTimestamp") AS "FirstSubmissionTimestamp",
                         MAX("LastSubmissionTimestamp")  AS "LastSubmissionTimestamp"
                  FROM "CombinedTimestamps";

                  -- =====================================================================================

                  WITH
                  	"MonitoringNgoDetails" AS (
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
                  			) AS "IsCoalitionLeader"
                  		FROM
                  			"MonitoringNgos" MN
                  		WHERE
                  			MN."ElectionRoundId" = @electionRoundId
                  			AND MN."NgoId" = @ngoId
                  		LIMIT
                  			1
                  	),
                  	-- if ngo is coalition leader they need to see all the responses
                  	"AvailableMonitoringObservers" AS (
                  		SELECT
                  			MO."Id",
                  			MO."MonitoringNgoId",
                  			U."DisplayName"
                  		FROM
                  			"Coalitions" C
                  			INNER JOIN "CoalitionMemberships" CM ON C."Id" = CM."CoalitionId"
                  			INNER JOIN "MonitoringObservers" MO ON MO."MonitoringNgoId" = CM."MonitoringNgoId"
                  			INNER JOIN "MonitoringNgos" MN ON MN."Id" = CM."MonitoringNgoId"
                  			INNER JOIN "MonitoringNgoDetails" MND ON MND."MonitoringNgoId" = MN."Id"
                  			INNER JOIN "AspNetUsers" U ON U."Id" = MO."ObserverId"
                  		WHERE
                  			CM."ElectionRoundId" = MN."ElectionRoundId"
                  			AND (
                  				MND."IsCoalitionLeader"
                  				OR MN."NgoId" = @ngoId
                  			)
                  	)
                  SELECT DISTINCT
                  	F."Id" AS "FormId",
                  	F."Name" ->> F."DefaultLanguage" "FormName",
                  	F."Code" "FormCode"
                  FROM
                  	"FormSubmissions" FS
                  	INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                  	INNER JOIN "AvailableMonitoringObservers" MO ON MO."Id" = FS."MonitoringObserverId"
                  WHERE
                  	FS."ElectionRoundId" = @electionRoundId
                  UNION ALL
                  SELECT DISTINCT
                  	F."Id" AS "FormId",
                  	F."Name" ->> F."DefaultLanguage" "FormName",
                  	F."Code" "FormCode"
                  FROM
                  	"PollingStationInformation" PSI
                  	INNER JOIN "PollingStationInformationForms" F ON F."Id" = PSI."PollingStationInformationFormId"
                  	INNER JOIN "AvailableMonitoringObservers" MO ON MO."Id" = PSI."MonitoringObserverId"
                  	INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                  WHERE
                  	PSI."ElectionRoundId" = @electionRoundId
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId
        };

        SubmissionsTimestampsFilterOptions timestampFilterOptions;
        List<SubmissionsFormFilterOption> formFilterOptions;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

            timestampFilterOptions = multi.Read<SubmissionsTimestampsFilterOptions>().Single();
            formFilterOptions = multi.Read<SubmissionsFormFilterOption>().ToList();
        }

        return TypedResults.Ok(new Response
        {
            TimestampsFilterOptions = timestampFilterOptions,
            FormFilterOptions = formFilterOptions
        });
    }
}
