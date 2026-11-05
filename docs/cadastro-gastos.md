# Cadastro e lançamento de gastos

A interface Windows Forms possui a área **Gastos** no lugar do antigo menu Categorias.

## Experimentar a alteração

Abra `JKCASH.sln` no Visual Studio, escolha `JKCASH.WinForms` como projeto de inicialização e execute com F5. A alteração está no código-fonte; executáveis antigos na pasta Distribuicao não recebem essas mudanças automaticamente.

Ao iniciar, a migração local cria a tabela Gastos e acrescenta campos opcionais ao histórico existente. Ela preserva categorias, valores, saldos e lançamentos antigos. Esses lançamentos continuam identificados no extrato como **Sem gasto vinculado**.

Em **Gastos**, use as abas:

- **Cadastrar ou editar gasto:** escolha “Criar novo gasto” ou um cadastro existente. Informe nome, natureza, categoria, importância, necessidade, planejamento, frequência e motivo.
- **Gastos salvos:** consulte os padrões cadastrados.
- **Categorias:** mantenha as classificações auxiliares, inclusive as categorias que já existiam.

Em **Transações**, escolha **Gasto (débito ou crédito)**. É possível selecionar um cadastro salvo ou preencher um novo no mesmo formulário. Ajustes em um gasto salvo valem apenas para aquele lançamento. A opção **Salvar este gasto para reutilizar** aparece para gastos novos; desmarque para registrar somente no histórico.

Informe o valor, a data e a forma de pagamento. Débito exige uma conta ativa. Crédito exige um cartão de conta ativa e permite uma parcela à vista na fatura ou de 2 a 120 parcelas. Comprar no cartão compromete o limite; a conta será debitada no pagamento da fatura.

O motivo é obrigatório se a necessidade **ou** a importância for baixa. Nos demais casos, é opcional. A frequência tem as opções Eventual e Mensal; não gera novos lançamentos automaticamente e não determina o parcelamento.

O novo cadastro reutilizável e o lançamento financeiro são confirmados na mesma operação do banco. Cancelar a confirmação preserva o preenchimento e não cria um cadastro. Editar um cadastro na área Gastos não altera as cópias históricas dos lançamentos anteriores. O extrato apresenta o nome do gasto e seus detalhes; as consultas por categoria continuam disponíveis.

## Compatibilidade

O Console mantém seus menus anteriores. Os novos fluxos de cadastro e seleção de gastos estão no Windows Forms. Para bancos SQL Server existentes, execute o script aditivo `BancoDeDados/Scripts/003-cadastro-gastos.sql` antes de usar esta versão do Console. O aplicativo gráfico aplica automaticamente a migração SQLite ao abrir.

A exportação SQL aceita origens anteriores à alteração: mantém os registros legados sem criar classificações artificiais. Backups locais incluem a nova tabela e continuam aceitando versões antigas para atualização.

## Verificação

A solução compilou durante a implementação. A verificação funcional final fica com o usuário, conforme solicitado. Os últimos ajustes de compatibilidade e espaçamento não foram recompilados após essa solicitação.

## Extrato adaptado ao modelo de gastos

A tabela de movimentações mostra gasto, descrição, natureza, necessidade e frequência. A categoria e o motivo aparecem nos detalhes. O nome do gasto é preenchido pela cópia histórica do lançamento.

Na aba **Relatório de gastos**, a tabela e o gráfico agrupam por gasto e pelas classificações usadas no lançamento. Há filtros de gasto, categoria auxiliar, natureza, frequência, importância, necessidade e planejamento. Sem marcações em um filtro, todas as opções desse filtro são consideradas. O filtro de gasto inclui nomes de cadastros, gastos avulsos e nomes históricos; **Sem gasto vinculado (legado)** seleciona os registros anteriores ao novo modelo.

Lançamentos do mesmo gasto com classificações diferentes aparecem separados, preservando os dados originais. No crédito, o relatório usa o valor integral da compra no período da compra. No débito, transferências internas e pagamentos de fatura são excluídos do relatório de gastos; essas movimentações continuam aparecendo no extrato financeiro e nos seus totais.

Esta atualização não exige uma nova migração. Compilação e testes desta etapa não foram executados, respeitando a preferência de verificação pelo usuário.
