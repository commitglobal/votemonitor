using Vote.Monitor.Api.Feature.Form.Specifications;
using Vote.Monitor.Form.Module.Mappers;

namespace Vote.Monitor.Api.Feature.Form.Update;

public class Endpoint(IRepository<FormAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgo}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetFormSpecification(req.Id, req.Code, req.FormType);
        var duplicatedFormTemplate = await repository.AnyAsync(specification, ct);

        if (duplicatedFormTemplate)
        {
            AddError(r => r.Name, "A form template with same parameters already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var questions = req.Questions
                 .Select(FormMapper.ToEntity)
                 .ToList()
                 .AsReadOnly();

        formTemplate.UpdateDetails(req.Code, req.Name, req.FormType, req.Languages, questions);

        await repository.UpdateAsync(formTemplate, ct);
        return TypedResults.NoContent();
    }
}
