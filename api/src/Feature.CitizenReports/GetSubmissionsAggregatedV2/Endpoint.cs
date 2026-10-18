using Dapper;
using Module.Answers.Aggregators;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;

namespace Feature.CitizenReports.GetSubmissionsAggregatedV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    IFileStorageService fileStorageService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports/forms/{formId}:aggregated-submissionsV2");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Gets aggregated citizen report form submissions with all the notes and attachments (v2)";
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
            .Build(req.ToFilterRule());

        const string sql = """
                           SELECT * FROM "GetCitizenReportsAggregate"(@electionRoundId, @ngoId, @formId, @filter, @filterParameters::jsonb)
                           """;

        CitizenReportFormSubmissionsAggregateV2? aggregate;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            aggregate = await dbConnection.QuerySingleOrDefaultAsync<CitizenReportFormSubmissionsAggregateV2>(new CommandDefinition(sql, new
            {
                electionRoundId = req.ElectionRoundId,
                ngoId = req.NgoId,
                formId = req.FormId,
                filter = filter.Sql,
                filterParameters = filter.ToJsonParameters()
            }, cancellationToken: ct));
        }

        if (aggregate is null)
        {
            return TypedResults.NotFound();
        }

        aggregate.Attachments = await fileStorageService.PresignAsync(aggregate.Attachments);

        return TypedResults.Ok(new Response { SubmissionsAggregate = aggregate });
    }
}
