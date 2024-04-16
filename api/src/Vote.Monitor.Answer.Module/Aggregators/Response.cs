namespace Vote.Monitor.Answer.Module.Aggregators;

public record Response<T>(Guid Responder, T Value);
