// Global using directives

global using Ardalis.Specification;
global using FastEndpoints;
global using FluentValidation;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Vote.Monitor.Domain.Repository;
global using Microsoft.AspNetCore.Http;
global using Vote.Monitor.Core.Services.Time;
global using ElectionRoundAggregate = Vote.Monitor.Domain.Entities.ElectionRoundAggregate.ElectionRound;
global using NgoAggregate = Vote.Monitor.Domain.Entities.NgoAggregate.Ngo;
global using MonitoringNgoAggregate = Vote.Monitor.Domain.Entities.MonitoringNgoAggregate.MonitoringNgo;
global using MonitoringObserverAggregate = Vote.Monitor.Domain.Entities.MonitoringObserverAggregate.MonitoringObserver;
global using ObserverAggregate = Vote.Monitor.Domain.Entities.ObserverAggregate.Observer;
