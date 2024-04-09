using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class FormAggregateFaker : PrivateFaker<Form>
{
    private readonly List<FormStatus> _statuses = [FormStatus.Drafted, FormStatus.Published, FormStatus.Obsolete];
    public FormAggregateFaker(Guid? id = null, FormStatus? status = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.Code, fake => status ?? fake.Random.String(3, 'A', 'Z'));
    }
}
