namespace IndTrace.Application.Cycles.Commands.UpdateCyclesNok;

/// <summary>
/// Name-preserving handler that delegates to the unified SRP handler.
/// </summary>
public class UpdateCyclesNotOkCommandHandler : IGatewayRequestHandler<UpdateCyclesNotOkCommand, TaskGatewayResponse>, IResettable
{
    private readonly UpdateCyclesCommandHandler _unified;

    public UpdateCyclesNotOkCommandHandler(UpdateCyclesCommandHandler unified)
    {
        _unified = unified;
    }

    public Task<Result<TaskGatewayResponse>> ProcessAsync(UpdateCyclesNotOkCommand command, CancellationToken cancellationToken)
        => _unified.ProcessAsync(command, cancellationToken);

    public bool TryReset() => true;
}
