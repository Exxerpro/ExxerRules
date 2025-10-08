// <copyright file="GatewayMessages.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Gateway;

public static class GatewayMessages
{
    // Error messages
    public const string ErrorCreateBarcode = "Error while sending create barcode Request";

    public const string FailedCreateBarcode = "Failed to create barcode";
    public const string ErrorReadBarcode = "Error while sending get barcode detail query";
    public const string FailedReadBarcode = "Failed to read barcode";
    public const string ErrorCreateCycle = "Error while creating a cycle Request";
    public const string FailedCreateCycle = "Failed to create cycle";
    public const string ErrorUpdateCycle = "Error while updating a cycle Request";
    public const string FailedUpdateCycle = "Failed to update cycle";
    public const string ErrorEndProcess = "Error while sending end of process Request";
    public const string FailedEndProcess = "Failed to end process workflow";

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate gateway messages logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
