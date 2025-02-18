using FluentValidation;
using FluentValidation.Results;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public class FormTemplate : BaseForm
{
    [JsonConstructor]
    public FormTemplate(Guid id,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        FormStatus status,
        string defaultLanguage,
        string[] languages,
        string? icon,
        int numberOfQuestions,
        LanguagesTranslationStatus languagesTranslationStatus) : base(id,
        formType,
        code,
        name,
        description,
        status,
        defaultLanguage,
        languages,
        icon,
        numberOfQuestions,
        languagesTranslationStatus)
    {
    }

    private FormTemplate(FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        string? icon,
        IEnumerable<BaseQuestion> questions,
        FormStatus status) : base(formType,
        code,
        name,
        description,
        defaultLanguage,
        languages,
        icon,
        questions,
        status)
    {
    }

    public static FormTemplate Create(FormType formType,
        string code,
        string defaultLanguage,
        TranslatedString name,
        TranslatedString description,
        IEnumerable<string> languages,
        string? icon,
        IEnumerable<BaseQuestion> questions) =>
        new(formType, code, name, description, defaultLanguage, languages, icon, questions, FormStatus.Drafted);

    public FormTemplate Duplicate() =>
        new(FormType, Code, Name, Description, DefaultLanguage, Languages, Icon, Questions, FormStatus.Drafted);

    public Form Clone(Guid electionRoundId, Guid monitoringNgoId, string defaultLanguage, string[] languages)
    {
        if (Status != FormStatus.Published)
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Status), "Form template is not published.")
            ]);
        }

        if (!Languages.Contains(defaultLanguage))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(defaultLanguage), "Default language is not supported.")
            ]);
        }

        foreach (var iso in languages)
        {
            if (!Languages.Contains(iso))
            {
                throw new ValidationException([
                    new ValidationFailure(nameof(languages) + $".{iso}", "Language is not supported.")
                ]);
            }
        }

        return Form.Create(electionRoundId,
            monitoringNgoId,
            FormType,
            Code,
            new TranslatedString(Name).TrimTranslations(languages),
            new TranslatedString(Description).TrimTranslations(languages),
            defaultLanguage,
            languages,
            null,
            Questions.Select(x => x.DeepClone().TrimTranslations(languages)).ToList());
    }

    public override DraftFormResult DraftInternal()
    {
        return new DraftFormResult.Drafted();
    }

    public override ObsoleteFormResult ObsoleteInternal()
    {
        return new ObsoleteFormResult.Obsoleted();
    }

    public override PublishFormResult PublishInternal()
    {
        return new PublishFormResult.Published();
    }

#pragma warning disable CS8618 // Required by Entity Framework

    private FormTemplate()
    {
    }
#pragma warning restore CS8618
}
