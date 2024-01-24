﻿global using System.Text;
global using FluentAssertions;
global using FluentValidation.TestHelper;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using NSubstitute;
global using Vote.Monitor.Api.Feature.Observer.Services;
global using Xunit;

global using ObserverAggregate = Vote.Monitor.Domain.Entities.ApplicationUserAggregate.Observer;
global using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
global using Vote.Monitor.Api.Feature.Observer.Specifications;
global using Vote.Monitor.Core.Models;
global using Vote.Monitor.TestUtils;
global using Vote.Monitor.TestUtils.Fakes;
