using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Http.HttpResults;
using NPOI.SS.Formula.Functions;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record MatrixQuestion : BaseQuestion
{
    public IReadOnlyList<MatrixOption> Options { get; private set; }
    public IReadOnlyList<MatrixRow> Rows { get; private set; }

    [JsonConstructor]
    internal MatrixQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        DisplayLogic? displayLogic,
        IReadOnlyList<MatrixOption> options,
        IReadOnlyList<MatrixRow> rows) : base(id, code, text, helptext, displayLogic)
    {
        Options = options;
        Rows = rows;
    }

    protected override void AddTranslationsInternal(string languageCode)
    {
        foreach (var option in Options)   
        {
            option.AddTranslation(languageCode);
        }

        foreach (var row in Rows)  
        {
            row.AddTranslation(languageCode);
        }
    }

    protected override void RemoveTranslationInternal(string languageCode)
    {
        foreach (var option in Options)   
        {
            option.RemoveTranslation(languageCode);
        }

        foreach (var row in Rows)  
        {
            row.RemoveTranslation(languageCode);
        }
    }

    protected override TranslationStatus InternalGetTranslationStatus(string baseLanguageCode, string languageCode)
    {
        bool anyMissingInOptions = Options.Any(x => string.IsNullOrWhiteSpace(x.Text[languageCode]));
        bool anyMissingInRows = Rows.Any(x => string.IsNullOrWhiteSpace(x.Text[languageCode]));

        return anyMissingInOptions || anyMissingInRows
            ? TranslationStatus.MissingTranslations
            : TranslationStatus.Translated;
    }

    protected override void InternalTrimTranslations(IEnumerable<string> languages)
    {
        var languagesArray = languages as string[] ?? languages.ToArray();
        foreach (var option in Options)
        {
            option.TrimTranslations(languagesArray);
        }

        foreach (var row in Rows)  
        {
            row.TrimTranslations(languagesArray);
        }
    }

    public static MatrixQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        DisplayLogic? displayLogic,
        IReadOnlyList<MatrixOption> options,
        IReadOnlyList<MatrixRow> rows)
        => new(id, code, text, helptext, displayLogic, options, rows);
    
    public virtual bool Equals(MatrixQuestion? other)
    {
        bool options = base.Equals(other) && Options.SequenceEqual(other.Options);
        bool rows = base.Equals(other) && Rows.SequenceEqual(other.Rows);
        
        return options && rows;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Options, Rows);
    }
}
