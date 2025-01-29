using System.Text.Json;
using Dapper;

namespace Vote.Monitor.Core.Converters;

public class JsonToObjectConverter<T> : SqlMapper.TypeHandler<T> where T : class
{
    public override T Parse(object value)
    {
        var json = value?.ToString();
        return json == null ? default(T) : JsonSerializer.Deserialize<T>(json);
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, T value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }
}
