using Vote.Monitor.Api.Feature.Form.Specifications;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.Form.List;

public class Endpoint(IReadRepository<FormAggregate> repository) : Endpoint<Request, Results<Ok<PagedResponse<FormModel>>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgo}/forms");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<Ok<PagedResponse<FormModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListFormsSpecification(req);
        var formTemplates = await repository.ListAsync(specification, ct);
        var formTemplateCount = await repository.CountAsync(specification, ct);

        return TypedResults.Ok(new PagedResponse<FormModel>(formTemplates, formTemplateCount, req.PageNumber, req.PageSize));
    }
}
