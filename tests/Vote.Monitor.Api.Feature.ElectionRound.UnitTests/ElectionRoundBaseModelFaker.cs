using Bogus;

namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests;

public class ElectionRoundBaseModelFaker: Faker<ElectionRoundBaseModel>
{
    private readonly ElectionRoundStatus[] _statuses = [ElectionRoundStatus.NotStarted, ElectionRoundStatus.Started, ElectionRoundStatus.Archived];
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
        RuleFor(fake => fake.CountryId, fake => countryId ?? fake.PickRandom(CountriesList.GetAll()).Id);
        RuleFor(fake => fake.Country, fake => fake.PickRandom(CountriesList.GetAll()).Name);
        RuleFor(fake => fake.Title, fake => title ?? fake.Company.CompanyName());
        RuleFor(fake => fake.EnglishTitle, fake => englishTitle ?? fake.Company.CompanyName());
        RuleFor(fake => fake.StartDate, fake => startDate ?? fake.Date.FutureDateOnly());
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
        RuleFor(fake => fake.LastModifiedOn, _baseModifiedDate.AddHours(index ?? 0));
    }
}
