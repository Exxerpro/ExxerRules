// <copyright file="AppDetailsFactory.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Configuration.Services;

using IndTrace.Application.MachinesPlcs.Queries.GetMachinesList;

/// <summary>
/// Factory service responsible for creating comprehensive application configuration objects.
/// </summary>
/// <remarks>
/// This factory aggregates data from multiple repositories to build a complete ApplicationConfiguration
/// object containing PLCs, machines, products, workflows, customers, and OEE configuration.
/// It handles complex data relationships and mappings required for the IndTrace application.
/// </remarks>
public class AppDetailsFactory(
    IRepository<ConfigApp> configAppRepository,
    IRepository<Plc> plcRepository,
    IRepository<MachinePlc> machinePlcRepository,
    IRepository<WorkFlow> workflowRepository,
    IRepository<Machine> machineRepository,
    IRepository<Variable> variableRepository,
    IRepository<Customer> customerRepository,
    IRepository<VariablesGroup> variablesGroupRepository,
    IRepository<Product> productRepository,
    IIsOeeEnabledChecker isOeeEnabledChecker,
    ILogger<AppDetailsFactory> logger)
{
    /// <summary>
    /// Creates a complete application configuration object by aggregating data from multiple repositories.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A comprehensive ApplicationConfiguration object containing all system configuration data.</returns>
    /// <remarks>
    /// This method loads and correlates data from:
    /// - PLCs and their associated machines
    /// - Machine-Product compatibility mappings
    /// - Customer and product relationships
    /// - Workflow configurations
    /// - OEE settings and capabilities
    /// The method uses specifications to filter active/enabled entities and handles errors gracefully.
    /// </remarks>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    public async Task<ApplicationConfiguration> CreateAppDetailsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating AppDetails");

        // Specification for PLCs with Enabled == 1
        var plcSpecification = new Specification<Plc>(plc => plc.Enabled == 1);

        // Specification for Machine PLCs with IsActive == 1
        var machinePlcSpecification = new Specification<MachinePlc>(machinePlc => machinePlc.IsActive == 1);

        // Specification for Machines with EnableAppTraceability == 1 and EnableBypassTraceability == 0
        var configAppSpec = new Specification<ConfigApp>(_ => true)
            .AddOrderByDescending(c => c.AppId);

        var configApp = await configAppRepository.FirstOrDefaultAsync(configAppSpec, cancellationToken).ConfigureAwait(false);

        if (configApp.IsFailure)
        {
            logger.LogError("Error getting ConfigApp: {@Errors}", configApp.Errors);
        }

        var plcList = await plcRepository.
            ListAsync(plcSpecification, cancellationToken).ConfigureAwait(false);

        if (plcList.IsFailure)
        {
            logger.LogError("Error getting plcList: {@Errors}", plcList.Errors);
        }

        var machinePlcList = await machinePlcRepository.
            ListAsync(machinePlcSpecification, cancellationToken).ConfigureAwait(false);
        if (machinePlcList.IsFailure)
        {
            logger.LogError("Error getting machinePlcList: {@Errors}", machinePlcList.Errors);
        }

        var oeeConfigurationResult = await isOeeEnabledChecker.CheckOeeFeatureByMachineIdsAsync(
            machinePlcList.Value?.Select(mp => mp.MachineId).ToList() ?? [], cancellationToken).ConfigureAwait(false);

        if (oeeConfigurationResult.IsFailure)
        {
            logger.LogError("Error getting OeeConfigurationResult: {@Errors}", oeeConfigurationResult.Errors);
        }

        var customersList = await customerRepository.
             ListAsync(cancellationToken).ConfigureAwait(false);
        if (customersList.IsFailure)
        {
            logger.LogError("Error getting customersList: {@Errors}", customersList.Errors);
        }

        var machineList = await machineRepository.
            ListAsync(cancellationToken).ConfigureAwait(false);

        Dictionary<int, string> machinesNames = [];

        if (machineList is { IsFailure: true, Value: not null })
        {
            logger.LogError("Error getting machineList: {@Errors}", machineList.Errors);
        }
        else if (machineList.Value is not null)
        {
            machinesNames = machineList.Value
                .ToDictionary(m => m.MachineId, m => m.Name);
        }

        // Add a default entry for "None" with ID 0
        machinesNames.TryAdd(0, "None");

        var workflowList = await workflowRepository.
                ListAsync(cancellationToken).ConfigureAwait(false);
        if (workflowList.IsFailure)
        {
            logger.LogError("Error getting workflowList: {@Errors}", workflowList.Errors);
        }

        var productList = await productRepository.
            ListAsync(cancellationToken).ConfigureAwait(false);
        if (productList.IsFailure)
        {
            logger.LogError("Error getting productList: {@Errors}", productList.Errors);
        }

        var variablesList = await variableRepository.
            ListAsync(cancellationToken).ConfigureAwait(false);
        if (variablesList.IsFailure)
        {
            logger.LogError("Error getting variablesList: {@Errors}", variablesList.Errors);
        }

        var variablesGroupList = await variablesGroupRepository.
            ListAsync(cancellationToken).ConfigureAwait(false);
        if (variablesGroupList.IsFailure)
        {
            logger.LogError("Error getting variablesGroupList: {@Errors}", variablesGroupList.Errors);
        }

        var plcDtos = this.MapPlcListToPlcDtos(
            plcList.Value ?? [],
            machinePlcList.Value ?? [],
            machineList.Value ?? [],
            variablesList.Value ?? [],
            variablesGroupList.Value ?? [],
            oeeConfigurationResult.Value ?? new OeeConfiguration());

        var activeCustomer = this.GetActiveCustomer(
            customersList.Value?.Select(c => CustomerDto.ToDto(c)).Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!) ?? [],
            productList.Value?.Select(p =>
            {
                var result = ProductDto.ToDto(p);
                return result.IsSuccess && result.Value is not null ? result.Value : null;
            }).Where(p => p is not null).Cast<ProductDto>() ?? []);

        // Create a dictionary of machines names and a dictionary of products names for the UI to hande special cases
        var monitorPrinters = this.GetMonitorPrinterProductMapping(
            machineList.Value?.Select(m => MachineDto.ToDto(m)).Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!) ?? [],
            productList.Value?.Select(p => ProductDto.ToDto(p)).Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!) ?? [],
            activeCustomer?.Value ?? []);

        List<MachineProductMap> machineProductMap = [];

        if (workflowList.IsSuccess && productList.IsSuccess && customersList.IsSuccess && machineList.IsSuccess &&
            workflowList.Value is not null && productList.Value is not null && customersList.Value is not null && machineList.Value is not null)
        {
            machineProductMap = (
                from w in workflowList.Value
                join p in productList.Value on w.ProductId equals p.ProductId
                join c in customersList.Value on p.CustomerId equals c.CustomerId
                from mId in new[] { w.LastMachineId, w.NextMachineId }
                join m in machineList.Value on mId equals m.MachineId into machines
                from m in machines.DefaultIfEmpty()
                where mId != 0
                select new MachineProductMap
                {
                    MachineId = m?.MachineId ?? mId,
                    MachineName = m?.Name ?? "Unknown",
                    ProductId = p.ProductId,
                    PartNumber = p.PartNumber,
                    CustomerId = c.CustomerId,
                    CustomerName = c.Name,
                    WorkFlowId = w.WorkFlowId,
                    LastMachineId = w.LastMachineId,
                    NextMachineId = w.NextMachineId,
                }).Distinct().ToList();
        }
        else
        {
            logger.LogWarning("Skipping MachineProductCompatibility population due to data load failure.");
        }

        return new ApplicationConfiguration
        {
            WorkFlows = workflowList.Value?.Select(w => WorkFlowDto.ToDto(w)).Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!) ?? [],
            MachinePlcs = machinePlcList.Value?.Select(mp => MachinePlcDto.ToDto(mp)).Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!) ?? [],
            Machines = machineList.Value?.Select(m => MachineDto.ToDto(m)).Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!) ?? [],
            Products = productList.Value?.Select(p => ProductDto.ToDto(p)).Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!) ?? [],
            Customers = customersList.Value?.Select(c => CustomerDto.ToDto(c)).Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!) ?? [],
            ActiveCustomer = activeCustomer?.Value ?? [],
            MachineProductCompatibility = machineProductMap,
            Plcs = plcDtos,
            MachineNames = machinesNames,
            ConfigApp = configApp.Value is not null ? (ConfigAppDto.ToDto(configApp.Value).Value ?? new ConfigAppDto()) : new ConfigAppDto(),
            LineConfiguration = monitorPrinters,
            OeeConfiguration = oeeConfigurationResult.Value ?? new OeeConfiguration(),
        };
    }

    /// <summary>
    /// Maps a collection of PLC entities to PLC DTOs with their associated machines, variables, and OEE configuration.
    /// </summary>
    /// <param name="plcList">The collection of PLC entities to map.</param>
    /// <param name="machinePlcList">The machine-PLC relationship entities.</param>
    /// <param name="machineList">The collection of machine entities.</param>
    /// <param name="variablesList">The collection of variable entities.</param>
    /// <param name="variablesGroupList">The collection of variable group entities.</param>
    /// <param name="oeeConfiguration">The OEE configuration settings.</param>
    /// <returns>A list of PlcDto objects with complete mapping data.</returns>
    public List<PlcDto> MapPlcListToPlcDtos(
        IEnumerable<Plc> plcList,
        IEnumerable<MachinePlc> machinePlcList,
        IEnumerable<Machine> machineList,
        IEnumerable<Variable> variablesList,
        IEnumerable<VariablesGroup> variablesGroupList,
        OeeConfiguration oeeConfiguration)
    {
        return plcList.Select(plc => this.MapPlcToPlcDto(plc, machinePlcList, machineList, variablesList, variablesGroupList, oeeConfiguration)).ToList();
    }

    /// <summary>
    /// Maps a single PLC entity to a PLC DTO with its associated machines, variables, and configuration.
    /// </summary>
    /// <param name="plc">The PLC entity to map.</param>
    /// <param name="machinePlcList">The machine-PLC relationship entities.</param>
    /// <param name="machineList">The collection of machine entities.</param>
    /// <param name="variablesList">The collection of variable entities.</param>
    /// <param name="variablesGroupList">The collection of variable group entities.</param>
    /// <param name="oeeConfiguration">The OEE configuration settings.</param>
    /// <returns>A fully mapped PlcDto object.</returns>
    private PlcDto MapPlcToPlcDto(
        Plc plc,
        IEnumerable<MachinePlc> machinePlcList,
        IEnumerable<Machine> machineList,
        IEnumerable<Variable> variablesList,
        IEnumerable<VariablesGroup> variablesGroupList,
        OeeConfiguration oeeConfiguration)
    {
        var plcDtoResult = PlcDto.ToDto(plc);
        if (plcDtoResult.IsSuccess && plcDtoResult.Value is not null)
        {
            var plcDto = plcDtoResult.Value;
            plcDto.HasOeeEnabled = oeeConfiguration.Enabled &&
                                   oeeConfiguration.EnabledByMachine.GetValueOrDefault(plc.MachineId, false);

            plcDto.Machines = this.GetMachinesForPlc(plc.PlcId, machinePlcList, machineList);
            var plcVariables = this.GetActiveVariablesForPlc(plc.PlcId, variablesList);

            plcDto.Variables = this.MapVariablesToDictionary(plcVariables);
            plcDto.VariablesGroups = this.MapVariablesGroupsToDictionary(variablesGroupList);

            plcDto.Perfomances = this.MapVariablesToRegisters(plcVariables, TagsGroups.PerformanceTags.Value);
            plcDto.Registers = this.MapVariablesToRegisters(plcVariables, TagsGroups.RegisterTags.Value);
            plcDto.References = this.MapVariablesToRegisters(plcVariables, TagsGroups.ReferenceTags.Value);

            return plcDto;
        }
        else
        {
            // Return a default PlcDto if conversion fails
            return new PlcDto();
        }
    }

    /// <summary>
    /// Retrieves all machines associated with a specific PLC.
    /// </summary>
    /// <param name="plcId">The ID of the PLC to find machines for.</param>
    /// <param name="machinePlcList">The machine-PLC relationship entities.</param>
    /// <param name="machineList">The collection of all machines.</param>
    /// <returns>A collection of machines associated with the specified PLC.</returns>
    private IEnumerable<Machine> GetMachinesForPlc(
        int plcId,
        IEnumerable<MachinePlc> machinePlcList,
        IEnumerable<Machine> machineList)
    {
        var machineIds = machinePlcList.Where(mp => mp.PlcId == plcId).Select(mp => mp.MachineId).ToList();
        return machineList.Where(m => machineIds.Contains(m.MachineId)).ToList();
    }

    /// <summary>
    /// Retrieves all active variables for a specific PLC.
    /// </summary>
    /// <param name="plcId">The ID of the PLC to find variables for.</param>
    /// <param name="variablesList">The collection of all variables.</param>
    /// <returns>A collection of active variables for the specified PLC.</returns>
    private IEnumerable<Variable> GetActiveVariablesForPlc(
        int plcId,
        IEnumerable<Variable> variablesList)
    {
        return variablesList.Where(v => v.PlcId == plcId && v.IsActive == 1).ToList();
    }

    /// <summary>
    /// Maps variables to a dictionary with variable names as keys.
    /// </summary>
    /// <param name="plcVariables">The collection of variables to map.</param>
    /// <returns>A dictionary mapping variable names to Variable objects.</returns>
    /// <remarks>
    /// If multiple variables have the same name, the first one is used.
    /// </remarks>
    private Dictionary<string, Variable> MapVariablesToDictionary(
        IEnumerable<Variable> plcVariables)
    {
        return plcVariables.GroupBy(v => v.Name).ToDictionary(g => g.Key, g => g.FirstOrDefault()!);
    }

    /// <summary>
    /// Maps variables groups to a dictionary with group names as keys.
    /// </summary>
    /// <param name="variablesGroupList">The collection of variable groups to map.</param>
    /// <returns>A dictionary mapping group names to VariablesGroup objects.</returns>
    private Dictionary<string, VariablesGroup> MapVariablesGroupsToDictionary(
        IEnumerable<VariablesGroup> variablesGroupList)
    {
        return variablesGroupList.ToDictionary(vg => vg.VariableGroupName);
    }

    /// <summary>
    /// Maps variables to registers based on their variable group ID.
    /// </summary>
    /// <param name="plcVariables">The collection of variables to map.</param>
    /// <param name="variableGroupId">The ID of the variable group to filter by.</param>
    /// <returns>A dictionary mapping variable names to Register objects.</returns>
    private Dictionary<string, Register> MapVariablesToRegisters(
     IEnumerable<Variable> plcVariables, int variableGroupId)
    {
        return plcVariables
            .Where(v => v.VariableGroupId == variableGroupId)
            .GroupBy(v => v.Name)
            .ToDictionary(
                g => g.Key,
                g => new Register
                {
                    RegisterId = g.First().VariableId,
                    Name = g.First().Name,
                    Description = g.First().Description,
                    MachineId = g.First().MachineId,
                    VariableId = g.First().VariableId,
                    CycleId = 0,
                    Value = g.First().Value,
                    DataType = g.First().NetType,
                    StatusValueId = 0,
                },
                StringComparer.Ordinal);
    }

    /// <summary>
    /// Determines which customers are currently active based on their running products.
    /// </summary>
    /// <param name="customer">The collection of all customers.</param>
    /// <param name="products">The collection of all products.</param>
    /// <returns>A Result containing the collection of active customers.</returns>
    /// <remarks>
    /// A customer is considered active if they have at least one product with IsActive = 1.
    /// </remarks>
    public Result<IEnumerable<CustomerDto>> GetActiveCustomer(
        IEnumerable<CustomerDto> customer,
        IEnumerable<ProductDto> products)
    {
        var activeCustomers = customer
            .Select(c =>
            {
                // Match products to customer
                var hasRunningProduct = products.Any(p =>
                    p.CustomerId == c.CustomerId &&
                    p.IsActive == 1);

                return new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    HasProductRunningOnLine = hasRunningProduct,
                };
            })
            .Where(c => c.HasProductRunningOnLine);

        return Result<IEnumerable<CustomerDto>>.Success(activeCustomers);
    }

    /// <summary>
    /// Builds a monitor mapping object linking initial printers to their associated products and customers.
    /// Ensures runtime null-safety and structured output for monitoring apps.
    /// </summary>
    /// <param name="machines">A non-null list of machines to evaluate.</param>
    /// <param name="products">A non-null list of products to associate with printers.</param>
    /// <returns>A populated <see cref="MainLineConfiguration"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if machines or products are null at runtime.</exception>
    private MainLineConfiguration GetMonitorPrinterProductMapping(
        IEnumerable<MachineDto> machines,
        IEnumerable<ProductDto> products,
        IEnumerable<CustomerDto> activeCustomers)
    {
        if (machines is null || products is null || activeCustomers is null)
        {
            logger.LogWarning("Machine list or product list is null. Returning empty monitor mapping.");
            return new MainLineConfiguration();
        }

        var monitorMapping = new MainLineConfiguration();

        var machineList = machines.ToList(); // Materialize early
        var productList = products.ToList();

        var initialPrinters = machineList
            .Where(m => m.MachineType == MachineType.InitialPrinter)
            .ToList();

        monitorMapping.ActiveCustomer = activeCustomers;
        monitorMapping.HasMultipleInitialPrinters = initialPrinters.Count > 1;
        monitorMapping.InitialPrinterIds = initialPrinters.Select(p => p.MachineId).ToList();

        monitorMapping.DictClientsProducts = productList
            .GroupBy(p => p.PartNumber)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => p.CustomerName)
                      .Distinct()
                      .ToList());

        monitorMapping.HasMultipleClients = monitorMapping.DictClientsProducts.Count > 1;

        monitorMapping.Associations = (from printer in initialPrinters
                                       from product in productList
                                       select new PrinterProductAssociation
                                       {
                                           MachineId = printer.MachineId,
                                           MachineName = printer.Name,
                                           MachineType = printer.MachineType,
                                           ProductId = product.ProductId,
                                           PartNumber = product.PartNumber,
                                           CustomerName = product.CustomerName,
                                       }).ToList();

        return monitorMapping;
    }
}
