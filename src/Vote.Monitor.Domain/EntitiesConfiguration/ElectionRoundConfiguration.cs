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

        builder.Navigation(nameof(ElectionRound.MonitoringNgos))
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(nameof(ElectionRound.MonitoringNgos))
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(e => e.MonitoringNgos, ngoBuilder =>
        {
            ngoBuilder.ToTable("MonitoringNGOs");
            ngoBuilder.WithOwner().HasForeignKey(nameof(MonitoringNGO.ElectionRoundId));
            ngoBuilder.HasKey("Id");

            ngoBuilder
                .HasOne(x => x.Ngo)
                .WithMany()
                .HasForeignKey(x => x.NgoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.OwnsMany(e => e.MonitoringObservers, o =>
        {
            o.ToTable("MonitoringObservers");
            o.WithOwner().HasForeignKey(nameof(MonitoringObserver.ElectionRoundId));
            o.HasKey("Id");

            o
                .HasOne(x => x.Observer)
                .WithMany()
                .HasForeignKey(x => x.ObserverId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder
            .HasOne(e => e.Country)
            .WithMany()
            .HasForeignKey(e => e.CountryId);
    }
}
