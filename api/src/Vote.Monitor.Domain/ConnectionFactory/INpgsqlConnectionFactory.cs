using System.Data;

namespace Vote.Monitor.Domain.ConnectionFactory;

public interface INpgsqlConnectionFactory
{
   Task< IDbConnection> GetOpenConnectionAsync(CancellationToken ct);
}
