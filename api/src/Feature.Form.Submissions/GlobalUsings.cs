// Global using directives

global using Authorization.Policies;
global using Authorization.Policies.Requirements;
global using Dapper;
global using FastEndpoints;
global using Feature.Form.Submissions.Specifications;
global using FluentValidation;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Vote.Monitor.Domain.ConnectionFactory;
global using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
global using Vote.Monitor.Domain.Repository;
global using PollingStationAggregate = Vote.Monitor.Domain.Entities.PollingStationAggregate.PollingStation;
global using FormAggregate = Vote.Monitor.Domain.Entities.FormAggregate.Form;
