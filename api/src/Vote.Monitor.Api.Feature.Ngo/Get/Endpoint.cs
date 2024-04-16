namespace Vote.Monitor.Api.Feature.Ngo.Get;

public class Endpoint(IReadRepository<NgoAggregate> repository) : Endpoint<Request, Results<Ok<NgoModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/ngos/{id}");
    }

    public override async Task<Results<Ok<NgoModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var ngo = await repository.GetByIdAsync(req.Id, ct);

        if (ngo is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new NgoModel
        {
            Id = ngo.Id,
            Name = ngo.Name,
            Status = ngo.Status,
            CreatedOn = ngo.CreatedOn,
            LastModifiedOn = ngo.LastModifiedOn
        });
    }
}
