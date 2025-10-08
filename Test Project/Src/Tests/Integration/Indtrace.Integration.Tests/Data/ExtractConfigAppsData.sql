-- Extract real ConfigApps test data for fixing ConfigStation tests

-- 1. Check what ConfigApps data exists
SELECT
    'ConfigApps Records: ' + CAST(COUNT(*) AS VARCHAR) AS Info
FROM ConfigApps;

-- 2. Generate working test data for ConfigApp (not ConfigStation)
SELECT TOP 10
    'new object[] { "' + ConfigAppId + '", ' +
    CAST(AppId AS VARCHAR) + ', ' +
    CAST(MachineId AS VARCHAR) + ', "' +
    ISNULL(Client, 'TestClient') + '", "' +
    ISNULL(Factory, 'TestFactory') + '", "' +
    ISNULL(Line, 'TestLine') + '" },' AS ConfigAppTestData
FROM ConfigApps
WHERE ConfigAppId IS NOT NULL
ORDER BY AppId DESC;

-- 3. Sample data for triplication
SELECT
    '[InlineData("IndTraceDbContext45", "' + ConfigAppId + '", ' +
    CAST(MachineId AS VARCHAR) + ', "' +
    ISNULL(Client, 'TestClient') + '")]' AS InlineDataFormat
FROM ConfigApps
WHERE ConfigAppId IS NOT NULL
ORDER BY AppId DESC
OFFSET 0 ROWS FETCH NEXT 5 ROWS ONLY;
