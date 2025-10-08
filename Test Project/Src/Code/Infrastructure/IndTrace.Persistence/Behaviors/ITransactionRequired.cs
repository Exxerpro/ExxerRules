namespace IndTrace.Persistence.Behaviors
{
    /// <summary>
    /// Marker interface that indicates a command or operation requires database transaction support.
    /// </summary>
    /// <remarks>
    /// This interface is used by the application infrastructure to identify operations that need
    /// to be executed within a database transaction context. Classes implementing this interface
    /// will automatically have their operations wrapped in transaction boundaries by the
    /// transaction behavior pipeline.
    ///
    /// This is a marker interface with no methods, following the marker interface pattern
    /// for metadata-driven behavior configuration.
    /// </remarks>
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Ensure interface is open for extension but closed for modification (OCP - SOLID). Consider using default interface methods or extension methods for future-proofing.
    internal interface ITransactionRequired
    {

    }
}
