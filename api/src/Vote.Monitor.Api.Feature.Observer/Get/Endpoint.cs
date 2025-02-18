namespace Vote.Monitor.Api.Feature.Observer.Get;

public class Endpoint(IReadRepository<ObserverAggregate> repository)
    : Endpoint<Request, Results<Ok<ObserverModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/observers/{id}");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<ObserverModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetObserverByIdSpecification(req.Id);
        var observer = await repository.SingleOrDefaultAsync(specification, ct);

        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new ObserverModel
        {
            Id = observer.Id,
            Email = observer.ApplicationUser.Email!,
            FirstName = observer.ApplicationUser.FirstName,
            LastName = observer.ApplicationUser.LastName,
            PhoneNumber = observer.ApplicationUser.PhoneNumber,
            Status = observer.ApplicationUser.Status,
            CreatedOn = observer.CreatedOn,
            LastModifiedOn = observer.LastModifiedOn
        });
    }
}
