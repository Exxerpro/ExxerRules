// <copyright file="IEntityRoot.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces;

/// <summary>
/// Marker interface for aggregate root domain entities.
/// Classes implementing this interface are considered primary business entities
/// and are eligible for DbSet&lt;T&gt; registration in the EF Core DbContext.
/// </summary>
/// <remarks>
/// <para>
/// This interface serves as a marker to identify domain entities that represent
/// aggregate roots in the Domain-Driven Design sense. These are typically
/// the main business objects that encapsulate business logic and state.
/// </para>
/// <para>
/// Examples of entities that should implement IEntityRoot:
/// - Product, Customer, Order, Machine, BarCode, etc.
/// </para>
/// <para>
/// This interface is used by EF Core model validation to ensure only
/// valid domain entities are registered as DbSet&lt;T&gt; in the DbContext.
/// </para>
/// </remarks>
public interface IEntityRoot;

// Marker interface - no members required
