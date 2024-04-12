using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Vote.Monitor.Core.Extensions;

public static class ConfigurationExtensions
{
    public static string GetNpgsqlConnectionString(this IConfiguration config, string section)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = config[$"{section}:Server"]!,
            Port = int.Parse(config[$"{section}:Port"]!),
            Database = config[$"{section}:Database"]!,
            Username = config[$"{section}:UserId"],
            Password = config[$"{section}:Password"],
            IncludeErrorDetail = true,
        };

        return connectionStringBuilder.ToString();
    }
}
