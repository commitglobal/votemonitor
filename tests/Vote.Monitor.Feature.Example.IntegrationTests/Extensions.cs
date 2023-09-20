using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Vote.Monitor.Feature.Example.IntegrationTests;

public static class Extensions
{
    public static ValidationProblemDetails ToValidationProblemDetails(this HttpContent content)
    {
        var json = content.ReadAsStringAsync().GetAwaiter().GetResult();

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web); //note: cache and reuse this
        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(json, jsonOptions);

        return problemDetails;
    }
}
