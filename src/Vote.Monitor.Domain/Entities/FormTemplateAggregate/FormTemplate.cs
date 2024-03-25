using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public class FormTemplate : AuditableBaseEntity, IAggregateRoot
{
    public FormTemplateType FormTemplateType { get; private set; }
    public string Code { get; private set; }
    public TranslatedString Name { get; private set; }
    public FormTemplateStatus Status { get; private set; }

    public IReadOnlyList<string> Languages { get; private set; } = new List<string>().AsReadOnly();

    public IReadOnlyList<FormSection> Sections { get; private set; } = new List<FormSection>().AsReadOnly();

    private FormTemplate(FormTemplateType formTemplateType,
        string code,
        TranslatedString name,
        IEnumerable<string> languages) : base(Guid.NewGuid())
    {
        FormTemplateType = formTemplateType;
        Code = code;
        Name = name;
        Languages = languages.ToList().AsReadOnly();
        Status = FormTemplateStatus.Drafted;
    }

    public static FormTemplate Create(FormTemplateType formTemplateType,
        string code,
        TranslatedString name,
        IEnumerable<string> languages) =>
        new(formTemplateType, code, name, languages);

    public PublishResult Publish()
    {
        var validator = new FormTemplateValidator();
        var validationResult = validator.Validate(this);

        if (!validationResult.IsValid)
        {
            return new PublishResult.InvalidFormTemplate(validationResult);
        }

        Status = FormTemplateStatus.Published;

        return new PublishResult.Published();
    }

    public void Draft()
    {
        Status = FormTemplateStatus.Drafted;
    }

    public void UpdateDetails(string code, TranslatedString name,
        FormTemplateType formTemplateType,
        IEnumerable<string> languages,
        IEnumerable<FormSection> sections)
    {
        Code = code;
        Name = name;
        FormTemplateType = formTemplateType;
        Languages = languages.ToList().AsReadOnly();
        Sections = sections.ToList().AsReadOnly();
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private FormTemplate()
    {

    }
#pragma warning restore CS8618
}
