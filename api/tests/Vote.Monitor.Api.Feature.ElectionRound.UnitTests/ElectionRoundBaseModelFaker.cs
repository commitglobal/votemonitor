using Bogus;

namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests;

public sealed class ElectionRoundBaseModelFaker : Faker<ElectionRoundModel>
{
    private readonly ElectionRoundStatus[] _statuses =
        [ElectionRoundStatus.NotStarted, ElectionRoundStatus.Started, ElectionRoundStatus.Archived];

    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);
    private readonly DateTime _baseModifiedDate = new(2024, 01, 02, 00, 00, 00, DateTimeKind.Utc);

    public ElectionRoundBaseModelFaker(
        Guid? id = null,
        int? index = null,
        Guid? countryId = null,
        string? title = null,
        string? englishTitle = null,
        DateOnly? startDate = null,
        ElectionRoundStatus? status = null)
    {
        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Title, fake => title ?? fake.Company.CompanyName());
        RuleFor(fake => fake.EnglishTitle, fake => englishTitle ?? fake.Company.CompanyName());
        RuleFor(fake => fake.StartDate, fake => startDate ?? fake.Date.FutureDateOnly());
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
        RuleFor(fake => fake.LastModifiedOn, _baseModifiedDate.AddHours(index ?? 0));
        
        var country = FakerHub.PickRandom(CountriesList.GetAll());
        RuleFor(fake => fake.CountryId, country.Id);
        RuleFor(fake => fake.CountryIso2, country.Iso2);
        RuleFor(fake => fake.CountryIso3, country.Iso3);
        RuleFor(fake => fake.CountryName, country.Name);
        RuleFor(fake => fake.CountryFullName, country.FullName);
        RuleFor(fake => fake.CountryNumericCode, country.NumericCode);
    }
}