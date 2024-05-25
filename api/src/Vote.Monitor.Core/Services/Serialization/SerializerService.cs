using System.Text.Json;

namespace Vote.Monitor.Core.Services.Serialization;

public class SerializerService : ISerializerService
{
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        MaxDepth = 3
    };

    public string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _serializerOptions);
}
