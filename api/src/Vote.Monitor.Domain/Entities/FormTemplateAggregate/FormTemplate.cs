using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public class FormTemplate : AuditableBaseEntity, IAggregateRoot
{
    public FormTemplateType FormTemplateType { get; private set; }
    public string Code { get; private set; }
    public string DefaultLanguage { get; private set; }
    public TranslatedString Name { get; private set; }
    public TranslatedString Description { get; private set; }
    public FormTemplateStatus Status { get; private set; }
    public string[] Languages { get; private set; } = [];

    public IReadOnlyList<BaseQuestion> Questions { get; private set; } = new List<BaseQuestion>().AsReadOnly();

    private FormTemplate(FormTemplateType formTemplateType,
        string code,
        string defaultLanguage,
        TranslatedString name,
        TranslatedString description,
        IEnumerable<string> languages) : base(Guid.NewGuid())
    {
        FormTemplateType = formTemplateType;
        Code = code;
        DefaultLanguage = defaultLanguage;
        Name = name;
        Description = description;
        Languages = languages.ToArray();
        Status = FormTemplateStatus.Drafted;
    }

    public static FormTemplate Create(FormTemplateType formTemplateType,
        string code,
        string defaultLanguage,
        TranslatedString name,
        TranslatedString description,
        IEnumerable<string> languages) =>
        new(formTemplateType, code, defaultLanguage, name, description, languages);

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

    public void UpdateDetails(string code,
        string defaultLanguage,
        TranslatedString name,
        TranslatedString description,
        FormTemplateType formTemplateType,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions)
    {
        Code = code;
        DefaultLanguage = defaultLanguage;
        Name = name;
        Description = description;
        FormTemplateType = formTemplateType;
        Languages = languages.ToArray();
        Questions = questions.ToList().AsReadOnly();
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private FormTemplate()
    {

    }
#pragma warning restore CS8618
}
