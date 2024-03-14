using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Form.Publish;

public class Endpoint(IRepository<FormAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgo}/forms/{id}:publish");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        var result = formTemplate.Publish();

        if (result is PublishResult.InvalidFormTemplate validationResult)
        {
            validationResult.Problems.Errors.ForEach(AddError);
            return new ProblemDetails(ValidationFailures);
        }

        await repository.UpdateAsync(formTemplate, ct);
        return TypedResults.NoContent();
    }
}
