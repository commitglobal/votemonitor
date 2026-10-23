using Feature.Form.Submissions.Requests;

namespace Feature.Form.Submissions.ListByForm;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<FormSubmissionsAggregateFilter, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byForm");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminOrStaff);

        Summary(x => { x.Summary = "Form submissions aggregated by form"; });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(FormSubmissionsAggregateFilter req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql =
            """
            SELECT *
            FROM "GetFormSubmissionsByForm"(@electionRoundId, @ngoId, @dataSource, @coalitionMemberId,
                                            @level1, @level2, @level3, @level4, @level5, @pollingStationNumber,
                                            @hasFlaggedAnswers, @followUpStatus, @tagsFilter, @monitoringObserverStatus,
                                            @formId, @hasNotes, @hasAttachments, @questionsAnswered);
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString(),
            coalitionMemberId= req.CoalitionMemberId,
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
        };

        IEnumerable<AggregatedFormOverview> aggregatedFormOverviews;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            aggregatedFormOverviews = await dbConnection.QueryAsync<AggregatedFormOverview>(sql, queryArgs);
        }

        return TypedResults.Ok(new Response { AggregatedForms = aggregatedFormOverviews.ToList() });
    }
}
