﻿namespace Feature.Form.Submissions.Any;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory connectionFactory) : Endpoint<Request, Results<Ok, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:any");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s =>
        {
            s.Summary = "Returns 200 if there are submissions for this polling station 404 if not";
        });
        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<Ok, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        bool hasSubmissionsForPollingStation;
        using (var dbConnection = await connectionFactory.GetOpenConnectionAsync(ct))
        {
            var sql = """
                      SELECT
                      EXISTS (
                        SELECT
                            1
                        FROM
                            "FormSubmissions" FS
                            INNER JOIN "MonitoringObservers" MO ON FS."MonitoringObserverId" = MO."Id"
                            INNER JOIN "Forms" f ON FS."FormId" = f."Id"
                        WHERE
                            FS."ElectionRoundId" = @electionRoundId
                            AND MO."ObserverId" = @observerId
                            AND FS."PollingStationId" = @pollingStationId
                            AND F."Status" = 'Published'
                            AND JSONB_ARRAY_LENGTH(FS."Answers") > 0
                      )
                      """;

            var queryParams = new
            {
                electionRoundId = req.ElectionRoundId,
                observerId = req.ObserverId,
                pollingStationId = req.PollingStationId
            };

            hasSubmissionsForPollingStation = await dbConnection.QuerySingleAsync<bool>(sql, queryParams);
        }

        return hasSubmissionsForPollingStation ? TypedResults.Ok() : TypedResults.NotFound();
    }
}
