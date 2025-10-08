-- Simple query to get working cycle test data
SELECT TOP 10
    'new object[] { "' + b.Label + '", ' +
    CAST(c.MachineId AS VARCHAR) + ', ' +           -- machineId
    CAST(c.MachineId AS VARCHAR) + ', ' +           -- lastMachineId (using same)
    CAST(c.MachineId AS VARCHAR) + ', ' +           -- nextMachineId (using same)
    '6, ' +                                         -- lenPartNumber (fixed)
    '"' + CASE c.PartStatus
        WHEN 0 THEN 'None'
        WHEN 1 THEN 'Ok'
        WHEN 2 THEN 'nOK'
        WHEN 8 THEN 'Rejected'
        WHEN 512 THEN 'Scrap'
        ELSE 'Ok'
    END + '", ' +                                   -- partStatus
    '"' + CASE b.FlowStatus
        WHEN 1 THEN 'Created'
        WHEN 2 THEN 'InProcess'
        WHEN 4 THEN 'Finished'
        WHEN 8 THEN 'Invalid'
        WHEN 16 THEN 'Restored'
        WHEN 32 THEN 'Rejected'
        ELSE 'InProcess'
    END + '", ' +                                   -- flowStatus
    '"' + CASE c.CycleStatus
        WHEN 0 THEN 'None'
        WHEN 1 THEN 'NotStarted'
        WHEN 2 THEN 'Started'
        WHEN 4 THEN 'FinishedOk'
        WHEN 8 THEN 'FinishedNok'
        WHEN 16 THEN 'Canceled'
        WHEN 32 THEN 'Rejected'
        ELSE 'Started'
    END + '", ' +                                   -- cycleStatus
    CAST(b.BarCodeId AS VARCHAR) + ', ' +           -- barCodeId
    CAST(c.CycleId AS VARCHAR) + ', ' +             -- cycleId
    '"' + CASE m.MachineType
        WHEN 1 THEN 'Printer'
        WHEN 2 THEN 'Initial'
        WHEN 4 THEN 'InitialPrinter'
        WHEN 8 THEN 'Process'
        WHEN 16 THEN 'Final'
        WHEN 32 THEN 'Inspection'
        WHEN 64 THEN 'DashBoard'
        ELSE 'Process'
    END + '", ' +                                   -- machineType
    '"' + CONVERT(VARCHAR(23), c.StartedOn, 126) + '" },' AS CorrectCycleTestData
FROM Cycles c
INNER JOIN BarCodes b ON c.BarCodeId = b.BarCodeId
INNER JOIN Machines m ON c.MachineId = m.MachineId
WHERE b.Label LIKE 'QA45%'
    AND c.MachineId IN (100, 400, 500)
    AND c.StartedOn IS NOT NULL
    AND c.CycleStatus IN (1, 2) -- NotStarted or Started
ORDER BY c.CycleId DESC;
