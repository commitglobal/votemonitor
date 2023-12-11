namespace Vote.Monitor.Api.Feature.Forms.Update;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
     readonly IRepository<FormAggregate> _repository;

    public Endpoint(IRepository<FormAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        //Put("/api/election-rounds/{electionRoundId}/forms/{id}");
        Put("/api/forms/{id}");
        AllowAnonymous();
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var form = await _repository.GetByIdAsync(req.Id, ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        form.UpdateDetails();
        await _repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
