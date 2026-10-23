// Global using directives

global using Authorization.Policies;
global using Authorization.Policies.Requirements;
global using Dapper;
global using FastEndpoints;
global using FluentValidation;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.EntityFrameworkCore;
global using Vote.Monitor.Core.Extensions;
global using Vote.Monitor.Core.Models;
global using Vote.Monitor.Core.Security;
global using Vote.Monitor.Core.Services.Security;
global using Vote.Monitor.Core.Services.Time;
global using Vote.Monitor.Domain;
global using Vote.Monitor.Domain.ConnectionFactory;
global using Vote.Monitor.Domain.Entities.ReviewAggregate;
global using Vote.Monitor.Domain.Entities.UserNotificationAggregate;
global using ReviewWorkflowEntity = Vote.Monitor.Domain.Entities.ReviewAggregate.ReviewWorkflow;
