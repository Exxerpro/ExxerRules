// <copyright file="CreateCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Generic.Commands.Create
{
    /// <summary>
    /// Interface for create command that creates a new entity.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be created.</typeparam>
    public interface ICreateCommand<TEntity> : IMonitorRequest<TEntity>
        where TEntity : class, new()
    {
        // Entity properties will be defined by implementing commands
        // This interface provides the contract for creation operations
    }

    /// <summary>
    /// Interface for create command handler.
    /// </summary>
    /// <typeparam name="TCommand">The command type implementing ICreateCommand.</typeparam>
    /// <typeparam name="TEntity">The entity type to be created.</typeparam>
    public interface ICreateCommandHandler<TCommand, TEntity> : IMonitorRequestHandler<TCommand, TEntity>
        where TCommand : class, ICreateCommand<TEntity>, new()
        where TEntity : class, new()
    {
        /// <summary>
        /// Creates a new entity based on the command.
        /// </summary>
        /// <param name="command">The create command containing entity data.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A result containing the created entity or error information.</returns>
        Task<Result<TEntity>> CreateAsync(TCommand command, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Generic handler for create commands that creates entities of any type.
    /// Follows the same pattern as other generic CRUD handlers in the application.
    /// </summary>
    /// <typeparam name="TCommand">The command type implementing ICreateCommand.</typeparam>
    /// <typeparam name="TEntity">The entity type to be created.</typeparam>
    public class CreateCommandHandler<TCommand, TEntity> : ICreateCommandHandler<TCommand, TEntity>
        where TCommand : class, ICreateCommand<TEntity>, new()
        where TEntity : class, new()
    {
        private readonly IRepository<TEntity> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler{TCommand, TEntity}"/> class.
        /// </summary>
        /// <param name="repository">The repository for entity operations.</param>
        public CreateCommandHandler(IRepository<TEntity> repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Creates a new entity based on the command.
        /// </summary>
        /// <param name="command">The create command containing entity data.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A result containing the created entity or error information.</returns>
        public async Task<Result<TEntity>> CreateAsync(TCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Create new entity instance
                var entity = new TEntity();

                // TODO: Map properties from command to entity
                // This would typically be handled by AutoMapper or similar mapping logic

                // Add entity to repository
                var addResult = await repository.AddAsync(entity, cancellationToken);

                if (!addResult.IsSuccess)
                {
                    return Result<TEntity>.WithFailure(addResult.Errors ?? ["Failed to add entity to repository"]);
                }

                return Result<TEntity>.Success(entity);
            }
            catch (Exception ex)
            {
                return Result<TEntity>.WithFailure([ex.Message]);
            }
        }

        /// <summary>
        /// Processes the command asynchronously (implements IMonitorRequestHandler interface).
        /// </summary>
        /// <param name="request">The create command request.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A result containing the created entity or error information.</returns>
        public async Task<Result<TEntity>> ProcessAsync(TCommand request, CancellationToken cancellationToken)
        {
            return await CreateAsync(request, cancellationToken);
        }
    }
}
