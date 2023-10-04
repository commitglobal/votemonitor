using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.Data;
public class AppDbContext: DbContext
{
    public DbSet<PollingStationEf> PollingStations { get; set; }
    public DbSet<TagEf> Tags { get; set; }    

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PollingStationEf>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.PollingStations)
            .UsingEntity(
            "PollingStationTag",
            l => l.HasOne(typeof(TagEf)).WithMany().HasForeignKey("TagId").HasPrincipalKey(nameof(TagEf.Id)),
            r => r.HasOne(typeof(PollingStationEf)).WithMany().HasForeignKey("PollingStationId").HasPrincipalKey(nameof(PollingStationEf.Id)),
            j => j.HasKey("PollingStationId", "TagId"))
            .Navigation(e=>e.Tags).AutoInclude();
        
    }

}
