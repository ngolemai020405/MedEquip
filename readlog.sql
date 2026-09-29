SELECT 
    [Current LSN], 
    [Transaction ID], 
    [Operation], 
    [Context], 
    [AllocUnitName],
    [RowLog Contents 0],
    [RowLog Contents 1]
FROM fn_dblog(NULL, NULL)
WHERE [Operation] = 'LOP_MODIFY_ROW' AND [AllocUnitName] LIKE 'dbo.Products%'
ORDER BY [Current LSN] DESC;
