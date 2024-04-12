namespace Vote.Monitor.Domain.Seeders;

public interface IAmDbSeeder
{
    string SectionKey { get; }
    Task SeedAsync();
}
