using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.ViewModels;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class PollingStationVisitsViewConfiguration : IEntityTypeConfiguration<PollingStationVisitViewModel>
{
    public void Configure(EntityTypeBuilder<PollingStationVisitViewModel> builder)
    {
        builder
            .ToView(CustomDBViews.PollingStationVisits)
            .HasNoKey();
    }
}
