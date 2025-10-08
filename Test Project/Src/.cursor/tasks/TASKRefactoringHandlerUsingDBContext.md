---
description:
globs:
alwaysApply: false
---

# Task > Refactoring Task for Obsolete Handlers and CRUD Patterns Using the Machine Entity Pattern

## Refactoring Strategy for Obsolete Handlers and CRUD Patterns Using the Machine Entity Pattern

This guide provides a systematic approach for refactoring obsolete handlers and standardizing CRUD operations across the codebase using the updated "Machine" entity as a reference implementation.

### Sample of [Obsolete] Handlers Identified

#### Create Command Handlers
- CreateConfigAppCommandHandler
- CreateConfigStationCommandHandler
- CreateMachinePlcCommandHandler
- CreatePlcCommandHandler
- CreateSettingCommandHandler
- CreateVariableCommandHandler
- CreateWorkFlowCommandHandler

#### Update Command Handlers
- UpdateConfigAppCommandHandler
- UpdateConfigStationCommandHandler
- UpdatePlcCommandHandler
- UpdateProductCommandHandler
- UpdateSettingCommandHandler
- UpdateShiftCommandHandler
- UpdateVariableCommandHandler
- UpdateWorkFlowCommandHandler

#### Query Handlers (List/Detail/Reports)
- GetBarCodeDetailMonitorGuiQueryHandler
- GetBarCodeDetailQueryQrCodeHandler
- GetBarCodeReportQueryHandler
- GetBarCodesLabelHandler
- GetConfigAppsDetailQueryHandler
- GetConfigAppsListQueryHandler
- GetConfigStationDetailQueryHandler
- GetConfigStationListQueryHandler
- GetCyclesDetailQueryHandler
- GetCyclesListQueryHandler
- GetMachinePlcDetailQueryHandler
- GetMaquinasConfigQueryHandler
- GetPlcDetailQueryHandler
- GetProductDetailQueryHandler
- GetReportesFilterInfoGuiQueryHandler
- GetReportsListGuiQueryHandler
- GetSettingDetailQueryHandler
- GetSettingsListQueryHandler
- GetShiftDetailQueryHandler
- GetShiftsListQueryHandler
- GetVariableDetailQueryHandler
- GetVariableListQueryHandler
- GetWorkFlowDetailQueryHandler

### Common Characteristics of Obsolete Handlers

- Direct usage of `DbContext`
- Embedded LINQ queries in the handler
- Lack of separation between business logic and data access
- Minimal use of cancellation tokens or asynchronous patterns

## Refactor Strategy: Use Repository Pattern

### Updated Handler Principles

- Dependency injection of repository interfaces
- Clear separation of read and write responsibilities
- Proper use of async/await with `CancellationToken`
- Validation via dedicated validator classes
- Business logic and event emission inside domain model
- Use of handler interfaces like `ICommandHandler<T>` and `IQueryHandler<TQuery, TResult>`

### Result Return Pattern

Use the `Result` class:
- Return value directly for `Result<T>.Ok`
- `null` returns result failure
- Incompatible types cause compile-time error

### Refactor Examples

#### A. Update Handler
**Before**
[Obsolete]
public class UpdateConfigAppCommandHandler {
    private readonly MyDbContext _context;
    public async Task Handle(UpdateConfigAppCommand command) {
        var entity = await _context.ConfigApps.FindAsync(command.Id);
        // ...
        await _context.SaveChangesAsync();
    }
}

**After**
public class UpdateConfigAppCommandHandler : ICommandHandler<UpdateConfigAppCommand> {
    private readonly IConfigAppRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(UpdateConfigAppCommand command, CancellationToken cancellationToken) {
        var configApp = await _repository.FindByIdAsync(command.Id, cancellationToken);
        configApp.UpdateSettings(command.Settings);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}

#### B. Delete Handler
Before
[Obsolete]
public class DeleteConfigAppCommandHandler {
    private readonly DbContext _context;
    public async Task Handle(DeleteConfigAppCommand cmd) {
        var entity = await _context.ConfigApps.FindAsync(cmd.Id);
        _context.ConfigApps.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

After
public class DeleteConfigAppCommandHandler : ICommandHandler<DeleteConfigAppCommand> {
    private readonly IConfigAppRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(DeleteConfigAppCommand command, CancellationToken cancellationToken) {
        await _repository.DeleteByIdAsync(command.Id, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}

#### C. Query Handler
Before
[Obsolete]
public class GetConfigAppsListQueryHandler {
    public async Task<IEnumerable<ConfigAppDto>> Handle(...) {
        return await _context.ConfigApps.Select(...).ToListAsync();
    }
}
After

public class GetConfigAppsListQueryHandler : IQueryHandler<GetConfigAppsListQuery, IEnumerable<ConfigAppDto>> {
    private readonly IConfigAppReadModel _readModel;

    public async Task<IEnumerable<ConfigAppDto>> Handle(GetConfigAppsListQuery query, CancellationToken cancellationToken) {
        return await _readModel.ListAsync(query.Filter, cancellationToken);
    }
}

### General Refactor Checklist
Inject appropriate IRepository<Entity> and ILogger<T> as dependencies
Eliminate DbContext from all handlers
Move all LINQ to read models or repositories
Use async and cancellation tokens
Use domain events only in commands
All handlers must implement:
ICommandHandler<T>
IQueryHandler<TQuery, TResult>

### Affected Unit and Integration Tests
All this refactoring will break some unit testing on application.UnitTesting and Agregation.Testing
Update the afected tests

using Machine tests as the pattern to follow standar or take as example:
you can find pattern to refactor this test on the following testing class

UpdatemachineCommandTest.cs
ToggleEnableMachineCommandValidatorTest.cs
CreateMachineCommandTest.cs
CreateMachineTests.cs
CreateMachineTestsTheory.cs
GetmachineDetailQueryHandlerTest.cs

#### List of all classes needing refactoring: all are marked as obsolete with a todo model:
Create Command Handlers
CreateConfigAppCommandHandler
CreateConfigStationCommandHandler
CreateMachinePlcCommandHandler
CreatePlcCommandHandler
CreateSettingCommandHandler
CreateVariableCommandHandler
CreateWorkFlowCommandHandler
Update Command Handlers
UpdateConfigAppCommandHandler
UpdateConfigStationCommandHandler
UpdatePlcCommandHandler
UpdateProductCommandHandler
UpdateSettingCommandHandler
UpdateShiftCommandHandler
UpdateVariableCommandHandler
UpdateWorkFlowCommandHandler
Query Handlers (List/Detail/Reports)
GetBarCodeDetailMonitorGuiQueryHandler
GetBarCodeDetailQueryQrCodeHandler
GetBarCodeReportQueryHandler
GetBarCodesLabelHandler
GetConfigAppsDetailQueryHandler
GetConfigAppsListQueryHandler
GetConfigStationDetailQueryHandler
GetConfigStationListQueryHandler
GetCyclesDetailQueryHandler
GetCyclesListQueryHandler
GetMachinePlcDetailQueryHandler
GetMaquinasConfigQueryHandler
GetPlcDetailQueryHandler
GetProductDetailQueryHandler
GetReportesFilterInfoGuiQueryHandler
GetReportsListGuiQueryHandler
GetSettingDetailQueryHandler
GetSettingsListQueryHandler
GetShiftDetailQueryHandler
GetShiftsListQueryHandler
GetVariableDetailQueryHandler
GetVariableListQueryHandler
GetWorkFlowDetailQueryHandler

This work can be don autonomsly, refactorin all handlers of an entitie and the afected unit test and droping the class to reivew without waiting for aproval, or made all on one batch ass you see fit, please don't modifed nothing extemal on production code beside thid class, on testing follow the rules about each project, and you can proceed withou autorization.
