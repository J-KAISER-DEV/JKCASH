# Backup pela API e restauração local

A tela inicial mantém o uso diário no SQLite. Conectar ao servidor não muda o banco usado pelas telas e não inicia envios automáticos.

## Como usar

1. Execute `JKCASH.Api` com o perfil **https**; a URL padrão é `https://localhost:7068/`.
2. Abra o Windows Forms. A tela verifica a API e, separadamente, a disponibilidade do armazenamento SQL.
3. **Enviar backup** pede confirmação, cria uma cópia consistente do SQLite e envia essa cópia completa. A confirmação aparece somente após o SQL Server concluir a gravação.
4. **Restaurar backup** lista as 100 cópias mais recentes, baixa a escolhida e verifica tamanho, SHA-256, integridade SQLite, chaves estrangeiras e versão de migrações. Só então pede confirmação com a quantidade de registros por tabela.
5. Ao confirmar, uma cópia anterior é guardada em `%LocalAppData%/JKCASH/BackupsSeguranca`. As migrações conhecidas são aplicadas em uma cópia temporária, e a substituição do arquivo local acontece somente depois da validação. Um backup de versão desconhecida é recusado.
6. **Abrir aplicativo** segue usando o SQLite mesmo sem o servidor. Durante envio/restauração, aguarda-se a conclusão antes de abrir ou fechar.

## Receber registros que já existem no SQL Server

Use **Receber dados existentes do SQL Server** na tela inicial. Essa ação não exige um backup enviado anteriormente: a API lê diretamente as oito tabelas de uso diário da conexão `ConnectionStrings:JKCASH` e monta um SQLite temporário.

Contas, categorias, cartões, transações, compras, faturas, parcelas e pagamentos são lidos dentro de uma transação serializável. Os valores monetários são convertidos de decimal para centavos, mantendo a precisão; os identificadores, datas, estados e vínculos são preservados. Nenhum serviço de crédito, débito ou compra é executado, e a origem SQL não recebe INSERT, UPDATE, DELETE ou migrações.

Após receber, a interface apresenta as quantidades e pergunta se deve substituir a base local. **Não** mantém o SQLite atual. **Sim** usa a mesma proteção da restauração: validação, cópia anterior em `BackupsSeguranca` e troca somente após preparar o novo banco. A ação aparece no histórico como **Importação SQL**, separada de receber um backup arquivado.

O banco de origem precisa ter todas as tabelas e colunas esperadas pela versão atual. Uma falha não produz uma importação parcial. A exportação possui prazo de 50 segundos e usa o mesmo limite de 16 MiB do formato de backup. O pacote recebido é transitório: para arquivar uma cópia no servidor depois da importação, use **Enviar backup**.

Feche instâncias de versões anteriores do JKCASH antes de restaurar. A versão atual mantém um bloqueio de arquivo durante toda a execução, impedindo duas instâncias do Windows Forms de usar a mesma base local.

## O que é armazenado

O formato 1 guarda um snapshot SQLite completo, com limite de **16 MiB**, incluindo todas as tabelas, identificadores, dinheiro em centavos e histórico de migrações. A API grava os bytes na tabela `dbo.BackupsAplicacao`, juntamente com identificador, data UTC do servidor, máquina, tamanho, versão e hash SHA-256. A tabela é criada automaticamente no primeiro acesso à área de backups, usando a conexão `ConnectionStrings:JKCASH`; requer permissão para criação dessa tabela.

As tabelas de contas e movimentações já existentes no SQL Server não são substituídas nem atualizadas por um envio. As rotas de contas continuam sendo consultas independentes. Enviar um snapshot não debita saldos nem repete compras. A importação explícita dessas tabelas é feita pelo botão descrito acima; não existe sincronização automática.

`PUT /api/backups/{id}` é idempotente para o mesmo identificador e conteúdo. Conteúdo diferente com o mesmo identificador recebe 409. Um novo envio manual cria outro identificador/cópia. Se a resposta do envio se perder, a cópia pode ter sido armazenada: confira a listagem antes de repetir. Não há tentativas automáticas de gravação.

As cópias não são apagadas automaticamente. O histórico local (`historico-backups.json`) mantém as 200 operações mais recentes, incluindo recebimento sem restauração, cancelamento, falha e confirmação. Esse histórico é separado do banco restaurado, para não desaparecer durante a troca. A data de envio confirmada na tela refere-se a esta máquina; o seletor mostra as cópias do servidor.

## Rotas

- `GET /api/status`: responde sobre a API, sem acessar o banco.
- `GET /api/backups`: lista metadados dos 100 últimos backups.
- `PUT /api/backups/{id}`: recebe e valida o snapshot; retorna metadados após confirmar a transação SQL.
- `GET /api/backups/{id}`: entrega o snapshot e seus metadados, ou 404.
- `GET /api/exportacao/sqlserver`: consulta diretamente as tabelas existentes do SQL Server e devolve uma cópia compatível com a base local, sem gravar na origem.

Nesta entrega a API aceita somente conexões de loopback (esta máquina), inclusive para as outras rotas. Autenticação e configuração para acesso de outra máquina são uma entrega futura: não basta abrir a porta. A validação do certificado HTTPS permanece habilitada. SQL Server indisponível produz 503 nos endpoints de backup; abrir o aplicativo local continua possível.

O backup no mesmo computador não protege contra perda desse computador. Cópia externa do SQL Server e criptografia/autenticação para outros ambientes ainda precisam de planejamento.

## Organização do código

- `Application/Modelos/BackupModelos.cs`: contratos da comunicação.
- `Infrastructure/Backup/BancoBackup.cs`: snapshot nativo, integridade, versão e preparação da cópia.
- `Infrastructure/Backup/BackupLocal.cs`: envio e restauração atômica do arquivo.
- `Infrastructure/Backup/BackupRepositorioSql.cs`: armazenamento separado no SQL Server.
- `Infrastructure/Integracoes/Api/BackupApiCliente.cs`: comunicação HTTP.
- `WinForms/FrmInicial.Backups.cs`: confirmação, histórico e coordenação da interface.
- `Infrastructure/Backup/ExportacaoSql.cs`: conversão consistente dos registros SQL existentes para um SQLite temporário.
- `WinForms/FrmInicial.Importacao.cs`: recebimento do SQL e aplicação compartilhada com a restauração de backups.

O snapshot utiliza `SqliteConnection.BackupDatabase`, e não uma cópia isolada do arquivo `.db`, para incluir transações confirmadas ainda no WAL. Referência: [Microsoft.Data.Sqlite — Online backup](https://learn.microsoft.com/dotnet/standard/data/sqlite/backup).
