-- Execute somente no banco SQL Server do JKCASH, antes de usar a nova versão no modo SQL.
-- Preserva os valores existentes; permite NULL para novas entradas sem avaliação.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.Transacoes', N'U') IS NULL
    THROW 50000, 'A tabela dbo.Transacoes não foi encontrada.', 1;

DECLARE @tipo sysname, @tamanho int;
SELECT @tipo = TYPE_NAME(c.system_type_id), @tamanho = c.max_length
FROM sys.columns c
WHERE c.object_id = OBJECT_ID(N'dbo.Transacoes') AND c.name = N'Justificativa';
IF @tipo NOT IN (N'varchar', N'nvarchar') OR @tipo IS NULL
    THROW 50000, 'Confira o tipo da coluna Justificativa antes de alterar o esquema.', 1;
DECLARE @limite nvarchar(20) = CASE WHEN @tamanho = -1 THEN N'max'
    ELSE CONVERT(nvarchar(20), CASE WHEN @tipo = N'nvarchar' THEN @tamanho / 2 ELSE @tamanho END) END;
DECLARE @sql nvarchar(max) = N'ALTER TABLE dbo.Transacoes ALTER COLUMN Justificativa '
    + QUOTENAME(@tipo) + N'(' + @limite + N') NULL;';
EXEC sys.sp_executesql @sql;
ALTER TABLE dbo.Transacoes ALTER COLUMN Importancia int NULL;
COMMIT TRANSACTION;
