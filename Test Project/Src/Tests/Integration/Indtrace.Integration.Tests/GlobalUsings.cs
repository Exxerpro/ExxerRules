// Global using directives

global using System;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using FluentValidation.TestHelper;
global using IndTrace.Application.BarCodes.Commands.Create;
global using IndTrace.Application.BarCodes.Commands.Update;
global using IndTrace.Application.BarCodes.Queries.GetBarCodeGatewayDetail;
global using IndTrace.Application.BarCodes.Services;
global using IndTrace.Application.Cycles.Commands.Create;
global using IndTrace.Application.Machines.Commands.Create;
global using IndTrace.Domain.Models;
global using IndTrace.Application.Repository;
global using IndTrace.Domain.Entities;
global using IndTrace.Domain.Entities.BarCodes;
global using IndTrace.Domain.Enum;
global using IndTrace.Domain.Interfaces;

// Use the test host Program (IndTrace.Monitor) to bootstrap DI for integration tests.
global using IndTrace.Persistence.DBContext;
global using IndTrace.Persistence.Interfaces;
global using Meziantou.Extensions.Logging.Xunit.v3;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.SignalR.Client;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using NSubstitute;
global using Shouldly;
global using Xunit;
global using Integration.Tests.Utilities;
// IndQuestResults global usings
global using IndQuestResults;
global using IndQuestResults.Collections;
global using IndQuestResults.Operations;
global using IndQuestResults.Performance;
global using IndQuestResults.Reactive;
global using IndQuestResults.Async;
global using IndQuestResults.Functional;
global using IndQuestResults.Validation;
