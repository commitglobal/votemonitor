using Module.Answers.Aggregators;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.Form.Submissions.GetQuestionAggregatedByLevel;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory connectionFactory,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-submissions/{formId}/questions/{questionId}:aggregatedByLevel");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets the aggregated answers to a question, broken down by the locations of a level (Level1..Level5)";
            s.Description = "Returns 404 when the form is not available to the ngo or does not contain the question.";
        });
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var filter = new FilterSqlCompiler(SubmissionFilterFields.All, parameterPrefixer: FilterSqlCompiler.FunctionParameter)
            .Build(req.Filter);

        const string sql = """
                           SELECT * FROM "GetFormSubmissionsQuestionAggregateByLevel"(@electionRoundId, @ngoId, @dataSource, @formId, @questionId, @level, @filter, @filterParameters::jsonb)
                           """;

        QuestionAggregateByLevel? aggregate;
        using (var dbConnection = await connectionFactory.GetOpenConnectionAsync(ct))
        {
            aggregate = await dbConnection.QuerySingleOrDefaultAsync<QuestionAggregateByLevel>(new CommandDefinition(sql, new
            {
                electionRoundId = req.ElectionRoundId,
                ngoId = req.NgoId,
                dataSource = req.DataSource.ToString(),
                formId = req.FormId,
                questionId = req.QuestionId,
                level = req.Level.Depth,
                filter = filter.Sql,
                filterParameters = filter.ToJsonParameters()
            }, cancellationToken: ct));
        }

        if (aggregate is null)
        {
            return TypedResults.NotFound();
        }

        aggregate.Attachments = await fileStorageService.PresignAsync(aggregate.Attachments);

        return TypedResults.Ok(new Response { QuestionAggregate = aggregate });
    }
}
