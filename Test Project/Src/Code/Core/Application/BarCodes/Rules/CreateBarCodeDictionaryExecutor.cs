// <copyright file="CreateBarCodeDictionaryExecutor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Rules
{
    /// <summary>
    /// Executes barcode creation rules using a dictionary-based approach and a date/time provider.
    /// </summary>
    /// <summary>
    /// Executes barcode creation rules using a dictionary-based approach and a date/time provider.
    /// </summary>
    public class CreateBarCodeDictionaryExecutor
    {
        private readonly IDateTimeMachine dateTimeMachineProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateBarCodeDictionaryExecutor"/> class.
        /// </summary>
        /// <param name="dateTimeMachineProvider">The date/time machine provider.</param>
        public CreateBarCodeDictionaryExecutor(IDateTimeMachine dateTimeMachineProvider)
        {
            this.dateTimeMachineProvider = dateTimeMachineProvider;
        }

        /// <summary>
        /// Dictionary to store actions for different component types.
        /// </summary>
        public readonly Dictionary<string, Func<RuleFragment, string, int, IndQuestResults.Result<string?>>> ComponentActions = [];

        /// <summary>
        /// Gets or sets the rule to be used for barcode creation.
        /// </summary>
        public Rule? Rule { get; set; }

        /// <summary>
        /// Parses a rule from its JSON representation.
        /// </summary>
        /// <param name="ruleJson">The JSON string representing the rule.</param>
        /// <returns>The parsed <see cref="Rule"/> object, or null if parsing fails.</returns>
        public Rule? ParseRuleFromJson(string ruleJson)
        {
            try
            {
                // [Fix]
                // CLAUDE
                // Date: 20/08/2025
                // Reason: Add validation for empty/whitespace JSON to return null for invalid input
                if (string.IsNullOrWhiteSpace(ruleJson))
                {
                    return null;
                }

                var ruleNode = JsonNode.Parse(ruleJson);
                if (ruleNode is null)
                {
                    return null;
                }

                // Parse Rule ID (supports numeric or string). String non-numeric IDs are stored in Rule.Name
                int parsedRuleId = 0;
                string? parsedRuleName = null;
                var ruleIdNode = ruleNode["ruleId"];
                if (ruleIdNode is System.Text.Json.Nodes.JsonValue ruleIdValue)
                {
                    if (ruleIdValue.TryGetValue<int>(out var idInt))
                    {
                        parsedRuleId = idInt;
                    }
                    else if (ruleIdValue.TryGetValue<string>(out var idStr))
                    {
                        if (int.TryParse(idStr, out var idParsed))
                        {
                            parsedRuleId = idParsed;
                        }
                        else
                        {
                            parsedRuleName = idStr;
                        }
                    }
                }

                // Parse RuleFunction
                var ruleFunction = ruleNode["ruleFunction"]?.AsArray()
                    .Select(x => x?.GetValue<string>() ?? string.Empty)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList() ?? [];

                // [Fix]
                // CLAUDE
                // Date: 20/08/2025
                // Reason: Return null if required ruleFunction is missing or empty (invalid JSON structure)
                if (!ruleFunction.Any())
                {
                    return null;
                }

                // Parse Components
                var componentsNode = ruleNode["components"]?.AsObject();
                var components = new List<RuleFragment>();
                if (componentsNode != null)
                {
                    foreach (var kvp in componentsNode)
                    {
                        var comp = kvp.Value?.AsObject();
                        if (comp == null)
                        {
                            continue;
                        }

                        components.Add(new RuleFragment
                        {
                            Name = kvp.Key,
                            Action = comp["action"]?.GetValue<string>() ?? string.Empty,
                            Origin = comp["origin"]?.GetValue<string>() ?? string.Empty,
                            Value = comp["value"]?.GetValue<string>() ?? string.Empty,
                            LengthMin = comp["lengthMin"]?.GetValue<int?>(),
                            LengthMax = comp["lengthMax"]?.GetValue<int?>(),
                            Length = comp["length"]?.GetValue<int?>(),
                            Incremental = comp["incremental"]?.GetValue<bool?>() ?? false,
                        });
                    }
                }

                this.Rule = new Rule
                {
                    RuleId = parsedRuleId,
                    Name = parsedRuleName ?? string.Empty,
                    RuleFunction = ruleFunction,
                    Components = components,
                    IsActive = true,
                };

                return this.Rule;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing rule JSON: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Initializes the component actions dictionary based on the rule's components.
        /// </summary>
        /// <summary>
        /// Initializes the component actions dictionary based on the rule's components using functional Result semantics.
        /// </summary>
        /// <returns>Result indicating success or failure with an explanatory error.</returns>
        public IndQuestResults.Result InitializeComponentActions()
        {
            if (this.Rule == null)
            {
                return IndQuestResults.Result.WithFailure("Rule is not set");
            }

            this.ComponentActions.Clear();

            foreach (var componentName in this.Rule.RuleFunction)
            {
                var ruleFragment = this.Rule.Components.FirstOrDefault(c => c.Name == componentName);
                if (ruleFragment != null)
                {
                    Func<RuleFragment, string, int, IndQuestResults.Result<string?>>? action = componentName switch
                    {
                        "lineIdentifier" or "lineNumber" or "partNumber" or "fixedPart" => this.StringAction,
                        "julianDay" => this.JulianDayAction,
                        "lastTwoYearDigits" => this.LastTwoYearDigitsAction,
                        "autoIncrement" => this.NumericAction,
                        _ => null,
                    };
                    if (action is null)
                    {
                        return IndQuestResults.Result.WithFailure($"Unknown action for component: {componentName}");
                    }

                    this.ComponentActions[componentName] = action;
                }
            }

            return IndQuestResults.Result.Success();
        }

        /// <summary>
        /// Applies the rule to create a barcode string based on the part number and consecutive number.
        /// </summary>
        /// <param name="partNumber">The part number to be included in the barcode.</param>
        /// <param name="consecutive">The auto-incrementing number to be included in the barcode.</param>
        /// <returns>The constructed barcode string, or an error message if the rule is invalid.</returns>
        /// <summary>
        /// Applies the rule to create a barcode string based on the part number and consecutive number using functional Result semantics.
        /// </summary>
        /// <param name="partNumber">The part number to be included in the barcode.</param>
        /// <param name="consecutive">The auto-incrementing number to be included in the barcode.</param>
        /// <returns>Result with constructed barcode string, or failure with error message.</returns>
        public IndQuestResults.Result<string> ApplyRuleCreateBarCode(string partNumber, int consecutive)
        {
            if (this.dateTimeMachineProvider == null)
            {
                return IndQuestResults.Result<string>.WithFailure("DateTime provider not available");
            }

            if (this.Rule is not { IsActive: true })
            {
                return IndQuestResults.Result<string>.WithFailure("invalid rule for label");
            }

            var barcode = new StringBuilder();

            // Iterate through the rule functions in order
            foreach (var componentName in this.Rule.RuleFunction)
            {
                if (this.ComponentActions.TryGetValue(componentName, out var action))
                {
                    var ruleFragment = this.Rule.Components.FirstOrDefault(c => c.Name == componentName);
                    if (ruleFragment != null)
                    {
                        var result = action(ruleFragment, partNumber, consecutive);
                        if (result.IsFailure)
                        {
                            return IndQuestResults.Result<string>.WithFailure(result.Errors);
                        }

                        if (result.Value != null)
                        {
                            barcode.Append(result.Value);
                        }
                    }
                }
                else
                {
                    return IndQuestResults.Result<string>.WithFailure($"No action configured for component: {componentName}");
                }
            }

            return IndQuestResults.Result<string>.Success(barcode.ToString());
        }

        /// <summary>
        /// Processes string-based components for barcode creation.
        /// </summary>
        /// <param name="component">The rule fragment representing the component.</param>
        /// <param name="partNumber">The part number to be included in the barcode.</param>
        /// <param name="consecutive">The auto-incrementing number to be included in the barcode.</param>
        /// <returns>The processed string value for the component.</returns>
        private IndQuestResults.Result<string?> StringAction(RuleFragment component, string partNumber, int consecutive)
        {
            if (component.Origin == "fixed")
            {
                return IndQuestResults.Result<string?>.Success(component.Value);
            }

            if (component.Origin == "program")
            {
                var lengthMin = component.LengthMin ?? 6;
                var lengthMax = component.LengthMax ?? 9;

                var result = partNumber.Length switch
                {
                    var length when length < lengthMin => partNumber.PadLeft(lengthMin, '0'),
                    var length when length >= lengthMin && length <= lengthMax => partNumber,
                    var length when length > lengthMax => partNumber.Substring(0, lengthMax),
                    _ => null,
                };
                return result is null
                    ? IndQuestResults.Result<string?>.WithFailure("Invalid partNumber length.")
                    : IndQuestResults.Result<string?>.Success(result);
            }

            return IndQuestResults.Result<string?>.Success(null);
        }

        /// <summary>
        /// Processes components representing the last two digits of the year.
        /// </summary>
        /// <param name="component">The rule fragment representing the component.</param>
        /// <param name="partNumber">The part number to be included in the barcode.</param>
        /// <param name="consecutive">The auto-incrementing number to be included in the barcode.</param>
        /// <returns>The last two digits of the year as a string.</returns>
        private IndQuestResults.Result<string?> LastTwoYearDigitsAction(RuleFragment component, string partNumber, int consecutive)
        {
            if (component.Origin == "program")
            {
                return IndQuestResults.Result<string?>.Success(this.dateTimeMachineProvider.Year.ToString().Substring(2, 2));
            }

            return IndQuestResults.Result<string?>.Success(null);
        }

        /// <summary>
        /// Processes components representing the Julian day of the year.
        /// </summary>
        /// <param name="component">The rule fragment representing the component.</param>
        /// <param name="partNumber">The part number to be included in the barcode.</param>
        /// <param name="consecutive">The auto-incrementing number to be included in the barcode.</param>
        /// <returns>The Julian day of the year as a string.</returns>
        private IndQuestResults.Result<string?> JulianDayAction(RuleFragment component, string partNumber, int consecutive)
        {
            if (component.Origin == "program")
            {
                return IndQuestResults.Result<string?>.Success(this.dateTimeMachineProvider.DayOfYear.ToString("D3"));
            }

            return IndQuestResults.Result<string?>.Success(null);
        }

        /// <summary>
        /// Processes numeric components for barcode creation.
        /// </summary>
        /// <param name="component">The rule fragment representing the component.</param>
        /// <param name="partNumber">The part number to be included in the barcode.</param>
        /// <param name="consecutive">The auto-incrementing number to be included in the barcode.</param>
        /// <returns>The processed numeric value for the component.</returns>
        private IndQuestResults.Result<string?> NumericAction(RuleFragment component, string partNumber, int consecutive)
        {
            if (component.Origin == "fixed")
            {
                return IndQuestResults.Result<string?>.Success(component.Value);
            }

            if (component.Origin == "program")
            {
                var length = component.Length ?? 4;
                var paddedNumber = (consecutive % Math.Pow(10, length)).ToString().PadLeft(length, '0');
                return IndQuestResults.Result<string?>.Success(paddedNumber);
            }

            return IndQuestResults.Result<string?>.Success(null);
        }
    }
}
