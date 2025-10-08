// <copyright file="EnumLookupAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Enum.Attributes
{
    using System;

    /// <summary>
    /// Indicates that the EnumModel should have a generated lookup table provider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class EnumLookupAttribute : Attribute
    {
    }
}
