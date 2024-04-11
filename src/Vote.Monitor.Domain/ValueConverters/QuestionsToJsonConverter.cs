using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.ValueConverters;

public class QuestionsToJsonConverter : ValueConverter<IReadOnlyList<BaseQuestion>, string>
{
    public QuestionsToJsonConverter() : base(
        v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true }),
        v => JsonSerializer.Deserialize<IReadOnlyList<BaseQuestion>>(v, new JsonSerializerOptions { WriteIndented = true }))
    {
    }
}
