using System.Globalization;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Authorization.Policies;
using CsvHelper;
using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers.Export;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request>
{
    private class MonitoringObserverExportModel
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string[] Tags { get; init; }
        public DateTime? LatestActivityAt { get; init; }

        [JsonConverter(typeof(SmartEnumNameConverter<MonitoringObserverStatus, string>))]
        public MonitoringObserverStatus Status { get; init; }
    }

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
        var sql = """
                      SELECT
                          MO."Id",
                          U."FirstName",
                          U."LastName",
                          U."PhoneNumber",
                          U."Email",
                          MO."Tags",
                          MO."Status"
                      FROM
                          "MonitoringObservers" MO
                          INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                          INNER JOIN "Observers" O ON O."Id" = MO."ObserverId"
                          INNER JOIN "AspNetUsers" U ON U."Id" = O."ApplicationUserId"
                      WHERE
                          MN."ElectionRoundId" = @electionRoundId
                          AND MN."NgoId" = @ngoId
                  """;

        var queryArgs = new { electionRoundId = req.ElectionRoundId, ngoId = req.NgoId };
        IEnumerable<MonitoringObserverExportModel> monitoringObservers = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            monitoringObservers = await dbConnection.QueryAsync<MonitoringObserverExportModel>(sql, queryArgs);
        }

        var monitoringObserverModels = monitoringObservers.ToArray();

        var availableTags = monitoringObserverModels
            .SelectMany(x => x.Tags)
            .ToHashSet()
            .OrderBy(x => x)
            .ToList();

        using (var memoryStream = new MemoryStream())
        {
            using (var streamWriter = new StreamWriter(memoryStream, leaveOpen: true))
            {
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    WriteHeader(csvWriter, availableTags);
                    WriteData(monitoringObserverModels, csvWriter, availableTags);

                    streamWriter.Flush();

                    await SendBytesAsync(memoryStream.ToArray(), "monitoring-observers.csv",
                        contentType: "text/csv",
                        cancellation: ct);
                }
            }
        }
    }

    private void WriteData(MonitoringObserverExportModel[] monitoringObservers, CsvWriter csvWriter,
        List<string> availableTags)
    {
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
                csvWriter.WriteField(tag);
            }

            csvWriter.NextRecord();
        }
    }

    private static void WriteHeader(CsvWriter csvWriter, List<string> availableTags)
    {
        csvWriter.WriteField(nameof(MonitoringObserverExportModel.Id));
        csvWriter.WriteField(nameof(MonitoringObserverExportModel.FirstName));
        csvWriter.WriteField(nameof(MonitoringObserverExportModel.LastName));
        csvWriter.WriteField(nameof(MonitoringObserverExportModel.PhoneNumber));
        csvWriter.WriteField(nameof(MonitoringObserverExportModel.Email));
        csvWriter.WriteField(nameof(MonitoringObserverExportModel.Status));

        foreach (var tag in availableTags)
        {
            csvWriter.WriteField(tag);
        }

        csvWriter.NextRecord();
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
