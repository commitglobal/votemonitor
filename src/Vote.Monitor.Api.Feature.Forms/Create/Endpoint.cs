using Vote.Monitor.Api.Feature.Forms.Models;
using Vote.Monitor.Api.Feature.Forms.Specifications;

namespace Vote.Monitor.Api.Feature.Forms.Create;

public class Endpoint : Endpoint<Request, Results<Ok<FormModel>, Conflict<ProblemDetails>>>
{
    readonly IRepository<FormAggregate> _repository;

    public Endpoint(IRepository<FormAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        //Post("/api/election-rounds/{electionRoundId}/forms");
        Post("/api/forms");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<FormModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetFormByCodeAndLanguage(req.Code, req.LanguageId);
        var hasFormWithSameCodes = await _repository.AnyAsync(specification, ct);

        if (hasFormWithSameCodes)
        {
            AddError(r => r.Code, "A form with the same combination of form code and language code already exists.");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var form = new FormAggregate(req.Code, req.LanguageId, req.Description);
        await _repository.AddAsync(form, ct);

        return TypedResults.Ok(new FormModel
        {
            Id = form.Id,
            Code = form.Code,
            LanguageId = form.LanguageId,
            Description = form.Description,
            Status = form.Status,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt,
            Questions = new List<BaseQuestionModel>() { }
        });
    }
}
