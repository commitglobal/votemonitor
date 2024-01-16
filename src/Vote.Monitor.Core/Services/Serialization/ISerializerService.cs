namespace Vote.Monitor.Core.Services.Serialization;

public interface ISerializerService
{
    string Serialize<T>(T obj);
}
