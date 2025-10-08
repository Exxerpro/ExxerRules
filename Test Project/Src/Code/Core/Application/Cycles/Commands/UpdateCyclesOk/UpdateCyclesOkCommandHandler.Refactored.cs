namespace IndTrace.Application.Cycles.Commands.UpdateCyclesOk;

/// <summary>
/// Name-preserving handler that delegates to the unified SRP handler.
/// </summary>
public class UpdateCyclesOkCommandHandler : IGatewayRequestHandler<UpdateCyclesOkCommand, TaskGatewayResponse>, IResettable
{
    private readonly UpdateCyclesCommandHandler _unified;

    public UpdateCyclesOkCommandHandler(UpdateCyclesCommandHandler unified)
    {
        _unified = unified;
    }

    public Task<Result<TaskGatewayResponse>> ProcessAsync(UpdateCyclesOkCommand command, CancellationToken cancellationToken)
        => _unified.ProcessAsync(command, cancellationToken);

    public bool TryReset() => true;
}
