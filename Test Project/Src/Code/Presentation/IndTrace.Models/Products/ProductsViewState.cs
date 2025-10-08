// <copyright file="ProductsViewState.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.Products;

using IndTrace.Application.ConfigStations.Queries.GetConfigStationList;
using IndTrace.Application.Models.Interfaces;
using IndTrace.Application.Products.Commands.Create;
using IndTrace.Application.Products.Events;
using IndTrace.Application.Products.Queries.GetProductDetail;
using IndTrace.Application.Products.Services;
using IndTrace.Application.Repositories;
using IndTrace.Application.RulesEngine.Dto;
using IndTrace.Application.WorkFlows.Dto;
using IndTrace.Domain.Models;
using MudBlazor;

/// <summary>
/// Manages the state and operations for product view including products, machines, and workflow interactions.
/// </summary>
public class ProductsViewState(ApplicationConfiguration applicationConfiguration)
{
    /// <summary>
    /// Gets the current view of products displayed in the interface.
    /// </summary>
    public List<ProductDefinition> ProductsView { get; private set; } = [];

    /// <summary>
    /// Gets the complete list of available products.
    /// </summary>
    public List<ProductDefinition> ProductsList { get; private set; } = [];

    /// <summary>
    /// Gets the collection of machine items associated with products.
    /// </summary>
    public List<ProductMachineItem> MachineItems { get; private set; } = [];

    /// <summary>
    /// Gets the mapping of machine IDs to machine names.
    /// </summary>
    public Dictionary<int, string> MachineNames { get; private set; } = new();

    private readonly ApplicationConfiguration applicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));

    /// <summary>
    /// Executes SetMachineNames operation.
    /// </summary>
    /// <param name="Dictionary<int">The Dictionary.<int.</param>
    /// <param name="machineNames">The machineNames.</param>
    public void SetMachineNames(Dictionary<int, string> machineNames)
    {
        if (machineNames.Count == 0)
        {
            return;
        }

        this.MachineNames.Clear();
        this.MachineNames = machineNames;
    }

    /// <summary>
    /// Handles the addition of a new product to the view state.
    /// </summary>
    /// <param name="newProduct">The new product to add.</param>
    public void NewProductAdded(ProductDto newProduct)
    {
        if (this.ProductsList.Count == 0)
        {
            this.ProductsList.AddRange(this.ProductsView);
        }

        this.ProductsView.Clear();
        this.ProductsView.Add(new ProductDefinition("Catalog", false, string.Empty));

        var newProductDefinition = new ProductDefinition(newProduct.ProductName, false, string.Empty);
        this.ProductsView.Add(newProductDefinition);

        this.ProductsView.Add(new ProductDefinition("Discard", false, string.Empty));
    }

    /// <summary>
    /// Handles product selection and updates the view accordingly.
    /// </summary>
    /// <param name="selected">The selected product.</param>
    public void ProductSelected(ProductDto selected)
    {
        this.ProductSelectedItem = selected;

        if (this.ProductsList.Count == 0)
        {
            this.ProductsList.AddRange(this.ProductsView);
        }

        this.ProductsView.Clear();
        this.ProductsView.Add(new ProductDefinition("Catalog", false, string.Empty));

        var selectedProduct = this.ProductsList.FirstOrDefault(p => p.Name == selected.ProductName);
        if (selectedProduct != null)
        {
            this.ProductsView.Add(selectedProduct);
        }

        this.ProductsView.Add(new ProductDefinition("Discard", false, string.Empty));
    }

    /// <summary>
    /// Gets or sets the currently selected product item.
    /// </summary>
    public ProductDto ProductSelectedItem { get; set; } = new();

    /// <summary>
    /// Generates a list of product definitions from product DTOs.
    /// </summary>
    /// <param name="products">The list of product DTOs to convert.</param>
    /// <returns>A list of product definitions.</returns>
    public static List<ProductDefinition> GenerateProductsList(List<ProductDto> products)
    {
        List<ProductDefinition> result = [new ProductDefinition("Catalog", false, string.Empty)];

        result.AddRange(products.Select(product =>
            new ProductDefinition(product.ProductName, false, string.Empty)));

        result.Add(new ProductDefinition("Discard", false, string.Empty));

        return result;
    }

    /// <summary>
    /// Generates product machine items from products and workflows.
    /// </summary>
    /// <param name="products">The list of products.</param>
    /// <param name="workflows">The workflows to process.</param>
    /// <param name="machineNames">The machine names mapping.</param>
    /// <returns>A collection of product machine items or null if machine names are not provided.</returns>
    public static IEnumerable<ProductMachineItem>? GenerateProductsItems(List<ProductDto> products, IEnumerable<WorkFlowDto> workflows, Dictionary<int, string>? machineNames)
    {
        if (machineNames is null || machineNames?.Count == 0)
        {
            return null;
        }

        var productIdToNameMap = MapProductIdsToNames(products);
        var machineOrderMap = new Dictionary<int, int>();

        var result = from workflow in workflows.Where(workflow => workflow.NextMachineId != 0)
                     let productName = GetProductName(workflow.ProductId, productIdToNameMap)
                     let order = GetMachineOrder(workflow.ProductId, machineOrderMap)
                     select CreateProductMachineItem(workflow, productName, order, machineNames);

        return result.ToList();
    }

    private static Dictionary<int, string> MapProductIdsToNames(List<ProductDto> products)
    {
        return products.ToDictionary(p => p.ProductId, p => p.ProductName);
    }

    private static string GetProductName(int productId, Dictionary<int, string> productIdToNameMap)
    {
        return productIdToNameMap.GetValueOrDefault(productId, "default");
    }

    private static int GetMachineOrder(int productId, Dictionary<int, int> machineOrderMap)
    {
        if (!machineOrderMap.TryAdd(productId, 0))
        {
            machineOrderMap[productId]++;
        }

        return machineOrderMap[productId];
    }

    private static ProductMachineItem? CreateProductMachineItem(WorkFlowDto workflow, string productName, int order, Dictionary<int, string>? machineNames)
    {
        if (machineNames is null || machineNames?.Count == 0)
        {
            return null;
        }

        return new ProductMachineItem(
            $"Machine {machineNames!.GetValueOrDefault(workflow.NextMachineId)}",
            productName,
            order,
            workflow.NextMachineId,
            machineNames!.GetValueOrDefault(workflow.NextMachineId),
            string.Empty);
    }

    /// <summary>
    /// Generates catalog machine items from workflows.
    /// </summary>
    /// <param name="workFlowList">The list of workflows.</param>
    /// <param name="machineNames">The machine names mapping.</param>
    /// <returns>A collection of catalog machine items.</returns>
    public static IEnumerable<ProductMachineItem> GenerateCatalogMachineItems(IEnumerable<WorkFlowDto> workFlowList, Dictionary<int, string>? machineNames)
    {
        var distinctMachineIds = GetDistinctMachineIds(machineNames);
        return CreateCatalogMachineItems(distinctMachineIds, machineNames);
    }

    private static HashSet<int> GetDistinctMachineIds(Dictionary<int, string>? machineNames)
    {
        return machineNames?
            .Keys
            .Where(id => id != 0)
            .ToHashSet() ?? [];
    }

    private static IEnumerable<ProductMachineItem> CreateCatalogMachineItems(HashSet<int> distinctMachineIds, Dictionary<int, string>? machineNames)
    {
        var result = new List<ProductMachineItem>();
        var order = 0;

        if (machineNames is null || machineNames?.Count == 0)
        {
            return result;
        }

        foreach (var machineId in distinctMachineIds.OrderBy(id => id))
        {
            result.Add(new ProductMachineItem(
                $"Machine {machineNames!.GetValueOrDefault(machineId)}",
                "Catalog",
                order,
                machineId,
                machineNames!.GetValueOrDefault(machineId),
                "Catalog"));
            order++;
        }

        return result;
    }

    /// <summary>
    /// Initializes machine items with products and workflows data.
    /// </summary>
    /// <param name="products">The list of products.</param>
    /// <param name="workflows">The workflows to process.</param>
    /// <param name="machineNames">The machine names mapping.</param>
    public void InitializeMachineItems(List<ProductDto> products, IEnumerable<WorkFlowDto> workflows, Dictionary<int, string>? machineNames)
    {
        this.MachineItems.Clear();
        var productItems = GenerateProductsItems(products, workflows, machineNames);
        var machineCatalog = GenerateCatalogMachineItems(workflows, machineNames);
        if (productItems is not null)
        {
            this.MachineItems.AddRange(productItems);
        }

        if (machineCatalog is not null)
        {
            this.MachineItems.AddRange(machineCatalog);
        }

        // Enrich ProductMachineItem instances with full DTOs using your extension methods
        foreach (var item in this.MachineItems)
        {
            var product = this.applicationConfiguration.Products.FirstOrDefault(p => p.PartNumber == item.Status);

            var machine = this.applicationConfiguration.Machines.FirstOrDefault(p => p.MachineId == item.MachineId);

            if (product is not null)
            {
                item.SetProduct(product);
            }

            if (machine is not null)
            {
                item.SetMachine(machine);
            }
        }
    }

    /// <summary>
    /// Initializes the products view with the provided products.
    /// </summary>
    /// <param name="products">The list of products to initialize.</param>
    public void InitializeProducts(List<ProductDto> products)
    {
        this.ProductsView.Clear();
        this.ProductsList.Clear();

        var generatedProducts = GenerateProductsList(products);
        if (products is not null)
        {
            this.ProductsView.AddRange(generatedProducts);
            this.ProductsList.AddRange(generatedProducts);
        }
    }

    /// <summary>
    /// Handles the drop operation for catalog items in the drag-and-drop interface.
    /// </summary>
    /// <param name="info">The drop information containing item and drop zone details.</param>
    public void HandleCatalogItemDrop(MudItemDropInfo<ProductMachineItem> info)
    {
        // Guards: info, item, and dropzone identifier must be present
        if (info is null || info.Item is null || info.DropzoneIdentifier is not { } dropzone)
        {
            return;
        }

        var existingItem = this.MachineItems.FirstOrDefault(item => item.Name == info.Item.Name && item.Status == dropzone);

        if (existingItem != null)
        {
            this.ReplaceExistingItem(info, existingItem);
        }
        else if (info.Item.Status != dropzone)
        {
            info.Item.Status = dropzone;
            this.AddNewCatalogItem(info);
        }
    }

    private void ReplaceExistingItem(MudItemDropInfo<ProductMachineItem> info, ProductMachineItem existingItem)
    {
        if (info.Item is null || info.DropzoneIdentifier is not { } dropzone)
        {
            return;
        }
        this.MachineItems.Remove(existingItem);

        if (!this.MachineNames.TryGetValue(info.Item.MachineId, out var machineName))
        {
            return;
        }
        var catalogItem = new ProductMachineItem(info.Item.Name, "Catalog", existingItem.IndexInZone, info.Item.MachineId, machineName, string.Empty);
        var productItem = new ProductMachineItem(info.Item.Name, dropzone, existingItem.IndexInZone, info.Item.MachineId, machineName, string.Empty);

        this.MachineItems.AddIfNotExist(catalogItem);
        this.MachineItems.AddIfNotExist(productItem);
    }

    /// <summary>
    /// Handles dropping a catalog item into the discard zone.
    /// </summary>
    /// <param name="info">The drop information containing item and drop zone details.</param>
    public void HandleCatalogItemDropIntoDiscard(MudItemDropInfo<ProductMachineItem> info)
    {
        if (info.Item is null)
        {
            return;
        }
        this.RemoveItemFromState(info.Item);

        if (!this.MachineNames.TryGetValue(info.Item.MachineId, out var machineName))
        {
            return;
        }

        var newItem = new ProductMachineItem(info.Item.Name, "Catalog", info.IndexInZone, info.Item.MachineId, machineName, string.Empty);
        this.MachineItems.InsertIfNotExist(info.IndexInZone, newItem);
    }

    private void AddNewCatalogItem(MudItemDropInfo<ProductMachineItem> info)
    {
        if (info.Item is null)
        {
            return;
        }
        if (!this.MachineNames.TryGetValue(info.Item.MachineId, out var machineName))
        {
            return;
        }

        var newItem = new ProductMachineItem(info.Item.Name, "Catalog", info.IndexInZone, info.Item.MachineId, machineName, string.Empty);
        this.MachineItems.InsertIfNotExist(info.IndexInZone, newItem);
    }

    /// <summary>
    /// Removes an item from the current state.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    public void RemoveItemFromState(ProductMachineItem item)
    {
        var itemToRemove = this.MachineItems.FirstOrDefault(x => x.Name == item.Name && x.Status == item.Status);
        if (itemToRemove != null)
        {
            this.MachineItems.Remove(itemToRemove);
        }
    }

    /// <summary>
    /// Adds a new machine to the specified product section.
    /// </summary>
    /// <param name="section">The product section to add the machine to.</param>
    public void AddMachine(ProductDefinition section)
    {
        this.MachineItems.Add(new ProductMachineItem(section.NewMachineName, section.Name, this.MachineItems.Count, 0, "None", string.Empty));
        section.NewMachineName = string.Empty;
        section.NewMachine = false;
    }

    /// <summary>
    /// Deletes a product section and updates the view accordingly.
    /// </summary>
    /// <param name="section">The section to delete.</param>
    public void DeleteSection(ProductDefinition section)
    {
        if (this.IsLastSection())
        {
            this.ClearViewAndItems();
        }
        else
        {
            this.UpdateMachineStatusesAfterSectionDeletion(section);
        }
    }

    private bool IsLastSection()
    {
        return this.ProductsView.Count == 1;
    }

    private void ClearViewAndItems()
    {
        this.MachineItems.Clear();
        this.ProductsView.Clear();
    }

    private void UpdateMachineStatusesAfterSectionDeletion(ProductDefinition section)
    {
        var newIndex = this.GetNewIndexAfterDeletion(section);
        this.ProductsView.Remove(section);
        this.UpdateMachineStatuses(section.Name, this.ProductsView[newIndex].Name);
    }

    private int GetNewIndexAfterDeletion(ProductDefinition section)
    {
        var newIndex = this.ProductsView.IndexOf(section) - 1;
        return newIndex < 0 ? 0 : newIndex;
    }

    private void UpdateMachineStatuses(string oldStatus, string newStatus)
    {
        var machines = this.MachineItems.Where(x => x.Status == oldStatus);
        foreach (var machineItem in machines)
        {
            machineItem.Status = newStatus;
        }
    }

    /// <summary>
    /// Adds a new product using the product service.
    /// </summary>
    /// <param name="productService">The product service to use for creation.</param>
    /// <param name="productDto">The product data to create.</param>
    /// <returns>A task representing the result of the product creation operation.</returns>
    public async Task<Result<ProductCreatedEvent>> AddProduct(IProductService productService, ProductDto productDto)
    {
        // Workflows now is commming from the productDto
        // var workFlowDtos = CreateProductCommand.CreateWorkFlowDtos(productDto.Machines);

        // Step 3: Construct the RuleDto
        var ruleDto = new RuleDto
        {
            RuleJson = "{\"ruleId\": \"V3\",\"ruleFunction\": [\"lineIdentifier\", \"lineNumber\", \"fixedPart\", \"partNumber\", \"lastTwoYearDigits\", \"julianDay\", \"autoIncrement\"],\"components\": {\"lineIdentifier\": {\"action\": \"string\",\"origin\": \"fixed\",\"value\": \"QA\"},\"lineNumber\": {\"action\": \"string\",\"origin\": \"fixed\",\"value\": \"4\"},\"fixedPart\": {\"action\": \"string\",\"origin\": \"fixed\",\"value\": \"5\"},\"partNumber\": {\"action\": \"string\",\"origin\": \"program\",\"lengthMin\": 6,\"lengthMax\": 9},\"lastTwoYearDigits\": {\"action\": \"lastTwoYearDigits\",\"origin\": \"program\"},\"julianDay\": {\"action\": \"julianDay\",\"origin\": \"program\"},\"autoIncrement\": {\"action\": \"numeric\",\"origin\": \"program\",\"length\": 4,\"incremental\": true}}}",
            Name = "WS100",
            Description = "CHSML",
            Version = 3,
            IsActive = true,
        };

        // Step 4: Construct the Recipe (Example)
        var recipe = new RecipeDto
        {
            CycleTimeMinimum = 0,
            CycleTimeMaximum = 216000,
            ProductId = 1,
        };

        // Step 5: Create the ProductCreationDto
        var productCreationDto = new ProductCreationDto
        {
            Product = productDto,
            Machines = productDto.Machines,
            Rule = ruleDto,
            Recipe = recipe,
        };

        // TODO VALIDATE BEFORE SENDING THE EXECUTE COMMAND WE HAVE ALL THE DATA NEEDED
        // I THINK WE DON'HAVE THE CUSTOMER SOME WAY

        // Step 11: Execute the final monitorRequest using the service
        var result = await productService.ExecuteCreateProductCommand(productCreationDto);

        return result;
    }

    /// <summary>
    /// Sets the selected customer for the current context.
    /// </summary>
    /// <param name="selectedCustomer">The name of the selected customer.</param>
    public void SetSelectedCustomer(string selectedCustomer)
    {
        this.SelectedCustomer = selectedCustomer;

        // var customer = Customers.FirstOrDefault(p => p.Name == _newProduct.CustomerName);

        // if (customer is null || customer.CustomerId <= 0)
        // {
        //    return;
        // }

        // _newProduct.CustomerId = customer.CustomerId;
    }

    /// <summary>
    /// Gets or sets the name of the currently selected customer.
    /// </summary>
    public string SelectedCustomer { get; set; } = string.Empty;
}
