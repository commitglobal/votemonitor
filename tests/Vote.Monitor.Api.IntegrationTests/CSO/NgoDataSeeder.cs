using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.Api.IntegrationTests.CSO;

public class NgoDataSeeder(
    ILogger<NgoDataSeeder> logger,
    IRepository<Domain.Entities.CSOAggregate.CSO> repository,
    ITimeProvider timeProvider)
    : IDataSeeder
{
    public async Task SeedDataAsync()
    {
        logger.LogInformation("Generating ngo Test Data");

        var activatedNgos = Enumerable
            .Range(0, 20)
            .Select(x => new Domain.Entities.CSOAggregate.CSO($"Activated{x}", timeProvider));

        var deactivatedNgos = Enumerable
                  .Range(0, 20)
                  .Select(x =>
                  {
                      var ngo = new Domain.Entities.CSOAggregate.CSO($"Deactivated{x}", timeProvider);
                      ngo.Deactivate();
                      return ngo;
                  });

        await repository.AddRangeAsync(activatedNgos);
        await repository.AddRangeAsync(deactivatedNgos);
    }
}
