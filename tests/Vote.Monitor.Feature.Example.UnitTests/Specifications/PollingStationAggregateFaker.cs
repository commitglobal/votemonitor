using AutoBogus;
using Vote.Monitor.Feature.PollingStation.Helpers;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.Specifications;

public class PollingStationAggregateFaker : AutoFaker<PollingStationAggregate>
{
    public PollingStationAggregateFaker(Guid? id = null, int? displayOrder = null, string? address = null)
    {
        RuleFor(fake => fake.Id, id ?? Guid.NewGuid());
        RuleFor(fake => fake.DisplayOrder, fake => displayOrder ?? fake.Random.Int(0, 1000));
        RuleFor(fake => fake.Address, fake => address ?? fake.Address.FullAddress());
        RuleFor(fake => fake.Tags, fake => fake.Commerce.Categories(4).Select((x, i) => new TagImportModel
        {
            Name = $"Category {i}",
            Value = x
        }).ToTagsObject());
    }

}
