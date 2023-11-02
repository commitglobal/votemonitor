namespace Vote.Monitor.Country.Export;

public class Endpoint : EndpointWithoutRequest<Results<Ok<CountryModel>, NotFound>>
{
     readonly IReadRepository<Domain.Entities.CountryAggregate.Country> _repository;

    public Endpoint(IReadRepository<Domain.Entities.CountryAggregate.Country> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/countries:export");
    }

    public override async Task<Results<Ok<CountryModel>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
