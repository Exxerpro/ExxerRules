namespace IndTrace.Models.UnitTests.S7.Rx
{
    // The class under test
    /// <summary>
    /// Represents the PlcBatchWriter.
    /// </summary>
    public class PlcBatchWriter
    {
        private readonly IS7MultiVarFactory _factory;
        private readonly IValueConverter _converter;
        private readonly ILogger _logger;
        private const int MaxRetryDepth = 2;

        /// <summary>
        /// A function to parse and verify variable addresses.  Must be set before use.
        /// The function takes an alias and a type, and returns a VariableAddress.
        /// The caller is responsible for ensuring the alias is valid and the type is compatible.
        /// </summary>
        public Func<string, Type, VariableAddress>? ParseAndVerify;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="logger">The logger.</param>

        public PlcBatchWriter(IS7MultiVarFactory factory, IValueConverter converter, ILogger logger)
        {
            _factory = factory;
            _converter = converter;
            _logger = logger;

            //Default value for ParseAndVerify:
            //The user must to provide their own value

            //ParseAndVerify = (alias, type) => new VariableAddress
            //{
            //    Operand = 1,
            //    Type = 1,
            //    DbNo = 1,
            //    Start = 0,
            //    BufferLength = 4
            //};
        }

        /// <summary>
        /// Executes WriteBatchValuesPlcAsync operation.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <param name="token">The token.</param>
        /// <returns>The result of WriteBatchValuesPlcAsync.</returns>

        public async Task<bool> WriteBatchValuesPlcAsync(IEnumerable<PlcBatchWriteTag> tags, CancellationToken token = default)
        {
            if (ParseAndVerify is null)
            {
                return false;
            }
            var parsedTags = tags.Select(t => (t, ParseAndVerify(t.Alias, t.Type))).ToList();
            return await WriteBatchCoreAsync(parsedTags, 0, token);
        }

        private async Task<bool> WriteBatchCoreAsync(List<(PlcBatchWriteTag Tag, VariableAddress Address)> batch, int depth, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var multiVar = _factory.Create();
            foreach (var (tag, address) in batch)
            {
                var buffer = new byte[address.BufferLength];
                if (!_converter.TryWriteToBuffer(buffer, tag.Value, tag.Type, address))
                {
                    _logger.LogError("Invalid type");
                    return false;
                }

                multiVar.Add(address.Operand, address.Type, address.DbNo, address.Start, buffer.Length, ref buffer);
            }

            var result = multiVar.Write();
            if (result == 0) return true;

            if (depth >= MaxRetryDepth || batch.Count <= 1)
            {
                _logger.LogError("Write failed at depth {Depth}", depth);
                return false;
            }

            int mid = batch.Count / 2;
            var left = batch.GetRange(0, mid);
            var right = batch.GetRange(mid, batch.Count - mid);

            bool l = await WriteBatchCoreAsync(left, depth + 1, token);
            bool r = await WriteBatchCoreAsync(right, depth + 1, token);

            return l && r;
        }
    }
}
