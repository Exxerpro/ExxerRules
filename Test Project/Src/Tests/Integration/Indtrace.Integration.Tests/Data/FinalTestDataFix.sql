-- Generate final corrected test data

-- Working cycle test data with correct enums
SELECT TOP 10
    'new object[] { "' + b.Label + '", ' +
    CAST(c.MachineId AS VARCHAR) + ', ' +
    CAST(c.MachineId AS VARCHAR) + ', ' +
    CAST(c.MachineId AS VARCHAR) + ', ' +
    '6, "Ok", "InProcess", "Started", ' +
    CAST(b.BarCodeId AS VARCHAR) + ', ' +
    CAST(c.CycleId AS VARCHAR) + ', "Process", "' +
    CONVERT(VARCHAR(23), c.StartedOn, 126) + '" },' AS WorkingCycleData
FROM Cycles c
INNER JOIN BarCodes b ON c.BarCodeId = b.BarCodeId
INNER JOIN Machines m ON c.MachineId = m.MachineId
WHERE b.Label LIKE 'QA45%'
    AND c.MachineId IN (100, 400, 500)
    AND c.StartedOn IS NOT NULL
    AND c.CycleStatus = 2 -- Started
    AND c.PartStatus = 1  -- Ok
    AND b.FlowStatus = 2  -- InProcess
ORDER BY c.CycleId DESC;

-- Summary of what we found
SELECT 'SUMMARY:' AS Info
SELECT '- ConfigStation: NOT FOUND (need to add to DbContext or remove 7 tests)' AS Fix
SELECT '- Cycles: Found working data with Started status' AS Fix
SELECT '- Machines: All requested machines exist (100,400,500,1200,1400)' AS Fix
SELECT '- Enum Values: Got correct mappings for PartStatus, FlowStatus, CycleStatus' AS Fix
