namespace Vote.Monitor.Api.Feature.Auth.Options;

public class JWTConfig
{
    public const string Key = "JWTConfig";
    public required string TokenSigningKey { get; set; }
}
