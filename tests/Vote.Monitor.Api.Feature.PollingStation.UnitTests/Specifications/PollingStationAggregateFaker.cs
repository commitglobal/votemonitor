namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.Specifications;

public class PollingStationAggregateFaker : PrivateFaker<PollingStationAggregate>
{
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);

    public PollingStationAggregateFaker(Guid? id = null, int? displayOrder = null, string? address = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, id ?? Guid.NewGuid());
        RuleFor(fake => fake.DisplayOrder, fake => displayOrder ?? fake.Random.Int(0, 1000));
        RuleFor(fake => fake.Address, fake => address ?? fake.Address.FullAddress());
        RuleFor(fake => fake.Tags, fake => fake.Commerce.Categories(4).Select((x, i) => new TagImportModel
        {
            Name = $"Category {i}",
            Value = x
        }).ToTagsObject());
        RuleFor(fake => fake.CreatedOn, _baseCreationDate);
    }

}
