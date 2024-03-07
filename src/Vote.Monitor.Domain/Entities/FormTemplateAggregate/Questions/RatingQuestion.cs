using System.Text.Json.Serialization;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

public class RatingQuestion : BaseQuestion
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public RatingScale Scale { get; private set; }

    [JsonConstructor]
    private RatingQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        RatingScale scale) : base(text, helptext)
    {
        Id = id;
        Code = code;
        Scale = scale;
    }
    
    public static RatingQuestion Create(Guid id, string code, TranslatedString text, TranslatedString? helptext, RatingScale scale) => 
        new(id, code, text, helptext, scale);
}
