using System.Data;

namespace Vote.Monitor.Domain.ConnectionFactory;

public interface INpgsqlConnectionFactory
{
    IDbConnection GetOpenConnection();
}
