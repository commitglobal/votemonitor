﻿namespace Feature.Locations.Delete;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid Id { get; set; }
}