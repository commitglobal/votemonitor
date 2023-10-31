using System.Text.Json;

namespace Vote.Monitor.Domain;

public partial class Postgres
{
    // DB Functions
    public static class Functions
    {
        public static string ObjectKeys(JsonDocument @object) =>
            throw new InvalidOperationException("This method is not meant to be called directly.");
    }
}
