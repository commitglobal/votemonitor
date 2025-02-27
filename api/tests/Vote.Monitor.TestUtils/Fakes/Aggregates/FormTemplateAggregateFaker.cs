using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class FormTemplateAggregateFaker : PrivateFaker<FormTemplate>
{
    private readonly List<FormStatus> _statuses = [FormStatus.Drafted, FormStatus.Published];
    private readonly DateTime _baseCreationDate = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);
    private readonly DateTime _baseModifiedDate = new(2024, 01, 02, 00, 00, 00, DateTimeKind.Utc);

    public FormTemplateAggregateFaker(Guid? id = null,
        string? code = null,
        TranslatedString? name = null,
        FormStatus? status = null,
        List<LanguageDetails>? languages = null,
        int? index = null)
    {
        UsePrivateConstructor();


        var formLanguages = languages?.Select(x => x.Iso1).ToArray() ??
                            FakerHub.PickRandom(LanguagesList.GetAll(), 3).Select(x => x.Iso1).ToArray();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Code, code ?? Guid.NewGuid().ToString());
        RuleFor(fake => fake.Languages, formLanguages);
        RuleFor(fake => fake.DefaultLanguage, fake => fake.PickRandom(formLanguages));
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
        RuleFor(fake => fake.Name, name ?? new TranslatedString());

        RuleFor(fake => fake.CreatedOn, _baseCreationDate.AddHours(index ?? 0));
        RuleFor(fake => fake.LastModifiedOn, _baseModifiedDate.AddHours(index ?? 0));
    }
}
