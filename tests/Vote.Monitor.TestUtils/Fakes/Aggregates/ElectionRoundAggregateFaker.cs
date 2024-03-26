using Vote.Monitor.Domain.Entities.CountryAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class ElectionRoundAggregateFaker : PrivateFaker<ElectionRoundAggregate>
{
    private readonly ElectionRoundStatus[] _statuses = [ElectionRoundStatus.NotStarted, ElectionRoundStatus.Started, ElectionRoundStatus.Archived];
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);
    private readonly DateTime _baseModifiedDate = new(2024, 01, 02, 00, 00, 00, DateTimeKind.Utc);

    public ElectionRoundAggregateFaker(
        Guid? id = null,
        int? index = null,
        Guid? countryId = null,
        string? title = null,
        string? englishTitle = null,
        DateOnly? startDate = null,
        ElectionRoundStatus? status = null,
        List<MonitoringNgo>? monitoringNgos = null)
    {
        UsePrivateConstructor();

        var currentUtcTimeProvider = new CurrentUtcTimeProvider();

        var country = countryId.HasValue
            ? CountriesList.Get(countryId.Value)!.ToEntity()
            : new Country("Test country", "Fake test country", "FC", "FTC", "999");

        RuleFor(f => f.Status, f => status ?? f.PickRandom(_statuses));
        RuleFor(f => f.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
        RuleFor(f => f.LastModifiedOn, _baseModifiedDate.AddHours(index ?? 0));
        RuleFor(f => f.CreatedBy, f => f.Random.Guid());
        RuleFor(f => f.LastModifiedBy, f => f.Random.Guid());

        CustomInstantiator(f => new ElectionRoundAggregate(id ?? f.Random.Guid(),
            title: title ?? f.Company.CompanyName(),
            englishTitle: englishTitle ?? f.Company.CompanyName(),
            startDate: startDate ?? f.Date.FutureDateOnly(),
            country: country,
            monitoringNgos: monitoringNgos ?? []));
    }
}
