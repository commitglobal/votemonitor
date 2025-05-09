﻿using Authorization.Policies;
using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.DataExport.Get;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request>
{
    public override void Configure()
    {
        Get("/api/exported-data/{id}");
        DontAutoTag();
        Options(x => x.WithTags("exported-data"));
        Summary(s =>
        {
            s.Summary = "Gets exported data excel";
        });
        Policies(PolicyNames.AdminsOnly);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var sql = """
            SELECT "Base64EncodedData", "FileName"
            FROM "ExportedData"
            WHERE "Id" = @exportedDataId and "OwnerId" = @ownerId;
        """;

        var queryParams = new
        {
            ownerId = req.UserId,
            exportedDataId = req.Id
        };

        Response exportedData;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            exportedData = await dbConnection.QueryFirstOrDefaultAsync<Response>(sql, queryParams);
        }

        if (exportedData == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(exportedData.Base64EncodedData))
        {
            await SendNoContentAsync(ct);
            return;
        }

        var bytes = Convert.FromBase64String(exportedData.Base64EncodedData);
        await SendBytesAsync(bytes, fileName: exportedData.FileName, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", cancellation: ct);
    }
}
