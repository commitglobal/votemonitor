namespace Vote.Monitor.Auth.Options;

public class JWTConfig
{
    public const string Key = "JWTConfig";
    public string TokenSigningKey { get; set; }
}
