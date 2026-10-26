# Análise de organização e desempenho do JKCASH

Data: 25/10/2026. Base analisada: commit `c7e20fc`, versão sincronizada com o GitHub.

Esta revisão não altera as regras financeiras nem o banco de dados. As oportunidades de desempenho abaixo foram identificadas no código; não foram feitos benchmarks nem medições de latência da interface.

## Atualização após a etapa 1

A etapa 1 foi implementada após a análise abaixo. Os apontamentos originais descrevem a base revisada, antes dessas alterações.

- Transações, transferências e pagamentos consultam e conferem o estado financeiro dentro da transação de gravação, tanto no SQLite quanto no SQL Server. Saldo, situação da conta, cheque especial e, nos pagamentos, fatura e limite do cartão são revalidados.
- Falhas desfazem a gravação conjunta e os serviços restauram os objetos movimentados em memória. Conflitos exigem nova consulta e confirmação manual, sem repetir a movimentação automaticamente.
- As telas de Transações, Transferências e Faturas atualizam os dados após conflitos. O registro de transações executa fora da thread visual e impede novos cliques durante a gravação.
- Nove cenários de saldo desatualizado, concorrência, transferências, pagamentos parciais e rollback foram conferidos em cada provedor, usando bancos temporários isolados e removidos ao final. Nenhum projeto de testes foi adicionado à solução.
- A solução compilou sem erros; permaneceu o aviso NU1900 sobre a consulta de vulnerabilidades do NuGet. A interação visual ainda precisa ser conferida na aplicação.

O Windows Forms continua usando SQLite, sem exigir SQL Server instalado. A etapa não adicionou pacotes nem alterou o esquema do banco.

## Resultado da verificação

- A solução `JKCASH.sln` compilou com zero erros.
- Houve um aviso NU1900 de indisponibilidade da consulta de vulnerabilidades do NuGet. Isso não confirma a presença nem a ausência de vulnerabilidades nos pacotes.
- A solução contém cinco projetos após a remoção do projeto de testes. `JKCASH.Api` existe no repositório, mas está fora dessa solução; seu código ainda corresponde à estrutura inicial com WeatherForecast e não é usado pelo WinForms.
- Por decisão do usuário, o projeto de testes foi removido após esta revisão. O planejamento abaixo foi ajustado para priorizar diretamente a aplicação.

## 1. Prioridade alta: preservar o saldo em atualizações concorrentes

**Onde:** `JKCASH.Infrastructure/Repositorios/TransacaoRepositorioSql.cs`, método `Adicionar`, linha 35.

O serviço recebe uma conta obtida em outra consulta e altera seu saldo. O repositório anexa esse objeto e grava o valor absoluto de `SaldoAtual`, sem verificar se o saldo do banco mudou. Não há token de concorrência configurado nesse mapeamento.

Exemplo derivado do código: dois objetos foram carregados com R$ 1.000. Uma saída de R$ 100 grava R$ 900; uma segunda saída de R$ 50, calculada sobre a cópia antiga, grava R$ 950. As duas transações podem existir, enquanto o saldo correto seria R$ 850. É uma análise estática; esse cenário não foi reproduzido nesta revisão.

O repositório SQLite já faz uma leitura e validação dentro de uma transação serializável. A implementação SQL Server precisa oferecer a mesma garantia. Alternativas: consultar e movimentar a conta na mesma transação ou usar controle otimista de versão, com tratamento de conflito. Token mantido pela aplicação permite compartilhar a estratégia entre provedores; `rowversion` é específico do SQL Server.

**Critério de conclusão:** um teste com duas leituras desatualizadas demonstra que a segunda operação é recusada ou refeita sobre o estado atual, sem perder movimentações. Manter as garantias de rollback.

Referência: [concorrência no EF Core](https://learn.microsoft.com/en-us/ef/core/saving/concurrency).

## 2. Desempenho: consultar os dados que o dashboard usa

**Onde:** `JKCASH.WinForms/Telas/UcDashboard.cs`, linhas 198–230; `JKCASH.Infrastructure/Repositorios/CompraCartaoRepositorioLocal.cs`, linhas 107–116 e 195–201.

O dashboard carrega as transações do período, todos os cartões e as faturas de cada cartão. As faturas trazem parcelas e pagamentos de vários ciclos. Só depois a tela filtra o mês, calcula totais e seleciona oito movimentações. O número de consultas de faturas cresce com o número de cartões.

**Proposta:** criar um serviço de consulta `DashboardConsulta` com um DTO de resultado. No banco, filtrar contas e vencimentos, calcular os resumos e consultar somente as oito últimas movimentações. Separar consulta resumida e detalhes completos da fatura. Aplicar paginação ao extrato quando necessário.

Os repositórios de transações usam `transacao.Data.Date` no filtro. Avaliar o intervalo direto `Data >= inicio && Data < fimExclusivo`, com o limite final calculado antes da consulta. Avaliar um índice composto como `(ContaId, Data)` com planos de execução e dados representativos; índices adicionais também têm custo nas gravações.

**Critério de conclusão:** mesmos resultados financeiros, quantidade de consultas que não cresce uma vez por cartão e comparação de tempo/memória com uma massa sintética maior. Ainda não há estimativa de ganho percentual.

Referência: [consultas eficientes no EF Core](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying).

## 3. Desempenho da interface: retirar operações de banco dos eventos síncronos

**Onde:** `JKCASH.WinForms/Telas/UcTransacoes.cs`, linhas 49, 52, 102 e 129.

O carregamento de opções e o registro da transação chamam repositórios síncronos diretamente na thread da interface. Outras telas já usam trabalho em segundo plano, mas esse comportamento não está padronizado.

**Proposta:** padronizar o estado ocupado, desabilitar o botão durante a operação e carregar/gravar fora da thread visual. Capturar os valores dos controles antes de iniciar o trabalho, criar um contexto por operação e tratar o fechamento da tela durante a execução. Centralizar o tratamento de falhas de consulta e gravação, preservando a diferença entre "não foi salvo" e "salvou, mas falhou ao atualizar a tela".

Para SQL Server, avaliar os métodos assíncronos do EF. Para SQLite, acrescentar `Async` não cria I/O assíncrono: o provedor executa essas operações sincronamente. O uso controlado de trabalho em segundo plano continua relevante no desktop. O `DbContext` não deve ser compartilhado entre tarefas simultâneas.

Referência: [limitações assíncronas do Microsoft.Data.Sqlite](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/async).

## 4. Organização: reduzir duplicação entre SQL Server e SQLite

**Onde:** pares `CartaoCreditoRepositorioSql/Local`, `CompraCartaoRepositorioSql/Local` e `TransferenciaRepositorioSql/Local`.

Comparação realizada nesta revisão: esses três pares são idênticos após normalizar o nome da classe e substituir `ConexaoLocal` por `ConexaoSql`. Apenas o par de compras contém 211 linhas em cada arquivo. Outros pares têm divergências reais, como a proteção de saldo descrita no item 1.

**Proposta:** extrair implementações EF por caso de uso, recebendo uma fábrica de `JKCASHDbContext`. Configurar o provedor na inicialização. Manter explícitas as diferenças de conversão monetária, migrations, concorrência e recursos suportados por cada banco. Fazer a extração por par e conferir os mesmos fluxos nos dois provedores.

O `ConversorMoeda` que guarda centavos em `long` é uma decisão a preservar no SQLite; não converter dinheiro para `double` durante a simplificação.

**Benefício principal:** corrigir uma regra de persistência uma vez e reduzir divergências. Remover classes duplicadas, por si só, não torna as consultas mais rápidas.

## 5. Organização: centralizar dependências e separar a consulta da apresentação

**Onde:** `JKCASH.WinForms/FrmPrincipal.Visual.cs`, método `AbrirTela`, linha 103.

A navegação também cria repositórios e serviços. Essa composição se repete por tela, e o Console tem uma configuração independente. O dashboard concentra a criação dos controles, as consultas e os cálculos dos indicadores.

**Proposta:** registrar serviços em uma extensão de inicialização, usar uma fábrica de telas e manter os formulários responsáveis por apresentar dados e coletar entradas. Extrair primeiro as consultas do dashboard; não há necessidade de reescrever a interface inteira. Preservar o descarte dos controles antigos, que já é feito ao navegar.

Usar `IDbContextFactory<JKCASHDbContext>` ou uma abstração equivalente por operação. Não registrar um único contexto como singleton. No modo em memória, preservar o compartilhamento intencional do armazenamento entre compras e faturas.

Referência: [Generic Host e injeção de dependência do .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host).

## 6. Desempenho: limitar a abrangência das prévias de exclusão

**Onde:** `JKCASH.Infrastructure/Repositorios/ExclusaoRepositorioLocal.cs`, linhas 20–35; `JKCASH.Application/Modelos/EstadoFinanceiro.cs`, linha 15.

Para preparar e confirmar exclusões, o código carrega contas, categorias, transações, cartões, compras e faturas de todo o banco. A assinatura serializa esse estado e calcula SHA-256. O custo cresce com todo o histórico, mesmo quando a exclusão afeta poucos registros. Uma alteração não relacionada também pode invalidar a prévia.

**Proposta:** consultar o conjunto afetado e seus vínculos, gerar a prévia desse conjunto e revalidar seus identificadores/versões na transação de exclusão. Manter as proteções contra pagamentos órfãos, saldo inválido e alteração do limite. Conferir os efeitos financeiros de cada tipo de exclusão ao implementar essa mudança.

## 7. Dependências, diagnóstico e padronização

- `Domain` e `Application` referenciam `System.Security.Cryptography.Pkcs`, mas não encontrei uso de APIs PKCS nesses projetos. `SHA256.HashData`, utilizado em `EstadoFinanceiro`, não exige esse pacote. Verificar a remoção com build e publicação; avaliar as referências equivalentes no Console e Infrastructure.
- Há `using` sem uso e estilos inconsistentes. Configurar `.editorconfig`, regras do SDK e `dotnet format` em modo de verificação. Tratar os arquivos do Designer e os gerados por migrations separadamente.
- As telas usam `Debug.WriteLine` e mensagens próprias. Introduzir `ILogger<T>` para registrar falhas, operação e duração. Um provedor de arquivos com rotação facilita investigar problemas nas máquinas dos usuários; evitar registrar o conteúdo financeiro completo.
- Métodos como `TransacaoServico.Registrar` recebem muitos argumentos posicionais. Um DTO de entrada, por exemplo `RegistrarTransacaoEntrada`, melhora a leitura e o reuso futuro pela API. As invariantes continuam nas entidades e nos serviços.
- `MELHORIAS.md` já registra confirmação de transação e avaliação somente nas saídas. São alterações de comportamento e devem ter entregas próprias, separadas das refatorações que apenas reorganizam o código.

## Bibliotecas e ferramentas sugeridas

| Recurso | Aplicação concreta | Ordem |
|---|---|---|
| Analisadores do SDK + `.editorconfig` | Convenções consistentes e detecção automática de problemas; já disponíveis no .NET | Primeiro |
| `Microsoft.Extensions.DependencyInjection` / `Microsoft.Extensions.Hosting` | Composição das telas, serviços, configuração e ciclo de vida | Na organização da aplicação |
| `Microsoft.Extensions.Logging` | Diagnóstico com uma interface comum | Junto da composição |
| `Serilog.Extensions.Hosting` + `Serilog.Sinks.File` | Provedor opcional para logs em arquivos com rotação e retenção | Conforme a necessidade de suporte |
| EF Core já instalado | Projeções, filtros, agregações, índices e concorrência | Manter e otimizar |

Referências: [analisadores incluídos no SDK](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview), [logging no .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/), [Serilog File Sink](https://github.com/serilog/serilog-sinks-file).

## Sequência recomendada

1. Corrigir a diferença de concorrência no saldo SQL Server.
2. Configurar a análise estática e padronizar o código da aplicação.
3. Padronizar composição, fábrica de contexto e diagnóstico.
4. Extrair as consultas de dashboard e extrato; medir antes e depois.
5. Consolidar os pares de repositórios, preservando as diferenças dos provedores.
6. Limitar o estado carregado para exclusões e uniformizar o trabalho em segundo plano nas telas.
7. Retomar a API sobre esses serviços; validar contratos antes de alterar a comunicação do WinForms.

Cada entrega deve compilar, manter as regras financeiras existentes e ter seus fluxos conferidos na aplicação. As alterações posteriores à análise estão registradas em "Atualização após a etapa 1".
