# Repositórios locais do JKCASH

Os repositórios locais implementam os mesmos contratos de Application usando `ConexaoLocal.CriarContexto()`. O arquivo SQLite fica em `%LocalAppData%\JKCASH\jkcash.db`.

## Classes e responsabilidades

| Classe | Interface | Responsabilidade |
|---|---|---|
| CategoriaRepositorioLocal | ICategoriaRepositorio | Adicionar, listar e verificar categorias; implementado durante o aprendizado |
| ContaRepositorioLocal | IContaRepositorio | Adicionar, listar e excluir contas, com as verificações dos vínculos |
| CartaoCreditoRepositorioLocal | ICartaoCreditoRepositorio | Adicionar, buscar e listar cartões |
| TransacaoRepositorioLocal | ITransacaoRepositorio | Consultas e gravação conjunta do lançamento e do saldo |
| TransferenciaRepositorioLocal | ITransferenciaRepositorio | Gravar os dois lançamentos e os dois saldos juntos |
| CompraCartaoRepositorioLocal | ICompraCartaoRepositorio e IFaturaCartaoRepositorio | Compras, parcelas, faturas e pagamentos |
| ExclusaoRepositorioLocal | IExclusaoRepositorio | Gravar as exclusões e os ajustes preparados por ExclusaoServico |

Não há um repositório independente para cada parcela ou pagamento. Como na implementação SQL existente, eles participam das operações coordenadas pelo repositório de compras e faturas. As duas interfaces são implementadas pela mesma classe. Os métodos de consulta de faturas são implementações explícitas de `IFaturaCartaoRepositorio`.

## Como as operações são salvas

`using` libera o contexto e a transação ao terminar o método. `SaveChanges()` executa as alterações acompanhadas pelo EF. Quando há uma transação explícita, `Commit()` confirma o conjunto; sair do método sem confirmar desfaz as alterações dessa transação.

- **Transação:** o serviço movimenta o objeto em memória. O repositório confere o saldo anterior no banco, movimenta a conta persistida pelas regras da entidade e grava o lançamento no mesmo contexto. Se outra operação já mudou o saldo, recusa a gravação desatualizada.
- **Transferência:** confere os saldos anteriores e grava débito, crédito e lançamentos juntos.
- **Compra:** confere a versão recebida do cartão, valida o limite e os ciclos e grava compra, parcelas, faturas e limite conjuntamente.
- **Pagamento:** confere a fatura, o saldo e o limite e grava pagamento, saída da conta e alterações financeiras conjuntamente.
- **Exclusão:** aplica dependentes antes dos registros referenciados, atualiza os saldos e limites e confirma tudo junto. A assinatura do estado impede confirmar uma prévia desatualizada.

O método de exclusão de conta também mantém na mesma transação a consulta dos vínculos e a exclusão. O tratamento de chave estrangeira dessa implementação usa `SqliteException`, pois os códigos de erro do SQL Server não se aplicam ao SQLite.

## Conexão e precisão

`ConexaoLocal` usa `SqliteConnectionStringBuilder` com `ForeignKeys = true`, para ativar a verificação dos relacionamentos em cada conexão. A conexão não depende do serviço SQL Server.

O conversor monetário já criado mantém `decimal` nas entidades e grava centavos inteiros no arquivo. Os novos repositórios reutilizam esse modelo, sem converter dinheiro para `double`.

## Estado desta entrega

- Implementações locais criadas para todos os contratos atuais de repositório.
- Repositórios SQL e em memória preservados.
- Todas as telas do Windows Forms usam os repositórios locais: Dashboard, Contas (incluindo transferências), Categorias, Transações, Cartões e Faturas (incluindo pagamentos).
- Antes de abrir a janela principal, `Program.cs` chama `ConexaoLocal.Inicializar()`: cria a pasta e aplica as migrações pendentes. Falhas na preparação são informadas antes de carregar as telas; não há fallback automático para SQL Server.
- O Console mantém sua configuração anterior, com os modos SQL e em memória.
- Nenhum dado importado do SQL Server para SQLite e nenhum registro criado para teste.
- Compilação da Infrastructure e do Windows Forms concluída sem avisos ou erros. Os fluxos de execução dos novos repositórios e telas ainda não foram testados.

## Próxima etapa

Se os dados atuais do SQL Server precisarem aparecer localmente, tratar a importação explicitamente. Não apagar nem sobrescrever registros locais já cadastrados. A sincronização entre máquinas continua fora do escopo desta etapa.
