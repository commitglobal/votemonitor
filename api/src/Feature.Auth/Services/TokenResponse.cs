namespace Feature.Auth.Services;

public record TokenResponse(string Token, string RefreshToken, DateTime RefreshTokenExpiryTime, string Role);
