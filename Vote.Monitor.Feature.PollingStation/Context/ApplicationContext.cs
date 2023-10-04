using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Feature.PollingStation.Models;

namespace Vote.Monitor.Feature.PollingStation.Context;
//TODO: EntityFramework
public class ApplicationContext : DbContext
{
    public DbSet<PollingStationModel> PollingStation { get; set; }
    public DbSet<Tag> Tag { get; set; }
    public DbSet<PollingStationTag> PollingStationTag { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
