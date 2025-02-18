using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.TestUtils.Fakes;

namespace Feature.FormTemplates.UnitTests;

public sealed class FormTemplateSlimModelFaker : PrivateFaker<FormTemplateSlimModel>
{
    private readonly List<FormStatus> _statuses = [FormStatus.Drafted, FormStatus.Published];
    public FormTemplateSlimModelFaker(Guid? id = null, FormStatus? status = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.Code, fake => status ?? fake.Random.String(3, 'A', 'Z'));
    }
}
