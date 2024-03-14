using Vote.Monitor.Form.Module.Mappers;
using Vote.Monitor.Form.Module.Models;

namespace Vote.Monitor.Api.Feature.FormTemplate.Get;

public class Endpoint(IReadRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<Ok<FormTemplateModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/form-templates/{id}");
    }

    public override async Task<Results<Ok<FormTemplateModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new FormTemplateModel
        {
            Id = formTemplate.Id,
            Code = formTemplate.Code,
            Name = formTemplate.Name,
            Status = formTemplate.Status,
            CreatedOn = formTemplate.CreatedOn,
            LastModifiedOn = formTemplate.LastModifiedOn,
            Languages = formTemplate.Languages.ToList(),
            Sections = formTemplate.Sections.Select(section => new SectionModel
            {
                Id = section.Id,
                Code = section.Code,
                Title = section.Title,
                Questions = section.Questions.Select(FormMapper.ToModel).ToList()
            }).ToList()
        });
    }
}
