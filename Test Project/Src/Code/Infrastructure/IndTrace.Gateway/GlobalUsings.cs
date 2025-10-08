// <copyright file="GlobalUsings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Text.Json;
global using Gateway.Extensions;
global using Gateway.Helpers;
global using IndTrace.Application.BarCodes.Queries.GetBarCodeGatewayDetail;
global using IndTrace.Application.ConfigStations.Queries.GetConfigStationList;
global using IndTrace.Application.Configuration.Services;
global using IndTrace.Application.Plcs.Queries.GetDetail;
global using IndTrace.HubConnection.Abstractions;
global using IndTrace.HubConnection.Extensions;
global using IndTrace.Domain.Entities;
global using IndTrace.Domain.Enum;
global using IndTrace.Domain.Interfaces;
global using IndTrace.Domain.Models;
global using IndTrace.S7Rx.Interfaces;
global using IndTrace.S7Rx.Models;
global using Microsoft.AspNetCore.SignalR.Client;
global using static Gateway.Gateway.GatewayConstants;
global using Result = IndQuestResults.Result;

// IndQuestResults global usings
global using IndQuestResults;
global using IndQuestResults.Collections;
global using IndQuestResults.Operations;
global using IndQuestResults.Performance;
global using IndQuestResults.Reactive;
global using IndQuestResults.Async;
global using IndQuestResults.Functional;
global using IndQuestResults.Validation;
