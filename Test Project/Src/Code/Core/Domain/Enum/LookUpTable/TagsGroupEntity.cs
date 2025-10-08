// <copyright file="TagsGroupEntity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Enum.LookUpTable;

using IndTrace.Domain.Enum.Attributes;

/// <summary>
/// Represents the TagsGroupEntity.
/// </summary>
[EnumLookup]
public class TagsGroupEntity(int id, string name, string displayName) : EnumLookUpTable(id, name, displayName);
