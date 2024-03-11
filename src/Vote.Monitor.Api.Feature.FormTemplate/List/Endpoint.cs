using Vote.Monitor.Api.Feature.FormTemplate.Models;
using Vote.Monitor.Api.Feature.FormTemplate.Specifications;

namespace Vote.Monitor.Api.Feature.FormTemplate.List;

public class Endpoint(IReadRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<Ok<PagedResponse<AttachmentModel>>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/form-templates");
    }

    public override async Task<Results<Ok<PagedResponse<AttachmentModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListFormTemplatesSpecification(req);
        var formTemplates = await repository.ListAsync(specification, ct);
        var formTemplateCount = await repository.CountAsync(specification, ct);

        return TypedResults.Ok(new PagedResponse<AttachmentModel>(formTemplates, formTemplateCount, req.PageNumber, req.PageSize));
    }
}
