using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using Vote.Monitor.Api.Feature.Auth.Services;

namespace Vote.Monitor.Api.IntegrationTests;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions =
        new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public static HttpClient NewForAuthenticatedUser(this Func<HttpClient> clientFactory, string email,
        string password)
    {
        var authenticatedUser = clientFactory();

        var loginResponse = authenticatedUser
            .PostWithResponse<TokenResponse>("/api/auth/login", new { email, password });

        authenticatedUser.DefaultRequestHeaders.Authorization = new("Bearer", loginResponse.Token);

        return authenticatedUser;
    }

    public static void PostWithoutResponse(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri,
        object value)
    {
        var response = client.PostAsJsonAsync(requestUri, value, new JsonSerializerOptions()).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            TestContext.Out.WriteLine(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }

        response.EnsureSuccessStatusCode();
    }

    public static void PostWithoutResponse(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri)
    {
        var response = client.PostAsync(requestUri, null).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            TestContext.Out.WriteLine(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }

        response.EnsureSuccessStatusCode();
    }

    public static void PutWithoutResponse(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri,
        object value)
    {
        var response = client.PutAsJsonAsync(requestUri, value, new JsonSerializerOptions()).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            TestContext.Out.WriteLine(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }

        response.EnsureSuccessStatusCode();
    }

    public static TResponse PostWithResponse<TResponse>(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri,
        object value)
    {
        var response = client.PostAsJsonAsync(requestUri, value, new JsonSerializerOptions()).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            TestContext.Out.WriteLine(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }

        response.EnsureSuccessStatusCode();

        var result = response.Content.ReadFromJsonAsync<TResponse>(_jsonSerializerOptions).GetAwaiter().GetResult();
        return result!;
    }

    public static TResponse PutWithResponse<TResponse>(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri,
        object value)
    {
        var response = client.PutAsJsonAsync(requestUri, value).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            TestContext.Out.WriteLine(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }

        response.EnsureSuccessStatusCode();

        var result = response.Content.ReadFromJsonAsync<TResponse>(_jsonSerializerOptions).GetAwaiter().GetResult();
        return result!;
    }

    public static TResponse GetResponse<TResponse>(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri)
    {
        var response = client.GetAsync(requestUri).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            TestContext.Out.WriteLine(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }

        response.EnsureSuccessStatusCode();

        var result = response.Content.ReadFromJsonAsync<TResponse>(_jsonSerializerOptions).GetAwaiter().GetResult();
        return result!;
    }


    public static async Task<TResponse> PostWithResponseAsync<TResponse>(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri,
        object value,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync(requestUri, value, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            TestContext.Out.WriteLine(await response.Content.ReadAsStringAsync(cancellationToken));
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TResponse>(_jsonSerializerOptions, cancellationToken);
        return result!;
    }

    public static async Task<TResponse> PutWithResponseAsync<TResponse>(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri,
        object value,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync(requestUri, value, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            TestContext.Out.WriteLine(await response.Content.ReadAsStringAsync(cancellationToken));
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TResponse>(_jsonSerializerOptions, cancellationToken);
        return result!;
    }

    public static async Task<TResponse> GetResponseAsync<TResponse>(
        this HttpClient client,
        [StringSyntax("Uri")] string? requestUri,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            TestContext.Out.WriteLine(await response.Content.ReadAsStringAsync(cancellationToken));
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TResponse>(_jsonSerializerOptions, cancellationToken);
        return result!;
    }
}
