// <copyright file="CycleUpdateStrategyFactory.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Services;

using IndTrace.Application.Cycles.Services.Strategies;

/// <summary>
/// Factory for creating cycle update strategies.
/// </summary>
public class CycleUpdateStrategyFactory : ICycleUpdateStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CycleUpdateStrategyFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public CycleUpdateStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public ICycleUpdateStrategy CreateStrategy(CycleStatus cycleStatus)
    {
        return cycleStatus == CycleStatus.FinishedOk
            ? _serviceProvider.GetRequiredService<OkUpdateStrategy>()
            : _serviceProvider.GetRequiredService<NotOkUpdateStrategy>();
    }
}
