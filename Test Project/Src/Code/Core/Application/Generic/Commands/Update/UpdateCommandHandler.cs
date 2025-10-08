// <copyright file="UpdateCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Generic.Commands.Update
{
    public interface IUpdateCommand<TEntity> : IMonitorRequest<Result<bool>> where TEntity : class, new()
    {
        int EntitieId { get; set; }
    }

    public interface IUpdateCommandHandler<TCommand, TEntity>
        where TCommand : class, IUpdateCommand<TEntity>, new()
        where TEntity : class, new()
    {
        Task<Result<bool>> Update(TCommand command, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Represents the UpdateCommandHandler.
    /// </summary>
    public class UpdateCommandHandler<TCommand, TEntity> : IUpdateCommandHandler<TCommand, TEntity>
    where TCommand : class, IUpdateCommand<TEntity>, new()
    where TEntity : class, new()
    {
        private readonly IRepository<TEntity> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler{TCommand, TEntity}"/> class.
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public UpdateCommandHandler(IRepository<TEntity> repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Executes Update operation.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The result of Update.</returns>
        public async Task<Result<bool>> Update(TCommand command, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result<bool>.WithFailure("Operation was cancelled.", false);
            }

            if (command == null)
            {
                return Result<bool>.WithFailure("Command cannot be null.", false);
            }

            try
            {
                var entityResult = await this.repository.GetByIdAsync(command.EntitieId, cancellationToken).ConfigureAwait(false);

                if (entityResult.IsFailure || entityResult.Value == null)
                {
                    return false;
                }

                var updateResult = await this.repository.UpdateAsync(entityResult.Value, cancellationToken).ConfigureAwait(false);

                return updateResult.IsSuccess;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
