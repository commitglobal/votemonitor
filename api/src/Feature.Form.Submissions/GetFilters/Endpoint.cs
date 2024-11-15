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
                    SELECT 
                      MIN(
                        COALESCE(
                          FS."LastModifiedOn", FS."CreatedOn"
                        )
                      ) AS "FirstSubmissionTimestamp", 
                      MAX(
                        COALESCE(
                          FS."LastModifiedOn", FS."CreatedOn"
                        )
                      ) AS "LastSubmissionTimestamp" 
                    FROM 
                      "FormSubmissions" FS 
                      INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId" 
                      inner join "AvailableForms" af on fs."FormId" = af."Id" 
                    WHERE 
                      FS."ElectionRoundId" = @electionRoundId 
                    UNION ALL 
                      -- Second subquery for PollingStationInformation
                    SELECT 
                      MIN(
                        COALESCE(
                          PSI."LastModifiedOn", PSI."CreatedOn"
                        )
                      ) AS "FirstSubmissionTimestamp", 
                      MAX(
                        COALESCE(
                          PSI."LastModifiedOn", PSI."CreatedOn"
                        )
                      ) AS "LastSubmissionTimestamp" 
                    FROM 
                      "PollingStationInformation" PSI 
                      INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO ON MO."MonitoringObserverId" = PSI."MonitoringObserverId" 
                    WHERE 
                      PSI."ElectionRoundId" = @electionRoundId
                  ) -- Final query to get the overall min and max
                  SELECT 
                    MIN("FirstSubmissionTimestamp") AS "FirstSubmissionTimestamp", 
                    MAX("LastSubmissionTimestamp") AS "LastSubmissionTimestamp" 
                  FROM 
                    "CombinedTimestamps";
                  -- =====================================================================================
                  SELECT 
                    DISTINCT F."Id" AS "FormId", 
                    F."Name" ->> F."DefaultLanguage" "FormName", 
                    F."Code" "FormCode" 
                  FROM 
                    "FormSubmissions" FS 
                    INNER JOIN "Forms" F ON F."Id" = FS."FormId" 
                    INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) MO ON MO."MonitoringObserverId" = FS."MonitoringObserverId" 
                    inner join "GetAvailableForms"(@electionRoundId, @ngoId, @dataSource) af on fs."FormId" = af."FormId" 
                  WHERE 
                    FS."ElectionRoundId" = @electionRoundId 
                  UNION ALL 
                  SELECT 
                    DISTINCT F."Id" AS "FormId", 
                    F."Name" ->> F."DefaultLanguage" "FormName", 
                    F."Code" "FormCode" 
                  FROM 
                    "PollingStationInformation" PSI 
                    INNER JOIN "PollingStationInformationForms" F ON F."Id" = PSI."PollingStationInformationFormId" 
                    inner join "GetAvailableForms"(@electionRoundId, @ngoId, @dataSource) af on PSI."FormId" = af."FormId" 
                  WHERE 
                    PSI."ElectionRoundId" = @electionRoundId
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString()
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
