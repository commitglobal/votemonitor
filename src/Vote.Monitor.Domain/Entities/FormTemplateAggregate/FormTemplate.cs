﻿using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Validation;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public class FormTemplate : AuditableBaseEntity, IAggregateRoot
{
    public FormType FormType { get; private set; }
    public string Code { get; private set; }
    public TranslatedString Name { get; private set; }
    public FormTemplateStatus Status { get; private set; }

    public IReadOnlyList<string> Languages { get; private set; } = new List<string>().AsReadOnly();

    public IReadOnlyList<FormSection> Sections { get; private set; } = new List<FormSection>().AsReadOnly();

    private FormTemplate(FormType formType,
        string code,
        TranslatedString name,
        IEnumerable<string> languages,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        FormType = formType;
        Code = code;
        Name = name;
        Languages = languages.ToList().AsReadOnly();
        Status = FormTemplateStatus.Drafted;
    }

    public static FormTemplate Create(FormType formType,
        string code,
        TranslatedString name,
        IEnumerable<string> languages,
        ITimeProvider timeProvider) =>
        new(formType, code, name, languages, timeProvider);

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
        FormType formType,
        IEnumerable<string> languages,
        IEnumerable<FormSection> sections)
    {
        Code = code;
        Name = name;
        FormType = formType;
        Languages = languages.ToList().AsReadOnly();
        Sections = sections.ToList().AsReadOnly();
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private FormTemplate()
    {

    }
#pragma warning restore CS8618
}


public abstract record PublishResult
{
    public record Published : PublishResult;
    public record InvalidFormTemplate(ValidationResult Problems) : PublishResult;
}
