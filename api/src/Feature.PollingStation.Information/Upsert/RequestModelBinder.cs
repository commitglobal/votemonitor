using System.Globalization;
using System.Text;
using System.Text.Json;
using FastEndpoints.Security;
using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.PollingStation.Information.Upsert;

public class RequestModelBinder() : IRequestBinder<Request>
{
    // Parse the string into a JsonDocument
    private readonly JsonDocumentOptions _jsonOptions = new()
    {
        AllowTrailingCommas = true
    };

    public async ValueTask<Request> BindAsync(BinderContext ctx, CancellationToken ct)
    {
        string bodyAsString = string.Empty;
        // Read the body as a string
        using var reader = new StreamReader(ctx.HttpContext.Request.Body, Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        // Read the stream asynchronously
        bodyAsString = await reader.ReadToEndAsync(ct);

        var jsonDocument = JsonDocument.Parse(bodyAsString, _jsonOptions);

        var breaksPropertyExist = jsonDocument.RootElement.TryGetProperty("breaks", out var breaksElement);
        var answersPropertyExist = jsonDocument.RootElement.TryGetProperty("answers", out var answersElement);

        var arrivalTimePropertyExist =
            jsonDocument.RootElement.TryGetProperty("arrivalTime", out var arrivalTimeElement);

        var departureTimePropertyExist =
            jsonDocument.RootElement.TryGetProperty("departureTime", out var departureTimeElement);

        var isCompletedPropertyExist =
            jsonDocument.RootElement.TryGetProperty("isCompleted", out var isCompletedElement);

        var request = new Request
        {
            ElectionRoundId = Guid.Parse(ctx.HttpContext.Request.RouteValues["electionRoundId"]?.ToString()!),
            PollingStationId = Guid.Parse(ctx.HttpContext.Request.RouteValues["pollingStationId"]?.ToString()!),
            ObserverId = Guid.Parse(ctx.HttpContext.User.ClaimValue(ApplicationClaimTypes.UserId)!),

            ArrivalTime = arrivalTimePropertyExist
                ? ParseDateTime(arrivalTimeElement)
                : ValueOrUndefined<DateTime?>.Undefined(),
            DepartureTime = departureTimePropertyExist
                ? ParseDateTime(departureTimeElement)
                : ValueOrUndefined<DateTime?>.Undefined(),
            IsCompleted = isCompletedPropertyExist
                ? ParseBool(isCompletedElement)
                : ValueOrUndefined<bool>.Undefined(),

            Answers =
                answersPropertyExist ? DeserializeJsonElement<List<BaseAnswerRequest>>(answersElement) : null,
            Breaks = breaksPropertyExist ? DeserializeJsonElement<List<Request.BreakRequest>>(breaksElement) : null
        };

        return request;
    }

    private static ValueOrUndefined<bool> ParseBool(JsonElement isCompletedElement)
    {
        var success = bool.TryParse(isCompletedElement.GetRawText(), out var result);
        return success ? ValueOrUndefined<bool>.Some(result) : ValueOrUndefined<bool>.Some(false);
    }

    private static ValueOrUndefined<DateTime?> ParseDateTime(JsonElement dateTimeJsonElement)
    {
        if (dateTimeJsonElement.ValueKind == JsonValueKind.Null)
        {
            return ValueOrUndefined<DateTime?>.Some(null);
        }

        // var success = DateTime.TryParse(dateTimeJsonElement.GetRawText(), null, DateTimeStyles.RoundtripKind, out var result);
        var success = dateTimeJsonElement.TryGetDateTime(out var result);

        return success ? ValueOrUndefined<DateTime?>.Some(result) : ValueOrUndefined<DateTime?>.Some(null);
    }

    private static T? DeserializeJsonElement<T>(JsonElement jsonElement)
    {
        var jsonString = jsonElement.GetRawText();

        var result = JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        return result;
    }
}