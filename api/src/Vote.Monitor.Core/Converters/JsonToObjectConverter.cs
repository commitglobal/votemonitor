using System.Text.Json;
using Dapper;

namespace Vote.Monitor.Core.Converters;

public class JsonToObjectConverter<T> : SqlMapper.TypeHandler<T> where T : class
{
    public override T Parse(object value)
    {
        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, T value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }
}
