namespace Vote.Monitor.Core.Services.ConnectionString;

public class ConnectionStringProvider : IConnectionStringProvider
{
    public ConnectionStringProvider(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Null or empty connection string provided", nameof(connectionString));
        }

        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }
}
