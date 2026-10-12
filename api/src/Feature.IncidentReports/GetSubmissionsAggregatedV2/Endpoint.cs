using Dapper;
using Module.Answers.Aggregators;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;

namespace Feature.IncidentReports.GetSubmissionsAggregatedV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    IFileStorageService fileStorageService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-reports/forms/{formId}:aggregated-submissionsV2");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Gets aggregated incident report form submissions with all the notes and attachments (v2)";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var filter = new FilterSqlCompiler(V2ReportFilterFields.IncidentReports, parameterPrefixer: FilterSqlCompiler.FunctionParameter)
            .Build(req.Filter);

        const string sql = """
                           SELECT * FROM "GetIncidentReportsAggregate"(@electionRoundId, @ngoId, @dataSource, @formId, @filter, @filterParameters::jsonb)
                           """;

        FormSubmissionsAggregate? aggregate;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            aggregate = await dbConnection.QuerySingleOrDefaultAsync<FormSubmissionsAggregate>(new CommandDefinition(sql, new
            {
                electionRoundId = req.ElectionRoundId,
                ngoId = req.NgoId,
                dataSource = DataSource.Ngo.ToString(),
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
