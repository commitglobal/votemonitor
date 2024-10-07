using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.ValueConverters;

public class AnswersToJsonConverter : ValueConverter<IReadOnlyList<BaseAnswer>, string>
{
    public AnswersToJsonConverter() : base(
        v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true }),
        v => JsonSerializer.Deserialize<IReadOnlyList<BaseAnswer>>(v, new JsonSerializerOptions { WriteIndented = true }))
    {
    }
}