﻿namespace Vote.Monitor.Api.Feature.Auth.Login;

public class Request
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
