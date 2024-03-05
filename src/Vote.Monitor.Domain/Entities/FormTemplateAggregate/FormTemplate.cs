using Vote.Monitor.Domain.Entities.LanguageAggregate;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public class FormTemplate : AuditableBaseEntity, IAggregateRoot
{
    public FormType FormType { get; private set; }
    public string Code { get; private set; }
    public TranslatedString Name { get; private set; }
    public FormTemplateStatus Status { get; private set; }

    public IReadOnlyCollection<Language> Languages { get; private set; }

    private readonly List<FormSection> _sections = new();
    public IReadOnlyList<FormSection> Sections => _sections.AsReadOnly();

    private FormTemplate(FormType formType, string code, TranslatedString name, List<Language> languages, ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        FormType = formType;
        Code = code;
        Name = name;
        Languages = languages;
        Status = FormTemplateStatus.Drafted;
    }

    public static FormTemplate Create(FormType formType, string code, TranslatedString name, List<Language> languages, ITimeProvider timeProvider) =>
        new(formType, code, name, languages, timeProvider);

    public FormSection AddFormSection(string code, TranslatedString title)
    {
        var formSection = FormSection.Create(code, title);
        _sections.Add(formSection);
        return formSection;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    private FormTemplate()
    {

    }

#pragma warning restore CS8618

    public void Publish()
    {
        Status = FormTemplateStatus.Published;
    }

    public void Draft()
    {
        Status = FormTemplateStatus.Drafted;
    }

    public void UpdateDetails(string code, TranslatedString name, FormType formType, List<Language> languages)
    {
        Code = code;
        Name = name;
        FormType = formType;
        Languages = languages.ToList().AsReadOnly();
    }

    public void ClearSections()
    {
        _sections.Clear();
    }
}
