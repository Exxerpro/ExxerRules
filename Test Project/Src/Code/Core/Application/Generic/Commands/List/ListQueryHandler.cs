// <copyright file="ListQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Generic.Commands.List
{
    public interface TCommandList
    {
        string[] Includes { get; set; }

        int Page { get; set; }

        int PageSize { get; set; }
    }

    public interface IListQuery<TCommand, TResponse> : IMonitorRequestHandler<TCommand, TResponse>
            where TCommand : IMonitorRequest<TResponse>, TCommandList, new()
            where TResponse : class, new()
    {
        IEnumerable<TResponse> Entities { get; set; }

        public Task<Result<IEnumerable<TResponse>>> ListProcessAsync(TCommand command, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Represents the ListQueryHandler.
    /// </summary>
    public class ListQueryHandler<TCommand, TResponse>(IReadOnlyRepository<TResponse> repository) : IListQuery<TCommand, TResponse>
            where TCommand : class, IMonitorRequest<TResponse>, TCommandList, new()
            where TResponse : class, new()
    {
        private readonly IReadOnlyRepository<TResponse> repository = repository ?? throw new ArgumentNullException(nameof(repository));

        /// <summary>
        /// Gets or sets the Includes.
        /// </summary>
        public string[] Includes { get; set; } = [];

        /// <summary>
        /// Gets or sets the Page.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the PageSize.
        /// </summary>
        public int PageSize { get; set; }

        public const int PageSizeMin = 10;
        public const int PageSizeMax = 100;

        /// <summary>
        /// Gets or sets the Entities.
        /// </summary>
        public IEnumerable<TResponse> Entities { get; set; } = [];

        /// <summary>
        /// Executes ListProcessAsync operation.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The result of ListProcessAsync.</returns>
        public async Task<Result<IEnumerable<TResponse>>> ListProcessAsync(TCommand command, CancellationToken cancellationToken)
        {
            this.Page = command.Page > 0 ? command.Page : 1;

            // [Fix]
            // CLAUDE
            // Date: 24/08/2025
            // Reason: [LOGIC BUG FIX] - Fixed PageSize calculation logic. When PageSize <= 0, use PageSizeMin. When PageSize > PageSizeMax, use PageSizeMax.
            this.PageSize = command.PageSize > 0 ?
                (command.PageSize > PageSizeMax ? PageSizeMax : command.PageSize) :
                PageSizeMin;

            // Use repository's ListAsync method
            var repositoryResult = await this.repository.ListAsync(cancellationToken).ConfigureAwait(false);

            if (repositoryResult.IsFailure)
            {
                return Result<IEnumerable<TResponse>>.WithFailure(repositoryResult.Errors);
            }

            if (repositoryResult.Value is null)
            {
                return Result<IEnumerable<TResponse>>.WithFailure($"No entities of type {typeof(TResponse).Name} were found");
            }

            // Apply pagination in memory (for simple cases - consider using specification pattern for complex scenarios)
            var paginatedResult = repositoryResult.Value
                .Skip((this.Page - 1) * this.PageSize)
                .Take(this.PageSize)
                .ToList();

            return Result<IEnumerable<TResponse>>.Success(paginatedResult);
        }

        /// <summary>
        /// Processes the list query request asynchronously.
        /// </summary>
        /// <param name="request">The list query request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A Result containing the list of responses or failure information.</returns>
        public async Task<Result<TResponse>> ProcessAsync(TCommand request, CancellationToken cancellationToken)
        {
            var listResult = await this.ListProcessAsync(request, cancellationToken);
            if (listResult.IsFailure)
            {
                return Result<TResponse>.WithFailure(listResult.Errors);
            }

            // For list operations, we typically return the first item or a summary
            // This is a placeholder implementation - adjust based on your needs
            var firstItem = listResult.Value?.FirstOrDefault();
            if (firstItem == null)
            {
                return Result<TResponse>.WithFailure("No items found in the list");
            }

            return Result<TResponse>.Success(firstItem);
        }
    }
}
