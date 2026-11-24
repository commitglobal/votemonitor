using Dapper;
using Feature.Forms.Models;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Forms.Get;

/// <summary>A form as its GET returns it, with its version and who saved that version.</summary>
public static class FormDetailsQuery
{
    private const string Sql = """
                               SELECT *
                               FROM "GetNgoFormDetails"(@electionRoundId, @ngoId, @id);

                               SELECT F.xmin::text::bigint AS "Version", U."DisplayName" AS "LastModifiedByName"
                               FROM "Forms" F
                                        LEFT JOIN "AspNetUsers" U ON U."Id" = F."LastModifiedBy"
                               WHERE F."Id" = @id
                               UNION ALL
                               SELECT P.xmin::text::bigint, U."DisplayName"
                               FROM "PollingStationInformationForms" P
                                        LEFT JOIN "AspNetUsers" U ON U."Id" = P."LastModifiedBy"
                               WHERE P."Id" = @id;
                               """;

    public static async Task<FormFullModel?> GetAsync(INpgsqlConnectionFactory dbConnectionFactory,
        Guid electionRoundId,
        Guid ngoId,
        Guid id,
        CancellationToken ct)
    {
        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(new CommandDefinition(Sql,
            new { electionRoundId, ngoId, id }, cancellationToken: ct));

        var form = (await multi.ReadAsync<FormFullModel>()).SingleOrDefault();
        var version = (await multi.ReadAsync<VersionRow>()).FirstOrDefault();
        if (form is null)
        {
            return null;
        }

        if (version is not null)
        {
            form.Version = (uint)version.Version;
            form.LastModifiedByName = version.LastModifiedByName;
        }

        return form;
    }

    private sealed class VersionRow
    {
        public long Version { get; init; }
        public string? LastModifiedByName { get; init; }
    }
}
