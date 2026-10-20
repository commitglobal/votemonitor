using Dapper;
using Module.Answers.Aggregators;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;

namespace Feature.CitizenReports.GetQuestionAggregatedByLevel;

public class Endpoint(
    IAuthorizationService authorizationService,
    IFileStorageService fileStorageService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports/forms/{formId}/questions/{questionId}:aggregatedByLevel");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Gets the aggregated answers to a citizen report question, broken down by the locations of a level (Level1..Level5)";
            s.Description = "Returns 404 when the form does not belong to the ngo or does not contain the question.";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new CitizenReportingNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var filter = new FilterSqlCompiler(V2ReportFilterFields.CitizenReports, parameterPrefixer: FilterSqlCompiler.FunctionParameter)
            .Build(req.Filter);

        const string sql = """
                           SELECT * FROM "GetCitizenReportsQuestionAggregateByLevel"(@electionRoundId, @ngoId, @formId, @questionId, @level, @filter, @filterParameters::jsonb)
                           """;

        QuestionAggregateByLevel? aggregate;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            aggregate = await dbConnection.QuerySingleOrDefaultAsync<QuestionAggregateByLevel>(new CommandDefinition(sql, new
            {
                electionRoundId = req.ElectionRoundId,
                ngoId = req.NgoId,
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
