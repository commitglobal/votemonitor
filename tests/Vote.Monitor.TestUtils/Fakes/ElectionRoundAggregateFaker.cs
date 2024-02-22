using Vote.Monitor.Domain.Entities.CountryAggregate;

namespace Vote.Monitor.TestUtils.Fakes;

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
        ElectionRoundStatus? status = null)
    {
        UsePrivateConstructor();

        Country country = countryId.HasValue
            ? CountriesList.Get(countryId.Value)!.ToEntity()
            : new Country("Test country", "Fake test country", "FC", "FTC", "999", new CurrentUtcTimeProvider());

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.CountryId, country.Id);
        RuleFor(fake => fake.Country, country);
        RuleFor(fake => fake.Title, fake => title ?? fake.Company.CompanyName());
        RuleFor(fake => fake.EnglishTitle, fake => englishTitle ?? fake.Company.CompanyName());
        RuleFor(fake => fake.StartDate, fake => startDate ?? fake.Date.FutureDateOnly());
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
        RuleFor(fake => fake.LastModifiedOn, _baseModifiedDate.AddHours(index ?? 0));
        RuleFor(fake => fake.CreatedBy, fake => fake.Random.Guid());
        RuleFor(fake => fake.LastModifiedBy, fake => fake.Random.Guid());
    }
}
