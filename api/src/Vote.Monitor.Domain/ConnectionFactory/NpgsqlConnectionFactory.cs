using System.Data;
using Npgsql;

namespace Vote.Monitor.Domain.ConnectionFactory;

public class NpgsqlConnectionFactory(string connectionString) : INpgsqlConnectionFactory
{
    public async Task<IDbConnection> GetOpenConnectionAsync(CancellationToken ct)
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(ct);
        return connection;
    }
}
