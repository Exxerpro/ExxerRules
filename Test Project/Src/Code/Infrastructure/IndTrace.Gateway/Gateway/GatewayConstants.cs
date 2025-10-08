// <copyright file="GatewayConstants.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Gateway;

public static class GatewayConstants
{
    // Error Messages
    public const string ErrorGettingBarCode = "Error while getting barcode";

    public const string ErrorReadingPartNumber = "Error while reading part number";
    public const string ErrorReadingPartStatus = "Error while reading PartStatus";
    public const string ErrorReadingCycleStatus = "Error while reading CycleStatus";
    public const string ErrorDownloadingReferences = "Error while downloading references";
    public const string ErrorSettingBarCode = "Error while setting barcode";
    public const string ErrorClearReferences = "Error while ClearReferences. Controller info:";

    // Error Messages
    public const string ErrorExecutingCommand = "Error executing Request";

    public const string ErrorGatewayExecution = "Gateway Error while executing Request";
    public const string ErrorPlcExecution = "Error while executing Request";
    public const string ErrorStartingSignalRConnection = "Error starting SignalR connection";
    public const string ErrorWhileSendingHeartbeat = "Error while sending HeartBeat";
    public const string ErrorUploadingRegisters = " Error while Uploading Register from PLC";
    public const string ErrorUploadingDataForCommand = " Error while Uploading Data For Command from PLC";

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate gateway constants logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
