using Feature.FormTemplates.Specifications;
using Vote.Monitor.Core.Models;

namespace Feature.FormTemplates.List;

public class Endpoint(IReadRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<Ok<PagedResponse<FormTemplateSlimModel>>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/form-templates");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<FormTemplateSlimModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListFormTemplatesSpecification(req);
        var formTemplates = await repository.ListAsync(specification, ct);
        var formTemplateCount = await repository.CountAsync(specification, ct);

        return TypedResults.Ok(new PagedResponse<FormTemplateSlimModel>(formTemplates, formTemplateCount, req.PageNumber, req.PageSize));
    }
}
