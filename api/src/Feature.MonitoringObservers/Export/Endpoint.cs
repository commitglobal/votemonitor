using System.Globalization;
using System.Text;
using Authorization.Policies;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.MonitoringObservers.Export;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request>
{
    public override void Configure()
    {
        Get("api/election-rounds/{electionRoundId}/monitoring-observers:export");
        DontAutoTag();
        Options(x => x
            .WithTags("monitoring-observers")
            .Produces(200, typeof(string), contentType: "text/csv")
        );

        Summary(s =>
        {
            s.Summary = "Exports monitoring observers to csv file";
        });

        Policies(PolicyNames.AdminsOnly);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var monitoringObservers = await context.MonitoringObservers
            .Include(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => x.MonitoringNgo.NgoId == req.NgoId && x.ElectionRoundId == req.ElectionRoundId)
            .Select(x => MonitoringObserverModel.FromEntity(x))
            .ToListAsync(ct);

        var availableTags = monitoringObservers
            .SelectMany(x => x.Tags)
            .ToHashSet()
            .OrderBy(x => x)
            .ToList();

        using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true);
        await using var csvWriter = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);
        csvWriter.WriteHeader<MonitoringObserverModel>();
        foreach (var monitoringObserver in monitoringObservers)
        {
            csvWriter.WriteField(monitoringObserver.Id.ToString());
            csvWriter.WriteField(monitoringObserver.FirstName);
            csvWriter.WriteField(monitoringObserver.LastName);
            csvWriter.WriteField(monitoringObserver.PhoneNumber);
            csvWriter.WriteField(monitoringObserver.Email);
            csvWriter.WriteField(monitoringObserver.Status);

            var tags = MergeTags(monitoringObserver.Tags, availableTags);
            foreach (var tag in tags)
            {
                await csvWriter.NextRecordAsync();

            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        await SendStreamAsync(stream, "monitoring-observers.csv", stream.Length, "text/csv", cancellation: ct);
    }

    private IReadOnlyList<string> MergeTags(IReadOnlyList<string> tags, List<string> availableTags)
    {
        List<string> result = [];

        foreach (var tag in availableTags)
        {
            result.Add(tags.Contains(tag) ? tag : string.Empty);
        }

        return result;
    }

}
