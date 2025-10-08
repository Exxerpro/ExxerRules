// <copyright file="DeleteCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Generic.Commands.Delete
{
    public interface IDeleteCommand<TEntity> : IMonitorRequest<bool>
        where TEntity : class, new()
    {
        int Id { get; set; }
    }

    public interface IDeleteCommandHandler<TCommand, TEntity>
        where TCommand : class, IDeleteCommand<TEntity>, new()
        where TEntity : class, new()
    {
        Task<Result<bool>> Remove(TCommand command, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Represents the DeleteCommandHandler.
    /// </summary>
    public class DeleteCommandHandler<TCommand, TEntity> : IDeleteCommandHandler<TCommand, TEntity>
        where TCommand : class, IDeleteCommand<TEntity>, new()
        where TEntity : class, new()
    {
        private readonly IRepository<TEntity> repository;

        public DeleteCommandHandler(IRepository<TEntity> repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <inheritdoc/>
        public async Task<Result<bool>> Remove(TCommand command, CancellationToken cancellationToken)
        {
            var entityResult = await this.repository.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
            if (entityResult.IsFailure)
            {
                return Result<bool>.WithFailure(entityResult.Errors);
            }

            if (entityResult.Value is null)
            {
                return Result<bool>.WithFailure($"No entity of type {typeof(TEntity).Name} with ID {command.Id} was found");
            }

            var deleteResult = await this.repository.DeleteAsync(entityResult.Value, cancellationToken).ConfigureAwait(false);
            if (deleteResult.IsFailure)
            {
                return Result<bool>.WithFailure(deleteResult.Errors);
            }

            return Result<bool>.Success(true);
        }
    }
}
