using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Domain.DataContext;
public class AppDbContext : DbContext
{
    public DbSet<PollingStation> PollingStations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasPostgresExtension("uuid-ossp");

        builder.Entity<PollingStation>()
         .Property(e => e.Id)
         .HasDefaultValueSql("uuid_generate_v4()");

        var method = typeof(Postgres.Functions).GetRuntimeMethod(nameof(Postgres.Functions.ObjectKeys), new[] { typeof(JsonDocument) });

        builder
            .HasDbFunction(method!)
            .HasName("jsonb_object_keys");
    }
}
