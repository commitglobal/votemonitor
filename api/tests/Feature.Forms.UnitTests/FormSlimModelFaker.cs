using Bogus;
using Vote.Monitor.Core.Constants;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.UnitTests;

public sealed class FormSlimModelFaker: Faker<FormSlimModel>
{
    private readonly IReadOnlyCollection<FormStatus> _statuses = FormStatus.List;
    private readonly List<string> _languages = LanguagesList.GetAll().Select(x=>x.Iso1).ToList();
    public FormSlimModelFaker(Guid? id = null, FormStatus? status = null)
    {
        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom<FormStatus>(_statuses));
        RuleFor(fake => fake.Code, fake => status ?? fake.Random.String(3, 'A', 'Z'));
        RuleFor(fake => fake.CreatedOn, f => f.Date.Recent());
        RuleFor(fake => fake.LastModifiedOn, f => f.Date.Recent());
        RuleFor(fake => fake.Name, new TranslatedStringFaker().Generate());
        RuleFor(fake => fake.Languages, f => f.PickRandom(_languages, 3).ToArray());
    }
}
