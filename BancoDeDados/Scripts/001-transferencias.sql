USE JKCASHDb;
GO
SET XACT_ABORT ON;
-- Opções exigidas pelo SQL Server para criar índices filtrados.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
BEGIN TRANSACTION;

-- NULL identifica os registros normais, inclusive os que já existem.
IF COL_LENGTH('dbo.Transacoes', 'TransferenciaId') IS NULL
    ALTER TABLE dbo.Transacoes ADD TransferenciaId UNIQUEIDENTIFIER NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.Transacoes') AND name = 'IX_Transacoes_TransferenciaId')
    EXEC(N'CREATE INDEX IX_Transacoes_TransferenciaId
        ON dbo.Transacoes(TransferenciaId) WHERE TransferenciaId IS NOT NULL;');

COMMIT;
GO
