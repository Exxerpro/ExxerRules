using System;

namespace TestProject
{
    public class IncompleteValidationService
    {
        public void ProcessData(string input, object config, int? value)
        {
            // Only validate input
            ArgumentNullException.ThrowIfNull(input);
            
            // Use all parameters
            var length = input.Length;
            var type = config.GetType();
            var hasValue = value.HasValue;
        }
    }
}