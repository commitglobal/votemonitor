using Spectre.Console;

public class LoggingHandler(HttpMessageHandler innerHandler) : DelegatingHandler(innerHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            AnsiConsole.Markup("[red]Non success status code received[/]");

            if (request.Content != null)
            {
                var requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
                AnsiConsole.Markup("[red]request: {0}[/]", requestBody.EscapeMarkup());
            }

            if (response.Content != null)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                AnsiConsole.Markup("[red]response: {0}[/]", responseBody.EscapeMarkup());
            }
        }

        return response;
    }
}