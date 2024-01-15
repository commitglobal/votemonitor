using AutoBogus;
using AutoBogus.NSubstitute;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.Specifications;

public class PollingStationAggregateFaker : AutoFaker<PollingStationAggregate>
{
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);
    private readonly ITimeService _timeService = Substitute.For<ITimeService>();

    public PollingStationAggregateFaker(Guid? id = null, int? displayOrder = null, string? address = null)
    {
        _timeService.UtcNow.Returns(_baseCreationDate);

        Configure(builder => builder
            .WithBinder(new NSubstituteBinder())
            .WithOverride<ITimeService>(ctx => _timeService));

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
