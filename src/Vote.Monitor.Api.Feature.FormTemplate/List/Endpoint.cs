using Vote.Monitor.Api.Feature.FormTemplate.Specifications;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.FormTemplate.List;

public class Endpoint(IReadRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<Ok<PagedResponse<FormTemplateModel>>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/form-templates");
    }

    public override async Task<Results<Ok<PagedResponse<FormTemplateModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListFormTemplatesSpecification(req);
        var formTemplates = await repository.ListAsync(specification, ct);
        var formTemplateCount = await repository.CountAsync(specification, ct);

        return TypedResults.Ok(new PagedResponse<FormTemplateModel>(formTemplates, formTemplateCount, req.PageNumber, req.PageSize));
    }
}
