# Repositórios locais do JKCASH

Os repositórios locais implementam os mesmos contratos de Application usando `ConexaoLocal.CriarContexto()`. O arquivo SQLite fica em `%LocalAppData%\JKCASH\jkcash.db`.

## Classes e responsabilidades

Todos os antigos pares SQL/Local agora possuem uma implementação EF compartilhada. O Windows Forms fornece `ConexaoLocal.CriarContexto` e o Console em modo SQL fornece `ConexaoSql.CriarContexto`. Os repositórios em memória permanecem separados, pois não utilizam EF nem contextos de banco.

| Classe | Interface | Responsabilidade |
|---|---|---|
| CategoriaRepositorioEf | ICategoriaRepositorio | Adicionar, listar e verificar categorias; recebe a função que cria o contexto SQLite ou SQL Server |
| ContaRepositorioEf | IContaRepositorio | Adicionar, listar e excluir contas, com as verificações dos vínculos |
| CartaoCreditoRepositorioEf | ICartaoCreditoRepositorio | Adicionar, buscar e listar cartões; recebe a função que cria o contexto SQLite ou SQL Server |
| TransacaoRepositorioEf | ITransacaoRepositorio | Consultas e gravação conjunta do lançamento e do saldo |
| TransferenciaRepositorioEf | ITransferenciaRepositorio | Gravar os dois lançamentos e os dois saldos juntos |
| CompraCartaoRepositorioEf | ICompraCartaoRepositorio e IFaturaCartaoRepositorio | Compras, parcelas, faturas e pagamentos |
| ExclusaoRepositorioEf | IExclusaoRepositorio | Gravar as exclusões e os ajustes preparados por ExclusaoServico |

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

### Todos os repositórios compartilham a implementação EF

Cada classe EF exige `Func<JKCASHDbContext>` no construtor, evitando escolher um provedor implicitamente. A função só é executada dentro de cada operação e o contexto continua sendo descartado por `using`.

```csharp
ITransacaoRepositorio local = new TransacaoRepositorioEf(ConexaoLocal.CriarContexto);
ITransacaoRepositorio servidor = new TransacaoRepositorioEf(ConexaoSql.CriarContexto);
```

`CompraCartaoRepositorioEf` também aceita `TimeProvider` opcional como segundo argumento e continua implementando as interfaces de compras e faturas. Os serviços compartilham a mesma instância quando precisam desses dois contratos.

Na exclusão de contas, foi mantida a transação serializável da versão SQLite, agora também utilizada no SQL Server. O tratamento de vínculos contempla `SqliteException` (787) e `SqlException` (547). As demais regras de saldo, conferência de estado, pagamentos e rollback foram extraídas sem alteração dos algoritmos. Os conversores e mapeamentos específicos de cada provedor continuam no contexto.

A solução compilou sem avisos ou erros após a unificação completa. Não foram executadas operações, testes, migrações ou importações contra os bancos nesta refatoração.

### Categorias: implementação EF compartilhada

`CategoriaRepositorioEf` substitui `CategoriaRepositorioLocal` e `CategoriaRepositorioSql`. Seus três métodos mantêm o mesmo comportamento anterior; a configuração escolhe qual banco utilizar:

```csharp
// Windows Forms: SQLite.
ICategoriaRepositorio local = new CategoriaRepositorioEf(ConexaoLocal.CriarContexto);

// Console no modo SQL: SQL Server.
ICategoriaRepositorio sql = new CategoriaRepositorioEf(ConexaoSql.CriarContexto);
```

`Func<JKCASHDbContext>` representa uma função sem argumentos que devolve um contexto. Passar `ConexaoLocal.CriarContexto` sem parênteses entrega a função; chamar `_criarContexto()` dentro de cada método cria o contexto daquela operação. `using` continua liberando esse contexto ao terminar.

A interface de categorias, a ordenação por nome e Id, o uso de `AsNoTracking` nas consultas e a validação de argumento nulo ao adicionar foram preservados. O modo em memória do Console não mudou. Não foi necessário alterar esquema, migrações nem dados. A solução compilou sem avisos ou erros; os métodos não foram executados contra os bancos nesta refatoração.

### Cartões: implementação EF compartilhada

`CartaoCreditoRepositorioEf` substitui `CartaoCreditoRepositorioLocal` e `CartaoCreditoRepositorioSql`, seguindo o padrão de categorias. Implementa `ICartaoCreditoRepositorio` e recebe `Func<JKCASHDbContext>` no construtor.

No Windows Forms, a fábrica entrega `ConexaoLocal.CriarContexto`; no Console em modo SQL, a configuração entrega `ConexaoSql.CriarContexto`. `Adicionar`, `BuscarPorId` e `ListarTodos` possuem uma única implementação. Permanecem a validação de argumento nulo, o retorno nulo para Id inexistente, as consultas sem rastreamento e a ordenação por nome e Id. Compras, pagamentos e regras de limite não foram alterados nesta extração.

A solução compilou sem avisos ou erros. Não houve execução dos métodos contra os bancos nesta refatoração, alteração de esquema ou migração de dados.

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
