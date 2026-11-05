-- SQL Server: atualização aditiva para o cadastro de gastos.
-- Execute no banco JKCASH existente, após 001 e 002. Pode ser executado novamente.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Gastos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Gastos (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_Gastos PRIMARY KEY,
        Nome nvarchar(200) NOT NULL,
        CategoriaId uniqueidentifier NOT NULL,
        Natureza int NOT NULL,
        Importancia int NOT NULL,
        Necessidade int NOT NULL,
        Planejamento int NOT NULL,
        Frequencia int NOT NULL,
        Motivo nvarchar(500) NOT NULL,
        CONSTRAINT FK_Gastos_Categorias_CategoriaId FOREIGN KEY (CategoriaId)
            REFERENCES dbo.Categorias(Id)
    );
    CREATE INDEX IX_Gastos_CategoriaId ON dbo.Gastos(CategoriaId);
END;

IF COL_LENGTH(N'dbo.Transacoes', N'GastoCadastroId') IS NULL
    ALTER TABLE dbo.Transacoes ADD GastoCadastroId uniqueidentifier NULL;

IF COL_LENGTH(N'dbo.Transacoes', N'GastoNome') IS NULL
    ALTER TABLE dbo.Transacoes ADD GastoNome nvarchar(200) NULL;

IF COL_LENGTH(N'dbo.Transacoes', N'GastoNatureza') IS NULL
    ALTER TABLE dbo.Transacoes ADD GastoNatureza int NULL;

IF COL_LENGTH(N'dbo.Transacoes', N'GastoNecessidade') IS NULL
    ALTER TABLE dbo.Transacoes ADD GastoNecessidade int NULL;

IF COL_LENGTH(N'dbo.Transacoes', N'GastoFrequencia') IS NULL
    ALTER TABLE dbo.Transacoes ADD GastoFrequencia int NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Transacoes') AND name = N'IX_Transacoes_GastoCadastroId')
    EXEC(N'CREATE INDEX IX_Transacoes_GastoCadastroId ON dbo.Transacoes(GastoCadastroId)');

IF OBJECT_ID(N'dbo.FK_Transacoes_Gastos_GastoCadastroId', N'F') IS NULL
    EXEC(N'ALTER TABLE dbo.Transacoes ADD CONSTRAINT FK_Transacoes_Gastos_GastoCadastroId FOREIGN KEY (GastoCadastroId) REFERENCES dbo.Gastos(Id)');

IF COL_LENGTH(N'dbo.ComprasCartao', N'GastoCadastroId') IS NULL
    ALTER TABLE dbo.ComprasCartao ADD GastoCadastroId uniqueidentifier NULL;

IF COL_LENGTH(N'dbo.ComprasCartao', N'GastoNome') IS NULL
    ALTER TABLE dbo.ComprasCartao ADD GastoNome nvarchar(200) NULL;

IF COL_LENGTH(N'dbo.ComprasCartao', N'GastoNatureza') IS NULL
    ALTER TABLE dbo.ComprasCartao ADD GastoNatureza int NULL;

IF COL_LENGTH(N'dbo.ComprasCartao', N'GastoNecessidade') IS NULL
    ALTER TABLE dbo.ComprasCartao ADD GastoNecessidade int NULL;

IF COL_LENGTH(N'dbo.ComprasCartao', N'GastoFrequencia') IS NULL
    ALTER TABLE dbo.ComprasCartao ADD GastoFrequencia int NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ComprasCartao') AND name = N'IX_ComprasCartao_GastoCadastroId')
    EXEC(N'CREATE INDEX IX_ComprasCartao_GastoCadastroId ON dbo.ComprasCartao(GastoCadastroId)');

IF OBJECT_ID(N'dbo.FK_ComprasCartao_Gastos_GastoCadastroId', N'F') IS NULL
    EXEC(N'ALTER TABLE dbo.ComprasCartao ADD CONSTRAINT FK_ComprasCartao_Gastos_GastoCadastroId FOREIGN KEY (GastoCadastroId) REFERENCES dbo.Gastos(Id)');

COMMIT TRANSACTION;
