using Vote.Monitor.Api.Feature.Forms.Models;

namespace Vote.Monitor.Api.Feature.Forms.Get;

public class Endpoint : Endpoint<Request, Results<Ok<FormModel>, NotFound>>
{
     readonly IReadRepository<FormAggregate> _repository;

    public Endpoint(IReadRepository<FormAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        //Get("/api/election-rounds/{electionRoundId}/forms/{id}");
        Get("/api/forms/{id}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<FormModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var form = await _repository.GetByIdAsync(req.Id, ct);
        if (form is null)
        {
            return TypedResults.NotFound();
        }

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
