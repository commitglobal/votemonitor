namespace Vote.Monitor.Feature.Example.Services;

internal class SomethingSomethingService : ISomethingSomethingService
{
    public async Task<SomethingResult> DoSomethingAsync(string aParameter)
    {
        if (string.IsNullOrEmpty(aParameter))
        {
            return new SomethingResult.Error("Cannot have empty params");
        }

        if (aParameter.Length %2 == 1)
        {
            return new SomethingResult.Error("Our business rules do not allow odd length of parameter");
        }

        await Task.Delay(100);

        return new SomethingResult.Ok($"I did something with this: {aParameter}");
    }
}
