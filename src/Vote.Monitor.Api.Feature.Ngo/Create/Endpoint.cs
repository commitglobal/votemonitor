using Vote.Monitor.Api.Feature.Ngo.Specifications;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.Ngo.Create;

public class Endpoint(IRepository<NgoAggregate> repository, ITimeProvider timeProvider) :
        Endpoint<Request, Results<Ok<NgoModel>, Conflict<ProblemDetails>>>
{

    public override void Configure()
    {
        Post("/api/ngos");
    }

    public override async Task<Results<Ok<NgoModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetNgoByNameSpecification(req.Name);
        var hasNgoWithSameName = await repository.AnyAsync(specification, ct);

        if (hasNgoWithSameName)
        {
            AddError(r => r.Name, "A Ngo with same name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var ngo = new NgoAggregate(req.Name);
        await repository.AddAsync(ngo, ct);

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
