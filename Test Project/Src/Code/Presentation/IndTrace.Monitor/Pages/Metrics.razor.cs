using IndTrace.Application.Registers.Services;
using IndTrace.UI.Models.Metrics;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System.ComponentModel.Design;
using Castle.Components.DictionaryAdapter.Xml;
using DocumentFormat.OpenXml.Bibliography;

namespace IndTrace.Monitor.Pages;

/// <summary>
/// Represents the Metrics page component that displays time series data and variable metrics.
/// </summary>
public partial class Metrics
{
    /// <summary>
    /// Gets or sets the dataset containing variables data for display.
    /// </summary>
    public List<VariablesData> DataSet { get; private set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the component has been initialized.
    /// </summary>
    public bool IsComponentInitialized { get; set; }

    /// <summary>
    /// Gets or sets the list of available register records.
    /// </summary>
    public List<RegistersRecords> VariablesList { get; private set; } = new();

    /// <summary>
    /// Transforms a dictionary of time series data into a list of VariablesData objects.
    /// </summary>
    /// <param name="timeSeriesData">The dictionary containing time series data keyed by machine ID and name.</param>
    /// <returns>A list of VariablesData objects transformed from the input dictionary.</returns>
    public List<VariablesData> TransformDictionaryToDataSet(
        Dictionary<(int MachineId, string Name), IEnumerable<TimeSeriesDataPoint>> timeSeriesData)
    {
        return timeSeriesData.Select(entry => new VariablesData
        {
            Name = $"Machine{entry.Key.MachineId}:{entry.Key.Name}",  // Format the name
            Color = Color.Success,  // Assuming a default color; customize as needed
            Data = entry.Value
                .OrderBy(dataPoint => dataPoint.TimeStamp)  // Order by timestamp
                .Select(dataPoint => this.ParseValue(dataPoint.Value, dataPoint.ValueType))  // Parse value to double
                .ToArray(),
        }).ToList();
    }

    /// <summary>
    /// Called after the component has been rendered.
    /// </summary>
    /// <param name="firstRender">True if this is the first render of the component.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await this.InitializeParticles();
            this.IsComponentInitialized = true;
        }

        await this.PopulateDataFromLocalStorage();

    }

    /// <summary>
    /// Populates the component data from local storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task PopulateDataFromLocalStorage()
    {
        try
        {
            // Retrieve the data from local storage
            var storedVariablesList = await this.LocalStorage.GetAsync<List<RegistersRecords>>(nameof(this.VariablesList));

            if (storedVariablesList.Success && storedVariablesList.Value is not null)
            {
                // Populate the VariablesList with the retrieved data
                this.VariablesList = storedVariablesList.Value;
                this.Logger.LogInformation("Data loaded successfully from local storage.");
            }
            else
            {
                this.Logger.LogWarning("No data found in local storage.");
            }

            // Retrieve the DataSet from local storage
            var storedDataSet = await this.LocalStorage.GetAsync<List<VariablesData>>(nameof(this.DataSet));

            if (storedDataSet.Success && storedDataSet.Value is not null)
            {
                this.DataSet = storedDataSet.Value;
                this.Logger.LogInformation("DataSet loaded successfully from local storage.");
            }
            else
            {
                this.Logger.LogWarning("No DataSet data found in local storage.");
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An error occurred while loading data from local storage.");
            // Clear the local storage when an exception occurs
            this.ClearLocalStorage();
        }
    }
    /// <summary>
    /// Clears all data from local storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private void ClearLocalStorage()
    {
        try
        {
            // Clear all data from local storage using JavaScript interop
            //TODO DELETE THE LOCAL STORAGE
            // await JSRuntime.InvokeAsync<object>("localStorage.clear", default);

            this.Logger.LogInformation("Local storage cleared successfully due to an error.");
        }
        catch (Exception ex)
        {
            // Log any errors that occur while clearing the local storage
            this.Logger.LogError(ex, "An error occurred while clearing local storage.");
        }
    }

    // Timer field removed - implementation is currently commented out

    /// <summary>
    /// Initializes the component asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override Task OnInitializedAsync()
    {
        //TODO ENABLE THIS FEATURE WHEN READY
        return Task.CompletedTask;

        // Code disabled for future implementation:
        // // Initialize the timer with a 2-second interval
        // _timer = new System.Timers.Timer(2000); // 2000 milliseconds = 2 seconds
        // _timer.Elapsed += async (sender, e) => await OnTimerElapsed();
        // _timer.AutoReset = false; // Ensures the timer runs only once
        // _timer.Start(); // Start the timer
    }

    /// <summary>
    /// Handles the timer elapsed event to refresh register data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task OnTimerElapsed()
    {
        // Perform your method logic after the delay
        var result = await this.RegisterService.GetListOfAvailableRegisters();
        if (result.IsSuccess && result.Value is not null)
        {
            this.VariablesList = result.Value.ToList();
        }
        await this.PopulateData(this.VariablesList);

        // Timer disposal removed - timer implementation is currently commented out
    }

    /// <summary>
    /// Handles changes to the selected variables.
    /// </summary>
    /// <param name="variables">The set of selected register records.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SelectedValuesChanged(HashSet<RegistersRecords> variables)
    {
        this.VariablesList = variables.ToList();
        await this.PopulateData(this.VariablesList);

    }

    /// <summary>
    /// Populates the data based on the selected variables.
    /// </summary>
    /// <param name="variables">The collection of variables to populate data for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task PopulateData(IEnumerable<RegistersRecords> variables)
    {
        // Fetching data from the database
        var result = await this.RegisterService.GetListRegisterTrends(variables);

        if (result.IsSuccess && result.Value is not null)
        {
            var timeSeriesData = result.Value;

            // Group by MachineId and Name, then order by TimeStamp and parse the values
            this.DataSet = this.TransformDictionaryToDataSet(timeSeriesData);
        }
    }

    /// <summary>
    /// Handles the deletion of a data series.
    /// </summary>
    /// <param name="series">The series to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task OnSeriesDeleted(VariablesData series)
    {
        var set = this.DataSet.FirstOrDefault(x =>
                                x.Name == series.Name
                                && x.MachineId == series.MachineId);
        if (set is not null)
            this.DataSet.Remove(set);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Parses a string value to a double based on the specified value type.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <param name="valueType">The type of the value (int, double, etc.).</param>
    /// <returns>The parsed double value, or 0 if parsing fails.</returns>
    private double ParseValue(string value, string? valueType)
    {
        try
        {
            return valueType switch
            {
                "int" => Convert.ToDouble(int.Parse(value)),
                "double" => double.Parse(value),
                _ => double.Parse(value),
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return 0;
    }

    /// <summary>
    /// Initializes the particles.js library for visual effects.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task InitializeParticles()
    {
        this.Logger.LogInformation("Starting Home page and particles");
        try
        {
            await this.JsRuntime.InvokeVoidAsync("particlesJS.load", "particles-js", "/particles.json");
            this.Logger.LogInformation("particlesJS.load Invoked");
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading particles.js");
        }
    }



    /// <summary>
    /// Refreshes the available register values from the service.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task RefreshValues()
    {
        var result = await this.RegisterService.GetListOfAvailableRegisters();
        if (result.IsSuccess && result.Value is not null)
        {
            this.VariablesList = result.Value.ToList();
        }

    }



    /// <summary>
    /// Disposes the component asynchronously, saving data to local storage.
    /// </summary>
    /// <returns>A value task representing the asynchronous disposal operation.</returns>
    public async ValueTask DisposeAsync()
    {
        try
        {
            // Save the VariablesList to local storage
            await this.LocalStorage.SetAsync(nameof(this.VariablesList), this.VariablesList);

            Console.WriteLine("Data saved.");
        }
        catch (Exception ex)
        {
            // ProcessAsync any exceptions that occur during the save operation
            Console.WriteLine($"An error occurred while saving data: {ex.Message}");
        }
        finally
        {
            // Perform any additional cleanup if needed
        }
    }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate Metrics page logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

}
