using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.Forms.Models;

public class FormSlimModel
{
    public TranslatedString Description { get; set; }

    public required Guid Id { get; init; }

    public required FormType FormType { get; init; }

    public required string Code { get; init; }
    public TranslatedString Name { get; init; }
    public string? Icon { get; init; }

    public required FormStatus Status { get; init; }

    public required string DefaultLanguage { get; init; }
    public required string[] Languages { get; init; } = [];
    public int NumberOfQuestions { get; init; }

    /// <summary>
    /// Records sent on the form: all of them for the NGO's own forms, its own observers' for forms a coalition shares
    /// with it (from the forms list function; 0 where a form is read another way).
    /// </summary>
    public int NumberOfSubmissions { get; init; }
    public required DateTime LastModifiedOn { get; init; }

    public string LastModifiedBy { get; init; }
    public bool IsFormOwner { get; init; }

    public FormAccessModel[] FormAccess { get; init; }

    public LanguagesTranslationStatus LanguagesTranslationStatus { get; init; }
}
