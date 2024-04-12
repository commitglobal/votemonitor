namespace Vote.Monitor.Api.Feature.Auth.Options;

public class JWTConfig
{
    public const string Key = "JWTConfig";
    public string TokenSigningKey { get; set; }
    public long TokenExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }

}
