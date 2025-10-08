using IndTrace.Application.ConfigStations.Queries.GetConfigStationList;
using IndTrace.Application.Configuration.Services;
using IndTrace.Application.Products.Queries.GetProductDetail;
using IndTrace.UI.Models.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace IndTrace.Monitor.Pages;

/// <summary>
/// Represents the WorkFlows page component that manages product workflow configurations.
/// </summary>
public partial class WorkFlows
{
    /// <summary>
    /// Gets or sets the application configuration.
    /// </summary>
    [Parameter]
    public ApplicationConfiguration? ApplicationConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the product state. TODO remember initialize when ApplicationConfiguration has valid data.
    /// </summary>
    [Parameter]
    public ProductsViewState? ProductState { get; set; }

    /// <summary>
    /// Gets or sets the index state for tracking item positions.
    /// </summary>
    [Parameter]
    public Dictionary<string, Dictionary<ProductMachineItem, int>>? IndexState { get; set; }

    /// <summary>
    /// Gets or sets the dictionary of machine names keyed by machine ID.
    /// </summary>
    [Parameter]
    public Dictionary<int, string> MachineNames { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the component has been initialized.
    /// </summary>
    public bool IsInitialized { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the save button is enabled.
    /// </summary>
    public bool EnableSaveButton { get; set; }

    private MudDropContainer<ProductMachineItem>? dropContainer;

    private readonly NewProductForm newProductSectionModel = new("new section");

    /// <summary>
    /// Initializes the component asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        var initialization = await this.InitializeApplicationAsync();
        if (this.ApplicationConfiguration is null) return;

        this.IsInitialized = initialization;
    }

    /// <summary>
    /// Called after the component has been rendered.
    /// </summary>
    /// <param name="firstRender">True if this is the first render of the component.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (this.IsInitialized) return;

        var initialization = await this.InitializeApplicationAsync();
        if (this.ApplicationConfiguration is null) return;

        this.ProductState?.InitializeProducts(this.ApplicationConfiguration.Products.ToList());
        this.ProductState?.InitializeMachineItems(this.ApplicationConfiguration.Products.ToList(), this.ApplicationConfiguration.WorkFlows.ToList(), this.MachineNames);

        this.IsInitialized = initialization;
    }

    /// <summary>
    /// Initializes the application configuration asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns true if initialization succeeded.</returns>
    private async Task<bool> InitializeApplicationAsync()
    {
        if (this.ApplicationConfiguration is not null) return true;
        var result = await this.IndTraceConfigurationService.GetConfigurationAsync();
        if (result is { IsSuccess: true, Value: not null })
        {
            this.ApplicationConfiguration = result.Value;

            this.MachineNames = [];
            this.MachineNames = this.ApplicationConfiguration.MachineNames;

            this.ProductState?.SetMachineNames(this.MachineNames);

            this.ProductState?.InitializeProducts(this.ApplicationConfiguration.Products.ToList());
            this.ProductState?.InitializeMachineItems(this.ApplicationConfiguration.Products.ToList(), this.ApplicationConfiguration.WorkFlows.ToList(), this.MachineNames);

            this.ProductState?.SetMachineNames(this.MachineNames);
        }
        else
        {
            // Error loading configuration details - handled by ApplicationConfiguration null check
        }

        return this.ApplicationConfiguration is not null;
    }

    /// <summary>
    /// Saves the new model when triggered by a mouse event.
    /// </summary>
    /// <param name="obj">The mouse event arguments.</param>
    private void SaveNewModel(MouseEventArgs obj)
    {
        if (this.dropContainer is null) return;
        foreach (var item in this.dropContainer.Items)
        {
            this.Logger.LogInformation(
                "Item: {Name} Status: {Status} MachineId: {MachineId} MachineName: {MachineName}",
                item.Name, item.Status, item.MachineId, item.MachineName);
        }

        this.Logger.LogInformation(this.dropContainer.Items.Count().ToString());

        var mudDropZones = this.dropContainer.GetDropZones<ProductMachineItem>();

        foreach (var (zone, dropZone) in mudDropZones)
        {
            var indices = dropZone.GetIndexs();
            foreach (var (item, index) in indices)
            {
                this.Logger.LogInformation("  Item: {Name}, Status: {Status}, Zone: {Zone}, MachineId: {MachineId}, Index: {Index}", item.Name, item.Status, zone, item.MachineName, index);
            }
        }
    }

    /// <summary>
    /// Handles the product update when an item is dropped.
    /// </summary>
    /// <param name="info">The drop information containing item and zone details.</param>
    private void ProductUpdated(MudItemDropInfo<ProductMachineItem> info)
    {
        if (info.Item is null) return;
        info.Item.SourceZoneIdentifier = info.DropzoneIdentifier;
        // ProcessAsync item dropped events
        switch (info.DropzoneIdentifier)
        {
            case "Catalog":
                this.OnItemDroppedIntoCatalog(info);
                break;

            case "Discard":
                this.OnItemDroppedIntoDiscard(info);
                break;

            default:
                this.OnItemDroppedIntoProduct(info);
                break;
        }
    }

    /// <summary>
    /// Handles the event when a new product is added.
    /// </summary>
    /// <param name="newProduct">The new product that was added.</param>
    private void NewProductAdded(ProductDto newProduct)
    {
        this.ProductState?.NewProductAdded(newProduct);
        this.EnableSaveButton = true;
        // Optionally, refresh the UI to reflect the changes
        this.InvokeAsync(this.StateHasChanged);
    }

    /// <summary>
    /// Handles the selection of a product.
    /// </summary>
    /// <param name="selectedProduct">The selected product.</param>
    public void ProductSelected(ProductDto selectedProduct)
    {
        this.ProductState?.ProductSelected(selectedProduct);

        // Optionally, refresh the UI to reflect the changes
        this.InvokeAsync(this.StateHasChanged);
    }

    /// <summary>
    /// Handles when an item is dropped into a product zone.
    /// </summary>
    /// <param name="info">The drop information containing item and zone details.</param>
    private void OnItemDroppedIntoProduct(MudItemDropInfo<ProductMachineItem> info)
    {
        if (info.Item is null) return;
        if (info.Item.Status == "Catalog")
        {
            this.ProductState?.HandleCatalogItemDrop(info);
        }

        this.LogDropContainerState();
        this.UpdateIndexStateAndRefreshUI();
    }

    /// <summary>
    /// Logs the current state of the drop container for debugging purposes.
    /// </summary>
    private void LogDropContainerState()
    {
        if (this.dropContainer is null) return;
        if (this.dropContainer is null) return;
        foreach (var item in this.dropContainer.Items)
        {
            this.Logger.LogInformation(
                "Item: {ItemName} Status: {Status} MachineId: {MachineId} MachineName: {MachineName}",
                item.Name, item.Status, item.MachineId, item.MachineName);
        }

        this.Logger.LogInformation(this.dropContainer.Items.Count().ToString());

        if (this.dropContainer is null) return;
        var mudDropZones = this.dropContainer.GetDropZones<ProductMachineItem>();

        foreach (var (zone, dropZone) in mudDropZones)
        {
            var indices = dropZone.GetIndexs();
            foreach (var (item, index) in indices)
            {
                this.Logger.LogInformation("  Item: {Name}, Status: {Status}, Zone: {Zone}, MachineId: {MachineId}, Index: {Index}", item.Name, item.Status, zone, item.MachineId, index);
            }
        }
    }

    /// <summary>
    /// Updates the index state and refreshes the UI.
    /// </summary>
    private void UpdateIndexStateAndRefreshUI()
    {
        if (this.dropContainer is null) return;
        var mudDropZones = this.dropContainer.GetDropZones<ProductMachineItem>();
        var state = new Dictionary<string, Dictionary<ProductMachineItem, int>>();

        foreach (var (zone, dropZone) in mudDropZones)
        {
            var indices = dropZone.GetIndexs();
            state.Add(zone, indices);
        }

        this.ProductState?.MachineItems.RemoveDuplicates();

        this.IndexState = state;
        this.InvokeAsync(this.StateHasChanged);
    }

    /// <summary>
    /// Handles when an item is dropped into the discard zone.
    /// </summary>
    /// <param name="info">The drop information containing item and zone details.</param>
    private void OnItemDroppedIntoDiscard(MudItemDropInfo<ProductMachineItem> info)
    {
        if (info.Item is null) return;
        if (info.Item.Status != "Catalog")
        {
            this.ProductState?.RemoveItemFromState(info.Item);
        }
        else
        {
            this.ProductState?.HandleCatalogItemDropIntoDiscard(info);
        }

        this.InvokeAsync(this.StateHasChanged);
    }

    /// <summary>
    /// Handles when an item is dropped into the catalog zone.
    /// </summary>
    /// <param name="info">The drop information containing item and zone details.</param>
    private void OnItemDroppedIntoCatalog(MudItemDropInfo<ProductMachineItem> info)
    {
        // No specific action required for the catalog drop
    }

    /// <summary>
    /// Handles the submission of a valid section form.
    /// </summary>
    /// <param name="context">The edit context of the form.</param>
    private void OnValidSectionSubmit(EditContext context)
    {
        this.ProductState?.ProductsView.Add(new ProductDefinition(this.newProductSectionModel.Name, false, string.Empty));
        this.newProductSectionModel.Name = string.Empty;
        // Section is closed automatically after form submission
    }

    /// <summary>
    /// Opens the add new product section.
    /// </summary>
    private void OpenAddNewProduct()
    {
        // Add new product section opening logic would be handled by UI state
    }

    /// <summary>
    /// Adds a machine to the specified product section.
    /// </summary>
    /// <param name="section">The product definition section to add the machine to.</param>
    private void AddMachine(ProductDefinition section)
    {
        var productDefinition = new ProductDefinition(section.Name, section.NewMachine, section.NewMachineName);
        this.ProductState?.AddMachine(section);
        this.dropContainer?.Refresh();
    }

    /// <summary>
    /// Deletes the specified product section.
    /// </summary>
    /// <param name="section">The product definition section to delete.</param>
    private void DeleteSection(ProductDefinition section)
    {
        var productDefinition = new ProductDefinition(section.Name, section.NewMachine, section.NewMachineName);
        this.ProductState?.DeleteSection(section);
    }

    /// <summary>
    /// Determines whether the specified item can be dropped.
    /// </summary>
    /// <param name="item">The product machine item to check.</param>
    /// <returns>True if the item can be dropped, otherwise false.</returns>
    private bool CanDrop(ProductMachineItem item)
    {
        return true;
    }
}
