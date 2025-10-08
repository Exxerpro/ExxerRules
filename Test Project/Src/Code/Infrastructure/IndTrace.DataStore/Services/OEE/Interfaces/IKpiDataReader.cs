namespace IndTrace.DataStore.Services.OEE.Interfaces;

public interface IKpiDataReader
{
    Task<List<OeeResult>> GetKpiResultsAsync(CancellationToken cancellationToken);
}
