using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Vote.Monitor.Core.Services.Serialization;

public class SerializerService(ILogger<SerializerService> logger) : ISerializerService
{
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    public string Serialize<T>(T obj)
    {
        try
        {
            var json = JsonSerializer.Serialize(obj, _serializerOptions);

            return json;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error serializing the object");
            throw;
        }
    }
}
