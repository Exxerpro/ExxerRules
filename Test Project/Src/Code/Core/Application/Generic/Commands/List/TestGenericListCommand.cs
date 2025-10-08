// <copyright file="TestGenericListCommand.cs" company="IndTrace">
// Copyright (c) IndTrace. All rights reserved.
// </copyright>

namespace IndTrace.Application.Generic.Commands.List
{
    /// <summary>
    /// Generic test command for list operations in production code.
    /// Moved from test infrastructure to be available across the application.
    /// Used for I²TDD (Interface Infrastructure Test Driven Development) validation.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public class TestGenericListCommand<TEntity> :
        IMonitorRequest<TEntity>,
        TCommandList
        where TEntity : class, new()
    {
        /// <summary>
        /// Gets or sets the page number for pagination.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Gets or sets the page size for pagination.
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the includes for entity relationships.
        /// </summary>
        public string[] Includes { get; set; } = Array.Empty<string>();
    }
}
