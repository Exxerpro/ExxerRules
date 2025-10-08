namespace IndTrace.Domain.UnitTests.OEESsTests;

public static class OeeTestCases
{
    /// <summary>
    /// Represents the AllOeeTestCases.
    /// </summary>
    public class AllOeeTestCases : IEnumerable<object[]>
    {
        /// <summary>
        /// Executes GetEnumerator operation.
        /// </summary>
        /// <returns>The result of GetEnumerator.</returns>
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var item in OeeTestCasesNormalCases.GetWarningCases())
                yield return item;
            foreach (var item in OeeTestCasesEdgeCases.GetEdgeCasesWithErrors())
                yield return item;
            // Add more as needed
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
