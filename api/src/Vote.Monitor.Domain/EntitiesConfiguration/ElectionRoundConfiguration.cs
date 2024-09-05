using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ElectionRoundConfiguration : IEntityTypeConfiguration<ElectionRound>
{
    public void Configure(EntityTypeBuilder<ElectionRound> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.Title).HasMaxLength(256).IsRequired();
        builder.Property(e => e.EnglishTitle).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.CitizenReportingEnabled).HasDefaultValue(false).IsRequired();
        
        
        builder
            .HasOne(e => e.Country)
            .WithMany()
            .HasForeignKey(e => e.CountryId);

        builder
            .HasMany(x => x.MonitoringNgos)
            .WithOne(x => x.ElectionRound)
            .HasForeignKey(x => x.ElectionRoundId);

        builder.Navigation(nameof(ElectionRound.MonitoringNgos))
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder
            .HasOne(e => e.MonitoringNgoForCitizenReporting)
            .WithMany()
            .HasForeignKey(e => e.MonitoringNgoForCitizenReportingId);
    }
}
