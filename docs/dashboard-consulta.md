# Consulta do dashboard

## O que mudou

`UcDashboard` agora recebe `IDashboardConsulta`, em vez de cinco repositórios. A implementação `DashboardConsulta`, na Infrastructure, usa a fábrica de contexto configurada na inicialização. `FabricaTelas` fornece `ConexaoLocal.CriarContexto` para manter o Windows Forms no SQLite.

O resultado é um `DashboardResumo` da Application: opções de contas, indicadores, resumo por categoria e até oito movimentações. A tela formata valores, aplica cores e desenha os controles, sem carregar entidades de transações, parcelas ou pagamentos para calcular os indicadores.

## Regras preservadas

- Saldo atual e limites atuais das contas selecionadas, incluindo contas inativas na consulta histórica.
- Receitas, despesas e categorias limitadas ao mês selecionado, excluindo transferências internas desses totais.
- As transferências continuam aparecendo nas movimentações recentes com sua identificação própria.
- Faturas pendentes são a soma das parcelas menos os pagamentos dos ciclos com vencimento no mês, nos cartões das contas selecionadas.
- Saldo inicial não é usado para reconstruir o saldo atual, e compras no cartão não geram despesas da conta em duplicidade.
- Conta selecionada que deixou de existir retorna ao filtro de todas as contas, como antes.

## Consultas menores

O filtro de datas usa `Data >= inicio && Data < primeiroDiaDoMesSeguinte`. As somas são calculadas no banco e as oito últimas movimentações são limitadas antes de materializar o resultado.

Não se consultam mais as faturas individualmente para cada cartão. Os filtros de cartões, faturas, parcelas e pagamentos são combinados por subconsultas. Parcelas e pagamentos são somados separadamente para que uma junção entre várias parcelas e vários pagamentos não multiplique valores.

A execução utiliza até nove SELECTs: opções de contas, saldo, totais por tipo, totais por categoria, nomes das categorias utilizadas, resumo de crédito, soma das parcelas, soma dos pagamentos e movimentações recentes. Esse número não cresce uma vez por cartão. Há também os comandos de controle da transação de leitura.

Os resultados resumidos ainda crescem com o número de contas e categorias distintas, mas não transportam todo o histórico mensal de transações nem as coleções completas das faturas. A ordenação das categorias desempata por nome e Id.

## Valores exatos e consistência

No SQLite, os campos monetários já estão armazenados como centavos INTEGER pelo conversor existente. A consulta soma essas colunas como `long` e divide o resultado por `100m` para devolver reais. No SQL Server, soma os campos `decimal`. Não utiliza `double` nem aplica o conversor novamente ao resultado em centavos.

Todos os indicadores usam um único contexto e uma transação serializável de leitura. No SQLite a transação começa de maneira adiada, permitindo leitura consistente sem reservar uma transação de escrita antes de consultar. O contexto e a transação são descartados ao terminar.

A execução continua em `Task.Run`, fora da thread visual, com os filtros e o botão de atualização desabilitados durante o carregamento. O fechamento da tela é verificado antes de atualizar controles.

## Verificação e limites

- Solução compilada sem erros ou avisos.
- Consulta executada no SQLite existente com conexão explicitamente em modo somente leitura.
- Indicadores comparados com o cálculo anterior para todas as contas e um identificador inexistente. O banco estava sem contas; as duas comparações confirmaram apenas o estado vazio e o fallback do filtro. Não foram criados registros de teste.
- A leitura executou as novas consultas SQLite, mas a conferência com histórico preenchido, execução no SQL Server e interação visual ainda estão pendentes.
- Não há percentual de ganho medido nem benchmark com volume representativo. A mudança reduz materialização e consultas por cartão; o ganho efetivo precisa ser medido.
- Nenhuma alteração de esquema, migração, importação ou gravação de registros foi realizada nesta entrega.
