using Vote.Monitor.Form.Module.Mappers;

namespace Vote.Monitor.Api.Feature.FormTemplate.Get;

public class Endpoint(IReadRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<Ok<FormTemplateFullModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/form-templates/{id}");
    }

    public override async Task<Results<Ok<FormTemplateFullModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new FormTemplateFullModel
        {
            Id = formTemplate.Id,
            Code = formTemplate.Code,
            DefaultLanguage = formTemplate.DefaultLanguage,
            Name = formTemplate.Name,
            Status = formTemplate.Status,
            CreatedOn = formTemplate.CreatedOn,
            LastModifiedOn = formTemplate.LastModifiedOn,
            Languages = formTemplate.Languages.ToList(),
            Questions = formTemplate.Questions.Select(FormMapper.ToModel).ToList()
        });
    }
}
