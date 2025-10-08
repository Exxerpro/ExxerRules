-- Quick schema analysis for test fixing
-- Check what tables actually exist and their structure

-- 1. List all tables in the database
SELECT
    'Table: ' + TABLE_NAME as TableInfo,
    'Columns: ' + CAST(COUNT(*) AS VARCHAR) as ColumnCount
FROM INFORMATION_SCHEMA.COLUMNS
GROUP BY TABLE_NAME
ORDER BY TABLE_NAME;

-- 2. Check if ConfigStation exists
SELECT
    CASE WHEN EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ConfigStation')
        THEN 'ConfigStation EXISTS'
        ELSE 'ConfigStation NOT FOUND'
    END as ConfigStationStatus;

-- 3. Get enum lookup values for status fields
SELECT 'CycleStatus Values:' as Info
SELECT Id, Name, DisplayName FROM CycleStatus ORDER BY Id;

SELECT 'FlowStatus Values:' as Info
SELECT Id, Name, DisplayName FROM FlowStatus ORDER BY Id;

SELECT 'PartStatus Values:' as Info
SELECT Id, Name, DisplayName FROM PartStatus ORDER BY Id;

-- 4. Check machine types
SELECT 'MachineType Values:' as Info
SELECT Id, Name, DisplayName FROM MachineType ORDER BY Id;
